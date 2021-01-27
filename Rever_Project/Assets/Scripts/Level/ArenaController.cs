using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using NaughtyAttributes;
using UnityEngine.Events;
using System;
using UniRx;

public class ArenaController : Saveable
{
	[SerializeField] private float afterDoorCloseWaitTime;
	[SerializeField] private float afterConfineWaitTime;
	[SerializeField] private List<Door> doorsToClose;
	[SerializeField] private bool useSwitcher;
	[ShowIf("useSwitcher")] [SerializeField] private MonoBehaviour switcher;

	public UnityEvent ArenaPreparedEvent;

	private bool defeated = false;

    private CameraConfinerComponent confiner;
	private IDisposable bossDeathMessageDisposable;

	private GameManager game;
	private InputManager input;
	private MessageManager msg;

	[Inject]
	public void Constructor(GameManager game, InputManager input, MessageManager msg)
	{
		this.game = game;
		this.input = input;
		this.msg = msg;
	}

	public override void Rise()
	{
		confiner = GetComponent<CameraConfinerComponent>();

		if(useSwitcher && switcher)
		{
			(switcher as ISwitcher).EnableEvent.AddListener(PrepareArena);
		}
	}

	private void PrepareArena()
	{
		StartCoroutine(PrepareArenaCoroutine());
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (defeated || useSwitcher) return;

		if(collision.CompareTag(game.PlayerTag))
		{
			PrepareArena();
		}
	}

	public override void RestoreState(string state)
	{
		defeated = JsonConvert.DeserializeObject<bool>(state);
	}

	public override object CaptureState()
	{
		return defeated;
	}

	public void EnablePlayer()
	{
		input.TrySetDefaultInputActive(true, true);
	}

	private IEnumerator PrepareArenaCoroutine()
	{
		bossDeathMessageDisposable = msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.BOSS_DEFEATED).Subscribe( _ => StartCoroutine(OnBossDefeatedCoroutine()));

		doorsToClose.ForEach(x => x.CloseDoor());
		input.TrySetDefaultInputActive(false, true);
		yield return new WaitForSeconds(afterDoorCloseWaitTime);

		confiner.ConfineCamera();


		yield return new WaitForSeconds(afterConfineWaitTime);

		ArenaPreparedEvent?.Invoke();

		//TEMP
		//yield return new WaitForSeconds(2);

		//Debug.Log("BOSS DEFEATED");

		//yield return new WaitForSeconds(2);

		//defeated = true;
		//doorsToClose.ForEach(x => x.OpenDoor());
		//confiner.ResetCamera();
		//input.TrySetDefaultInputActive(true, true);
	}

	private IEnumerator OnBossDefeatedCoroutine()
	{
		yield return new WaitForSeconds(2);

		defeated = true;
		doorsToClose.ForEach(x => x.OpenDoor());
		confiner.ResetCamera();
		input.TrySetDefaultInputActive(true, true);

		bossDeathMessageDisposable.Dispose();
	}

	public override void OnRemove()
	{
		if (switcher) (switcher as ISwitcher).EnableEvent.RemoveListener(PrepareArena);
	}
}
