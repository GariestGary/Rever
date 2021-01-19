using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.Events;

public class EnemyBase : MonoCached
{
	[SerializeField] protected Transform groundCheckTransform;
	[SerializeField] protected float playerSearchRadius;
	[SerializeField] protected float playerChaseRadius;
	[SerializeField] protected LayerMask playerLayer;
	[SerializeField] protected LayerMask groundLayer;
	[Space]
	[SerializeField] protected int damage;

	public UnityEvent PlayerNoticedEvent;
	public UnityEvent PlayerLostEvent;

	protected Rigidbody2D rb;
	protected Animator anim;
	protected PandaBehaviour pb;
	protected Transform t;
	protected Health health;

	protected Player noticedPlayer;

	public Health SelfHealth => health;

	[Task]
	public bool IsPlayerAbove => IsPlayerNoticed && (noticedPlayer.transform.position.y >= t.position.y);
	[Task]
	public bool IsPlayerBelow => IsPlayerNoticed && (noticedPlayer.transform.position.y < t.position.y);
	[Task]
	public bool IsPlayerOnRight => IsPlayerNoticed && (noticedPlayer.transform.position.x >= t.position.x);
	[Task]
	public bool IsPlayerOnLeft => IsPlayerNoticed && (noticedPlayer.transform.position.x < t.position.x);

	[Task]
	public bool IsPlayerNoticed => noticedPlayer;

	[Task]
	public bool IsPlayerInSearchRadius => CheckPlayerInRadius(playerSearchRadius);

	[Task]
	public bool IsPlayerInChaseRadius => CheckPlayerInRadius(playerChaseRadius);

	public override void Rise()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		t = transform;
		pb = GetComponent<PandaBehaviour>();
	}

	public override void Tick()
	{
		//pb.Reset();
		pb.Tick();
	}

	[Task]
	protected void GroundedCheck()
	{
		if(Physics2D.OverlapCircle(groundCheckTransform.position, 0.2f, groundLayer))
		{
			Task.current.Succeed();
		}
		else
		{
			Task.current.Fail();
		}
	}

	public Vector2 GetDirectionToPlayer()
	{
		if(noticedPlayer)
		{
			return noticedPlayer.transform.position - t.position;
		}

		return Vector2.zero;
	}

	public void TryHitPlayer()
	{
		if(!IsPlayerNoticed)
		{
			return;
		}

		if(noticedPlayer.IsInvulnerable)
		{
			return;
		}

		noticedPlayer.TryTakeDamage(damage, HitSide.CalculateHitSide(t.position, noticedPlayer.transform.position));
	}

	protected bool CheckPlayerInRadius(float radius)
	{
		Collider2D playerCollider = Physics2D.OverlapCircle(t.position, radius, playerLayer);

		if(playerCollider)
		{
			if(!IsPlayerNoticed)
			{
				noticedPlayer = playerCollider.GetComponent<Player>();
				pb.Reset();
			}
			
			return true;
		}
		else
		{
			if(IsPlayerNoticed)
			{
				noticedPlayer = null;
			}
			
			return false;
		}

		
	}

#if UNITY_EDITOR
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, playerSearchRadius);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, playerChaseRadius);
	}
#endif
}
