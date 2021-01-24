using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using NaughtyAttributes;

public class ArenaController : Saveable
{
	[SerializeField] private float afterDoorCloseWaitTime;
	[SerializeField] private List<Door> doorsToClose;

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
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (defeated) return;

		if(collision.CompareTag(game.PlayerTag))
		{
			StartCoroutine(PrepareArenaCoroutine());
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

	private IEnumerator PrepareArenaCoroutine()
	{
		input.TrySetDefaultInputActive(false, true);
		confiner.ConfineCamera();
		doorsToClose.ForEach(x => x.CloseDoor());

		yield return new WaitForSeconds(afterDoorCloseWaitTime);

		Debug.Log("BOSS APPEAR");

		//TEMP
		yield return new WaitForSeconds(2);

		Debug.Log("BOSS DEFEATED");

		yield return new WaitForSeconds(2);

		defeated = true;
		doorsToClose.ForEach(x => x.OpenDoor());
		confiner.ResetCamera();
		input.TrySetDefaultInputActive(true, true);
	}
}
