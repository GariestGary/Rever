using RedBlueGames.LiteFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying : EnemyBase
{
    [SerializeField] protected float flySpeed;

	public enum States
	{
		Idle,
		Chase,
		Attack,
		Fly,
	}


	private StateMachine<States> fsm;

	public override void Rise()
	{
		base.Rise();
	}
}
