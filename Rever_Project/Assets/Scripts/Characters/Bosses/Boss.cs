using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Boss : MonoCached
{
	[SerializeField] protected ArenaController arena;
	[SerializeField] protected Transform playerCheckPositionFrom;

	protected GameManager game;
	protected MessageManager msg;

    protected Health bossHealth;
	protected Animator anim;
	protected Transform playerTransform;
	protected Player player;

	[Inject]
	public void Constructor(GameManager game, MessageManager msg)
	{
		this.game = game;
		this.msg = msg;
	}

	public override void Rise()
	{
		bossHealth = GetComponent<Health>();
		anim = GetComponentInChildren<Animator>();
		playerTransform = game.CurrentPlayer.transform;
		player = game.CurrentPlayer;

		bossHealth.Initialize();
	}

	protected Vector2 GetDirectionToPlayer()
	{
		return playerCheckPositionFrom.position - playerTransform.position;
	}
}
