using RedBlueGames.LiteFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryWorker : EnemyBase
{
	[SerializeField] private Transform edgeCheckTransform;
	[SerializeField] private float edgeCheckDistance;
	[Space]
	[SerializeField] private float wallCheckDistance;
	[Space]			 
	[SerializeField] private bool canJump;
	[SerializeField] private float jumpVelocity;
	[Space]			 
	[SerializeField] private float walkVelocity;
	[SerializeField] private float chasingVelocity;
	[Space]
	[SerializeField] private Vector2 idleIntervals;
	[SerializeField] private Vector2 walkIntervals;
	[Space]
	[SerializeField] private Vector2 attackBoxPosition;
	[SerializeField] private Vector2 attackBoxSize;

	private float currentInterval;

	public enum States
	{
		Idle, 
		Walk,
		Notice,
		Chase,
		Attack,
		Damage,
	}

	private StateMachine<States> fsm;

	public override void Rise()
	{
		base.Rise();

		SetupFSM();
		//attackCollider.attachedRigidbody.
	}

	public override void Tick()
	{
		base.Tick();

		fsm.Update(Time.deltaTime);
	}

	private void SetupFSM()
	{
		var states = new List<State<States>>();

		states.Add(new State<States>(States.Idle,	EnterIdle, null, UpdateIdle));
		states.Add(new State<States>(States.Attack, EnterAttack, null, null));
		states.Add(new State<States>(States.Walk,	EnterWalk, null, UpdateWalk));
		states.Add(new State<States>(States.Chase,	EnterChase, null, UpdateChase));
		states.Add(new State<States>(States.Notice, EnterNotice, null, null));
		states.Add(new State<States>(States.Damage, EnterDamage, null, null));

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

	//IDLE
	private void EnterIdle()
	{
		anim.Play("Idle");
		currentInterval = Random.Range(idleIntervals.x, idleIntervals.y);
	}

	private void UpdateIdle(float d)
	{
		if(IsPlayerInSearchRadius)
		{
			fsm.ChangeState(States.Notice);
			currentInterval = 0;
			return;
		}

		if(currentInterval <= 0)
		{
			fsm.ChangeState(States.Walk);
		}

		currentInterval -= d;
	}

	//WALK
	private void EnterWalk()
	{
		anim.Play("Walk");
		currentInterval = Random.Range(walkIntervals.x, walkIntervals.y);
	}

	private void UpdateWalk(float d)
	{
		if (IsPlayerInSearchRadius)
		{
			fsm.ChangeState(States.Notice);
			currentInterval = 0;
			return;
		}

		if (currentInterval <= 0)
		{
			fsm.ChangeState(States.Idle);
		}

		currentInterval -= d;
	}

	//CHASE
	private void EnterChase()
	{
		anim.Play("Chase");
	}

	private void UpdateChase(float d)
	{

	}

	//ATTACK
	private void EnterAttack()
	{
		anim.Play("Attack");
	}

	//ATTACK
	private void EnterNotice()
	{
		anim.Play("Notice");
	}

	private void EnterDamage()
	{
		anim.Play("Damage");
	}

	#endregion

	public void AttackCheck()
	{
		Collider2D collider = Physics2D.OverlapBox(position + attackBoxPosition, attackBoxSize, 0, playerLayer);

		if(collider)
		{
			collider.TryGetComponent(out Player player);

			player.TryTakeDamage(new HitInfo(damage, HitSide.CalculateHitSide(position, player.position)));
		}
	}

	private bool EdgeCheck()
	{
		RaycastHit2D hit = Physics2D.Raycast(edgeCheckTransform.position, Vector2.down, 500, groundLayer);

		if(hit)
		{
			float angle = Vector2.Angle(Vector2.up, hit.normal);

			if (angle == 0)
			{
				//if(collisions.climbingSlope)
				//{
				//	return false;
				//}

				if(Mathf.Abs(edgeCheckTransform.position.y - hit.point.y) > edgeCheckDistance)
				{
					return true;
				}
			}

			//if(angle > 0 && angle < maxSlopeAngle)
			//{
			//	return false;
			//}

			return true;
		}

		return true;
	}

#if UNITY_EDITOR
	protected override void OnDrawGizmosSelected()
	{
		if (!t) return;

		base.OnDrawGizmosSelected();

		Gizmos.color = Color.red;

		Gizmos.DrawWireCube(t.position + new Vector3(attackBoxPosition.x, attackBoxPosition.y), new Vector3(attackBoxSize.x, attackBoxSize.y, 1));
	}
#endif
}
