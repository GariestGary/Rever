using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBase : MonoCached
{
	[SerializeField] protected int groundCheckSteps;
	[SerializeField] protected float playerSearchRadius;
	[SerializeField] protected float playerChaseRadius;
	[SerializeField] protected float maxAllowedSlopeAngle;
	[SerializeField] protected LayerMask playerLayer;
	[SerializeField] protected LayerMask groundLayer;
	[Space]
	[SerializeField] protected int damage;

	public UnityEvent PlayerNoticedEvent;
	public UnityEvent PlayerLostEvent;

	protected Rigidbody2D rb;
	protected Collider2D c;
	protected Animator anim;
	protected Transform t;
	protected Health health;
	protected bool facingRight;

	protected Player noticedPlayer;

	public Health SelfHealth => health;

	public bool IsPlayerAbove => IsPlayerNoticed && (noticedPlayer.transform.position.y >= t.position.y);
	public bool IsPlayerBelow => IsPlayerNoticed && (noticedPlayer.transform.position.y < t.position.y);
	public bool IsPlayerOnRight => IsPlayerNoticed && (noticedPlayer.transform.position.x >= t.position.x);
	public bool IsPlayerOnLeft => IsPlayerNoticed && (noticedPlayer.transform.position.x < t.position.x);

	public bool IsPlayerNoticed => noticedPlayer;

	public bool IsPlayerInSearchRadius => CheckPlayerInRadius(playerSearchRadius);

	public bool IsPlayerInChaseRadius => CheckPlayerInRadius(playerChaseRadius);

	public override void Rise()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		t = transform;
		c = GetComponent<Collider2D>();
	}

	public override void Tick()
	{

	}

	protected bool IsGrounded()
	{
		float spacing = c.bounds.size.x / (groundCheckSteps - 1);
		Vector2 startPos = new Vector2(c.bounds.min.x, c.bounds.min.y);

		for (int i = 0; i < groundCheckSteps; i++)
		{
			Vector2 origin = startPos + Vector2.right * spacing * i;

			Debug.DrawRay(origin, Vector2.down * 1, Color.red);

			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, groundLayer);

			if(hit)
			{
				return true;
			}
		}

		return false;
	}

	protected bool IsOnSlope()
	{
		float spacing = c.bounds.size.x / (groundCheckSteps - 1);
		Vector2 startPos = new Vector2(c.bounds.min.x, c.bounds.min.y);

		for (int i = 0; i < groundCheckSteps; i++)
		{
			Vector2 origin = startPos + Vector2.right * spacing * i;

			Debug.DrawRay(origin, Vector2.down * 1, Color.red);

			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, groundLayer);

			if (Vector2.Angle(Vector2.up, hit.normal) != 0)
			{
				return true;
			}
		}

		return false;
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
				PlayerNoticedEvent?.Invoke();
			}
			
			return true;
		}
		else
		{
			if(IsPlayerNoticed)
			{
				PlayerLostEvent?.Invoke();
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
