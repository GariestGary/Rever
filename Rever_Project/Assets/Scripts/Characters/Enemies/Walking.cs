using RedBlueGames.LiteFSM;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Walking : EnemyBase
{
	[SerializeField] private Transform edgeCheckTransform;
	[SerializeField] private float edgeCheckDistance;
	[Space]
	[SerializeField] private float wallCheckDistance;
	[Space]			 
	[SerializeField] private float walkVelocity;
	[SerializeField] private float chasingVelocity;
	[SerializeField] private float chaseIdleDistance;
	[Space]
	[SerializeField] private Vector2 idleIntervals;
	[SerializeField] private Vector2 walkIntervals;
	[SerializeField] private Vector2 attackIntervals;
	[Space]
	[SerializeField] private float firstAttackDelay;
	[SerializeField] private float damageTime;
	[SerializeField] private float attackTime;
	[SerializeField] private float noticeTime;
	[SerializeField] private float jumpTime;

	private float currentInterval;
	private bool needToChangeDirection;
	private bool firstAttack;

	public enum States
	{
		Idle, 
		Walk,
		Notice,
		Chase,
		ChaseIdle,
		Attack,
		Damage,
		AttackIdle,
		Falling,
		Jump,
	}

	private StateMachine<States> fsm;

	public override void Rise()
	{
		base.Rise();
	}

	public override void Ready()
	{
		SetupFSM();
	}

	public override void Tick()
	{
		base.Tick();

		fsm.Update(Time.deltaTime);
		TurnHandle();
	}

	protected virtual void SetupFSM()
	{
		var states = new List<State<States>>();

		states.Add(new State<States>(States.Idle,	EnterIdle, null, UpdateIdle));
		states.Add(new State<States>(States.Attack, EnterAttack, null, UpdateAttack));
		states.Add(new State<States>(States.Walk,	EnterWalk, null, UpdateWalk));
		states.Add(new State<States>(States.Chase,	EnterChase, null, UpdateChase));
		states.Add(new State<States>(States.Notice, EnterNotice, null, UpdateNotice));
		states.Add(new State<States>(States.Damage, EnterDamage, null, UpdateDamage));
		states.Add(new State<States>(States.AttackIdle, EnterAttackIdle, null, UpdateAttackIdle));
		states.Add(new State<States>(States.Falling, EnterFalling, null, UpdateFalling));
		states.Add(new State<States>(States.Jump,	EnterJump, null, UpdateJump));
		states.Add(new State<States>(States.ChaseIdle, EnterChaseIdle, null, UpdateChaseIdle));

		fsm = new StateMachine<States>(states.ToArray(), States.Idle);
	}

	public void SetState(States state)
	{
		if(state == fsm.CurrentStateID)
		{
			return;
		}

		fsm.ChangeState(state);
	}

	#region states

	//IDLE=======================================================
	protected virtual void EnterIdle()
	{
		if(IsPlayerInChaseRadius)
		{
			fsm.ChangeState(States.Chase);
			return;
		}

		anim.Play("Idle");
		currentInterval = UnityEngine.Random.Range(idleIntervals.x, idleIntervals.y);
		controller.SetVelocity(Vector2.zero);
	}

	protected virtual void UpdateIdle(float d)
	{
		CheckGround();

		if (IsPlayerInSearchRadius)
		{
			fsm.ChangeState(States.Notice);
			return;
		}

		if (currentInterval <= 0)
		{
			fsm.ChangeState(States.Walk);
			return;
		}

		currentInterval -= d;
	}

	//TODO: Jump when player above, stay when cannot jump and player in center

	//WALK=============================================================
	protected virtual void EnterWalk()
	{
		anim.Play("Walk");
		currentInterval = UnityEngine.Random.Range(walkIntervals.x, walkIntervals.y);

		if(needToChangeDirection)
		{
			SetFacingDirection(-FacingDirection);
			needToChangeDirection = false;
		}
		else
		{
			SetFacingDirection(UnityEngine.Random.Range(-1, 1));
		}
	}

	protected virtual void UpdateWalk(float d)
	{
		CheckGround();

		if (IsPlayerInSearchRadius)
		{
			fsm.ChangeState(States.Notice);
			return;
		}

		if (IsLedgeAhead() || IsWallAhead(c.bounds.size.x))
		{
			fsm.ChangeState(States.Idle);
			needToChangeDirection = true;
			return;
		}

		controller.Move(FacingDirection, walkVelocity);


		if (currentInterval <= 0)
		{
			fsm.ChangeState(States.Idle);
			return;
		}

		currentInterval -= d;
	}

	//FALLING===================================================
	protected virtual void EnterFalling()
	{
		//anim.Play("Falling");
	}

	protected virtual void UpdateFalling(float d)
	{
		if(controller.Collision.below)
		{
			fsm.ChangeState(States.Idle);
			return;
		}
	}

	//CHASE==================================================
	protected virtual void EnterChase()
	{
		firstAttack = true;
		anim.Play("Chase");
		SetFacingDirection(game.CurrentPlayer.transform.position.x > t.position.x ? 1 : -1);
	}

	protected virtual void UpdateChase(float d)
	{
		CheckGround();

		if (!IsPlayerInChaseRadius)
		{
			fsm.ChangeState(States.Idle);
			return;
		}

		if(IsPlayerInAttackRadius)
		{
			fsm.ChangeState(States.AttackIdle);
			return;
		}
		else
		{
			if (Mathf.Abs(game.CurrentPlayer.transform.position.x - position.x) < chaseIdleDistance)
			{
				fsm.ChangeState(States.ChaseIdle);
				return;
			}
		}

		if(IsWallAhead(c.bounds.size.x * 2))
		{
			if(IsJumpDestinationAvailable(FacingDirection, jumpVelocity))
			{
				fsm.ChangeState(States.Jump);
				return;
			}
		}

		if (IsLedgeAhead() && IsJumpDestinationAvailable(FacingDirection, jumpVelocity))
		{
			fsm.ChangeState(States.Jump);
			return;
		}

		FaceToPlayer();

		controller.Move(FacingDirection, chasingVelocity);
	}

	//CHASE IDLE
	protected virtual void EnterChaseIdle()
	{
		
	}

	protected virtual void UpdateChaseIdle(float d)
	{
		if(!IsPlayerInChaseRadius)
		{
			fsm.ChangeState(States.Idle);
			return;
		}

		if((Mathf.Abs(game.CurrentPlayer.transform.position.x - position.x) > chaseIdleDistance) || !IsLedgeAhead())
		{
			fsm.ChangeState(States.Chase);
			return;
		}

		FaceToPlayer();
	}

	//ATTACK IDLE=============================================
	protected virtual void EnterAttackIdle()
	{
		anim.Play("Idle");

		controller.SetVelocity(Vector2.zero);

		if(firstAttack)
		{
			currentInterval = firstAttackDelay;
			firstAttack = false;
		}
		else
		{
			currentInterval = UnityEngine.Random.Range(attackIntervals.x, attackIntervals.y);
		}
		
	}

	protected virtual void UpdateAttackIdle(float d)
	{
		if(!IsPlayerInAttackRadius)
		{
			fsm.ChangeState(States.Chase);
			return;
		}

		if(currentInterval <= 0)
		{
			fsm.ChangeState(States.Attack);
			return;
		}

		FaceToPlayer();

		currentInterval -= d;
	}

	//ATTACK==============================================
	protected virtual void EnterAttack()
	{
		anim.Play("Attack");
		currentInterval = attackTime;
		controller.SetVelocity(Vector2.zero);
		AttackCheck();
	}

	protected virtual void UpdateAttack(float d)
	{
		if (currentInterval <= 0)
		{
			fsm.ChangeState(States.AttackIdle);
			return;
		}

		currentInterval -= d;
	}

	//NOTICE=============================================
	protected virtual void EnterNotice()
	{
		FaceToPlayer();
		anim.Play("Notice");
		currentInterval = noticeTime;
	}

	protected virtual void UpdateNotice(float d)
	{
		if(currentInterval <= 0)
		{
			fsm.ChangeState(States.Chase);
			return;
		}

		currentInterval -= d;
	}

	//DAMAGE==============================================
	protected virtual void EnterDamage()
	{
		anim.Play("Damage");
		currentInterval = damageTime;
	}

	protected virtual void UpdateDamage(float d)
	{
		if (currentInterval <= 0)
		{
			fsm.ChangeState(States.Idle);
			return;
		}

		currentInterval -= d;
	}

	//JUMP===============================================
	protected virtual void EnterJump()
	{
		Jump(FacingDirection, jumpVelocity);
		currentInterval = jumpTime;
	}

	protected virtual void UpdateJump(float d)
	{
		if (controller.Collision.above)
		{
			fsm.ChangeState(States.Falling);
		}

		if (currentInterval <= 0)
		{
			fsm.ChangeState(States.Falling);
			return;
		}

		currentInterval -= d;
	}

	#endregion

	private void CheckGround()
	{
		if(!controller.Collision.below)
		{
			fsm.ChangeState(States.Falling);
		}
	}

	protected override void SetFacingDirection(int dir)
	{
		base.SetFacingDirection(dir);

		edgeCheckTransform.localPosition = new Vector3(Mathf.Abs(edgeCheckTransform.localPosition.x) * dir, edgeCheckTransform.localPosition.y, 0);
	}

	private bool IsLedgeAhead()
	{
		RaycastHit2D hit = Physics2D.Raycast(edgeCheckTransform.position, Vector2.down, 500, groundLayer);

		Debug.DrawRay(edgeCheckTransform.position, Vector2.down, Color.red);

		if(hit)
		{
			Debug.DrawRay(edgeCheckTransform.position, edgeCheckTransform.position - new Vector3(hit.point.x, hit.point.y), Color.green);
			Debug.DrawRay(hit.point, hit.normal, Color.green);

			float angle = Vector2.Angle(Vector2.up, hit.normal);

			if (angle == 0)
			{
				if (controller.Collision.climbingSlope)
				{
					return false;
				}

				if (Mathf.Abs(edgeCheckTransform.position.y - hit.point.y) > edgeCheckDistance)
				{
					return true;
				}
			}

			if (angle < controller.MaxSlopeAngle)
			{
				return false;
			}

			return true;
		}

		return true;
	}
}
