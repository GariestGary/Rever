using Pathfinding;
using RedBlueGames.LiteFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : DefaultCharacter
{
	[SerializeField] protected float searchRadius = 3;
	[SerializeField] protected float chaseRadius = 5;
	[SerializeField] protected float attackDistance = 0.8f;
	[SerializeField] protected float attackInterval = 1;
	[SerializeField] protected LayerMask playerMask;

	protected enum DefaultEnemyStates
	{
		Search,
		Chase,
		Attack,
	}

	protected AIPath path;
	protected Transform t;
	protected List<Vector3> remainingPath = new List<Vector3>();
	protected bool stalePath;
	protected float currentAttackInterval = 0;

	protected StateMachine<DefaultEnemyStates> stateMachine;

	public override void OnAwake()
	{
		base.OnAwake();

		path = GetComponent<AIPath>();
		t = transform;

		RegisterStates();
	}

	protected void RegisterStates()
	{
		var stateList = new List<State<DefaultEnemyStates>>();
		stateList.Add(new State<DefaultEnemyStates>(DefaultEnemyStates.Search, null, null, this.SearchStateUpdate));
		stateList.Add(new State<DefaultEnemyStates>(DefaultEnemyStates.Chase, null, null, this.ChaseStateUpdate));
		stateList.Add(new State<DefaultEnemyStates>(DefaultEnemyStates.Attack, null, null, this.AttackStateUpdate));
		this.stateMachine = new StateMachine<DefaultEnemyStates>(stateList.ToArray(), DefaultEnemyStates.Search);
	}

	protected void SearchStateUpdate(float d)
	{
		Collider2D player = Physics2D.OverlapCircle(t.position, searchRadius, playerMask);

		if (player && Vector3.Distance(t.position, player.transform.position) > attackDistance)
		{
			path.enabled = true;
			path.destination = player.transform.position;
			stateMachine.ChangeState(DefaultEnemyStates.Chase);
		}
		else
		{
			path.destination = t.position;
			path.enabled = false;
		}
	}

	protected void ChaseStateUpdate(float d)
	{
		if(Vector3.Distance(t.position, path.destination) > chaseRadius)
		{
			path.enabled = false;
			stateMachine.ChangeState(DefaultEnemyStates.Search);
		}

		path.GetRemainingPath(remainingPath, out stalePath);

		Vector3 desiredPath = Vector3.zero;

		if(path.enabled && remainingPath.Count > 3)
		{
			desiredPath = remainingPath[2] - remainingPath[1];
		}

		movement.MovementUpdate(desiredPath.normalized);
	}

	protected void AttackStateUpdate(float d)
	{
		if(currentAttackInterval <= 0)
		{
			if(Vector3.Distance(t.position, path.destination) > attackDistance && currentAttackInterval <= 0)
			{
				stateMachine.ChangeState(DefaultEnemyStates.Chase);
			}

			currentAttackInterval = attackInterval;
			anim.SetTrigger("Attack");
		}

		currentAttackInterval -= d;
	}

	public override void OnTick()
	{
		base.OnTick();

		stateMachine.Update(Time.deltaTime);
		AnimatorVariablesUpdate();
	}

	public override void OnFixedTick()
	{
		base.OnFixedTick();

		movement.FixedMovementUpdate();
	}

	protected virtual void AnimatorVariablesUpdate()
	{
		anim.SetBool("Moving", movement.Velocity != Vector2.zero);

		anim.SetFloat("Velocity X", movement.Velocity.x);
		anim.SetFloat("Velocity Y", movement.Velocity.y);

		anim.SetBool("Jumping", movement.IsJumping);
		anim.SetBool("Falling", movement.IsFalling);
		anim.SetBool("Grounded", movement.IsGrounded);
	}
}
