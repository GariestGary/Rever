using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ConfinerSetup : MonoBehaviour, IAwake
{
	private GameManager game;

	[Inject]
	public void Constructor(GameManager game)
	{
		this.game = game;
	}

	public void OnAwake()
	{
		game.SetCameraConfiner(GetComponent<PolygonCollider2D>());
	}
}
