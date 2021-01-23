using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class EnemyBase : MonoCached
{
	[SerializeField] protected Transform graphicsRoot;
	[SerializeField] protected Animator anim;
	[Space]
	[SerializeField] protected int groundCheckSteps;
	[SerializeField] protected float playerSearchRadius;
	[SerializeField] protected float playerChaseRadius;
	[SerializeField] protected float playerAttackRadius;
	[Space]
	[SerializeField] private int wallCheckSteps;
	[Space]
	[SerializeField] protected LayerMask playerLayer;
	[SerializeField] protected LayerMask groundLayer;
	[Space]
	[SerializeField] protected int damage;
	[Space]
	[SerializeField] private bool canJump;
	[SerializeField] protected Vector2 jumpVelocity;
	[SerializeField] protected float maxGroundHeight;
	[Range(0.01f, 1)]
	[SerializeField] protected float timeStep;
	[SerializeField] protected float maxTime;

	public UnityEvent PlayerNoticedEvent;
	public UnityEvent PlayerLostEvent;

	protected Collider2D c;
	protected RigidBody2DController controller;
	protected Health health;
	protected Player noticedPlayer;
	protected Transform t;
	protected float slopeAngle;

	private int facingDirection = 1;

	public int FacingDirection 
	{ 
		get 
		{ 
			return facingDirection; 
		} 
		protected set 
		{
			if(value >= 0)
			{
				facingDirection = 1;
			}
			else
			{
				facingDirection = -1;
			}
		} 
	}

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
	public bool IsPlayerInAttackRadius => CheckPlayerInRadius(playerAttackRadius);

	[Inject]
	public void Construct(GameManager game)
	{
		this.game = game;
	}

	public override void Rise()
	{
		base.Rise();

		t = transform;
		controller = GetComponent<RigidBody2DController>();
		t = transform;
		c = GetComponent<Collider2D>();
	}

	public override void Tick()
	{

	}

	protected void TurnHandle()
	{
		if(FacingDirection == 1  && graphicsRoot.localEulerAngles.y != 0)
		{
			graphicsRoot.localEulerAngles = new Vector3(0, 0, 0);
		}
		else if(FacingDirection == -1 && graphicsRoot.localEulerAngles.y != 180)
		{
			graphicsRoot.localEulerAngles = new Vector3(0, 180, 0);
		}
	}

	protected void Jump(int dir, Vector2 vel)
	{
		if (!canJump) return;

		if (dir >= 0) dir = 1; else dir = -1;

		controller.SetVelocity(new Vector2(vel.x * dir, vel.y));
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
		Vector2 startPos = new Vector2(FacingDirection == 1 ? c.bounds.max.x : c.bounds.min.x, c.bounds.min.y);

		for (int i = 0; i < wallCheckSteps; i++)
		{
			Vector2 origin = startPos + Vector2.up * spacing * i;

			if (i == 0)
			{
				origin.y += 0.05f;
			}

			Debug.DrawRay(origin, (Vector2.right * FacingDirection) * distance, Color.red, 0.1f);

			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * FacingDirection, distance, groundLayer);

			if (hit)
			{
				float angle = Vector2.Angle(Vector2.up, hit.normal);

				if(angle > controller.MaxSlopeAngle)
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

	protected bool IsJumpDestinationAvailable(int dir, Vector2 jumpVelocity)
	{
		if (dir >= 0) dir = 1; else dir = -1;

		Vector3 prev = t.position;
		Vector3 vel = new Vector3(jumpVelocity.x * dir, jumpVelocity.y, 0);

		if(timeStep == 0)
		{
			timeStep = 0.01f;
		}

		for (int i = 1; ; i++)
		{
			float time = timeStep * i;

			if (time > maxTime)
			{
				return false;
			}

			Vector3 pos = PlotTrajectoryAtTime(t.position, vel, time);

			RaycastHit2D hit;

			hit = Physics2D.Linecast(prev, pos, groundLayer);

			Debug.DrawLine(prev, pos, Color.red, 1);

			if (hit)
			{
				if (hit.transform.CompareTag("Platform"))
				{
					return false;
				}

				Debug.DrawRay(hit.point, hit.normal, Color.green, 1);

				float angle = Vector2.Angle(Vector2.up, hit.normal);

				if (angle == 0 && (controller.Collision.climbingSlope || Mathf.Abs(t.position.y - hit.point.y) < maxGroundHeight))
				{
					return true;
				}

				if (angle < controller.MaxSlopeAngle)
				{
					return true;
				}

				return false;
			}

			prev = pos;
		}
	}

	protected Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
	{
		return start + startVelocity * time + Physics.gravity * time * time * (1 - controller.RB.drag);
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, playerSearchRadius);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, playerChaseRadius);

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, playerAttackRadius);
	}
#endif
}
