using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ArenaController : MonoCached
{
    private CameraConfinerComponent confiner;

	private GameManager game;

	[Inject]
	public void Constructor(GameManager game)
	{
		this.game = game;
	}

	public override void Rise()
	{
		confiner = GetComponent<CameraConfinerComponent>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag(game.PlayerTag))
		{
			confiner.ConfineCamera();
		}
	}
}
