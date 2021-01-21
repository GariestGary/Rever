using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class EnemyBase : MonoCached
{
	[SerializeField] protected int groundCheckSteps;
	[SerializeField] protected float playerSearchRadius;
	[SerializeField] protected float playerChaseRadius;
	[Space]
	[SerializeField] private int wallCheckSteps;
	[Space]
	[SerializeField] protected LayerMask playerLayer;
	[SerializeField] protected LayerMask groundLayer;
	[Space]
	[SerializeField] protected int damage;

	public UnityEvent PlayerNoticedEvent;
	public UnityEvent PlayerLostEvent;

	protected Collider2D c;
	protected RigidBody2DController rbController;
	protected Animator anim;
	protected Health health;
	protected Player noticedPlayer;
	protected Transform t;
	protected float slopeAngle;
	protected bool facingRight;

	protected GameManager game;


	public Health SelfHealth => health;

	public bool IsPlayerAbove => IsPlayerNoticed && (noticedPlayer.transform.position.y >= t.position.y);
	public bool IsPlayerBelow => IsPlayerNoticed && (noticedPlayer.transform.position.y < t.position.y);
	public bool IsPlayerOnRight => IsPlayerNoticed && (noticedPlayer.transform.position.x >= t.position.x);
	public bool IsPlayerOnLeft => IsPlayerNoticed && (noticedPlayer.transform.position.x < t.position.x);

	public Vector2 position => new Vector2(t.position.x, t.position.y);

	public bool IsPlayerNoticed => noticedPlayer;

	public bool IsPlayerInSearchRadius => CheckPlayerInRadius(playerSearchRadius);

	public bool IsPlayerInChaseRadius => CheckPlayerInRadius(playerChaseRadius);

	[Inject]
	public void Construct(GameManager game)
	{
		this.game = game;
	}

	public override void Rise()
	{
		base.Rise();

		t = transform;
		rbController = GetComponent<RigidBody2DController>();
		anim = GetComponent<Animator>();
		t = transform;
		c = GetComponent<Collider2D>();
	}

	public override void Tick()
	{

	}

	public Vector2 GetDirectionToPlayer()
	{
		if(noticedPlayer)
		{
			return noticedPlayer.transform.position - t.position;
		}

		return Vector2.zero;
	}

	protected bool IsWallAhead(float distance)
	{
		float spacing = c.bounds.size.y / (wallCheckSteps - 1);
		Vector2 startPos = new Vector2(facingRight ? c.bounds.max.x : c.bounds.min.x, c.bounds.min.y);

		for (int i = 0; i < wallCheckSteps; i++)
		{
			Vector2 origin = startPos + Vector2.up * spacing * i;

			if (i == 0)
			{
				origin.y += 0.05f;
			}

			Debug.DrawRay(origin, (facingRight ? Vector2.right : Vector2.left) * distance, Color.red, 1);

			RaycastHit2D hit = Physics2D.Raycast(origin, facingRight ? Vector2.right : Vector2.left, distance, groundLayer);

			if (hit)
			{
				float angle = Vector2.Angle(Vector2.up, hit.normal);

				if(angle > rbController.MaxSlopeAngle)
				{
					return true;
				}
			}
		}

		return false;
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
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, playerSearchRadius);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, playerChaseRadius);
	}
#endif
}
