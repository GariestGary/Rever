using RedBlueGames.LiteFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying : EnemyBase
{
    [SerializeField] protected float flySpeed;
	[SerializeField] protected float acceleration;
	[SerializeField] protected float deceleration;

	public enum States
	{
		Idle,
		Chase,
		Attack,
		Fly,
	}

	private StateMachine<States> fsm;

	public override void Ready()
	{
		base.Ready();
		SetupFSM();
	}

	public override void Tick()
	{
		base.Tick();

		fsm.Update(Time.deltaTime);
		TurnHandle();
	}

	private void SetupFSM()
	{
		var states = new List<State<States>>();
		states.Add(new State<States>(States.Attack, EnterAttack, null, UpdateAttack));
		states.Add(new State<States>(States.Chase, EnterChase, null, UpdateChase));
		states.Add(new State<States>(States.Fly, EnterFly, null, UpdateFly));
		states.Add(new State<States>(States.Idle, EnterIdle, null, UpdateIdle));
		fsm = new StateMachine<States>(states.ToArray(), States.Idle);
	}

	#region states

	//ATTACK===============================================
	protected virtual void EnterAttack()
	{

	}

	protected virtual void UpdateAttack(float d)
	{

	}

	//FLY===============================================
	protected virtual void EnterFly()
	{

	}

	protected virtual void UpdateFly(float d)
	{

	}

	//IDLE==============================================
	protected virtual void EnterIdle()
	{
		anim.Play("Idle");
	}

	protected virtual void UpdateIdle(float d)
	{
		AttackCheck();
		
		if(controller.RB.velocity != Vector2.zero)
		{
			controller.RB.velocity = Vector2.Lerp(controller.RB.velocity, Vector2.zero, deceleration * Time.deltaTime);
		}

		if(IsPlayerInSearchRadius)
		{
			fsm.ChangeState(States.Chase);
			return;
		}
	}

	//CHASE===============================================
	protected virtual void EnterChase()
	{

	}

	protected virtual void UpdateChase(float d)
	{
		AttackCheck();

		if(!IsPlayerInChaseRadius)
		{
			fsm.ChangeState(States.Idle);
			return;
		}

		FaceToPlayer();

		controller.RB.AddForce(GetDirectionToPlayer().normalized * acceleration);
		controller.RB.velocity = Vector2.ClampMagnitude(controller.RB.velocity, flySpeed);
	}

	#endregion


}
