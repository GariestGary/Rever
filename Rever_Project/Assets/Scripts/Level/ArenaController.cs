using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using NaughtyAttributes;
using UnityEngine.Events;

public class ArenaController : Saveable
{
	[SerializeField] private float afterDoorCloseWaitTime;
	[SerializeField] private List<Door> doorsToClose;
	[SerializeField] private bool useSwitcher;
	[ShowIf("useSwitcher")] [SerializeField] private MonoBehaviour switcher;

	public UnityEvent ArenaPreparedEvent;

	private bool defeated = false;

    private CameraConfinerComponent confiner;

	private GameManager game;
	private InputManager input;

	[Inject]
	public void Constructor(GameManager game, InputManager input)
	{
		this.game = game;
		this.input = input;
	}

	public override void Rise()
	{
		confiner = GetComponent<CameraConfinerComponent>();

		if(useSwitcher && switcher)
		{
			(switcher as ISwitcher).OnEnable += PrepareArena;
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
		input.TrySetDefaultInputActive(false, true);
		confiner.ConfineCamera();
		doorsToClose.ForEach(x => x.CloseDoor());

		yield return new WaitForSeconds(afterDoorCloseWaitTime);

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

	public override void OnRemove()
	{
		if (switcher) (switcher as ISwitcher).OnEnable -= PrepareArena;
	}
}
