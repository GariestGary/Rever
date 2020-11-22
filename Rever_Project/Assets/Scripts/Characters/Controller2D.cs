using UnityEngine;
using System.Collections;
using System;

public class Controller2D : RaycastController
{
	[SerializeField] private float maxJumpHeight = 4;
	[SerializeField] private float minJumpHeight = 1;
	[SerializeField] private float timeToJumpApex = .4f;
	[SerializeField] private float fallMultiplier = .2f;
	[Space]
	[SerializeField] private bool canStickWall;
	[SerializeField] private Vector2 wallJumpClimb;
	[SerializeField] private Vector2 wallJumpOff;
	[SerializeField] private Vector2 wallLeap;
	[Space]
	[SerializeField] private float wallSlideSpeedMax = 3;
	[SerializeField] private float wallStickTime = .25f;
	[Space]
	[SerializeField] private float accelerationTimeAirborne = .2f;
	[SerializeField] private float accelerationTimeGrounded = .1f;
	[SerializeField] private float moveSpeed = 6;
	[SerializeField] private float maxSlopeAngle = 80;
	[SerializeField] private float externalForcesDamp = 5;

	public Vector3 Velocity => velocity;
	public bool WallSliding => wallSliding;
	public int WallDirectionX => wallDirX;
	public float LastInputFacing => lastInputFacing;
	public bool CanStickWall => canStickWall;
	public CollisionInfo Collisions => collisions;
	public event Action OnJump = delegate { };
	
	public bool currentAbilityToStickWall = true;
	public bool useInput = true;

	private float gravity;
	private float maxJumpVelocity;
	private float minJumpVelocity;
	private Vector3 velocity;
	private Vector3 externalVelocity;
	private Vector2 externalVelocitySmoothing;
	private float velocityXSmoothing;
	private float timeToWallUnstick;
	private float lastInputFacing;

	private bool wallSliding;
	private int wallDirX;

	private Vector2 directionalInput;
	private CollisionInfo collisions;

	private Transform t;


	public override void OnAwake()
	{
		base.OnAwake();

		t = transform;

		ResetGravity();
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

		collisions.faceDir = 1;

		currentAbilityToStickWall = CanStickWall;
	}

	public void HandleInput(Vector2 input)
	{
		if (useInput)
		{
			if(input != Vector2.zero)
			{
				lastInputFacing = input.x;
			}

			directionalInput = input;
		}
		else
		{
			directionalInput = Vector2.zero;
		}
	}

	public void Move()
	{
		CalculateVelocity();
		HandleWallSliding();

		MoveUpdate(velocity * Time.fixedDeltaTime);

		if (collisions.above || collisions.below)
		{
			if (collisions.slidingDownMaxSlope)
			{
				velocity.y += collisions.slopeNormal.y * -gravity * Time.fixedDeltaTime;
			}
			else
			{
				velocity.y = 0;
			}
		}
	}

	private void MoveUpdate(Vector2 moveAmount, bool standingOnPlatform = false)
	{
		UpdateRaycastOrigins();

		collisions.Reset();
		collisions.moveAmountOld = moveAmount;

		if (moveAmount.y < 0)
		{
			DescendSlope(ref moveAmount);
		}

		if (moveAmount.x != 0)
		{
			collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}

		HorizontalCollisions(ref moveAmount);

		if(moveAmount.y != 0)
		{
			VerticalCollisions(ref moveAmount);
		}

		t.Translate(moveAmount);

		if (standingOnPlatform)
		{
			collisions.below = true;
		}
	}

	public void AddForce(Vector3 amount)
	{
		velocity += amount;
	}

	public void SetForce(Vector3 amount)
	{
		velocity = amount;
	}

	public void SetGravity(float amount)
	{
		gravity = amount;
	}

	public void ResetGravity()
	{
		gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
	}

	private void HorizontalCollisions(ref Vector2 moveAmount) 
	{
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth) 
		{
			rayLength = 2*skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i ++) 
		{
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX,Color.red);

			if (hit) 
			{

				if (hit.distance == 0) 
				{
					continue;
				}

				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxSlopeAngle) 
				{
					if (collisions.descendingSlope) 
					{
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) 
					{
						distanceToSlopeStart = hit.distance-skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle) 
				{
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) 
					{
						moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}

	private void VerticalCollisions(ref Vector2 moveAmount) 
	{
		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i ++) 
		{
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY,Color.red);

			if (hit) 
			{
				if (hit.collider.CompareTag("Through")) {
					if (directionY == 1 || hit.distance == 0) 
					{
						continue;
					}
					if (collisions.fallingThroughPlatform) 
					{
						continue;
					}
					if (directionalInput.y == -1) 
					{
						collisions.fallingThroughPlatform = true;
						Invoke("ResetFallingThroughPlatform",.5f);
						continue;
					}
				}

				moveAmount.y = (hit.distance - skinWidth) * directionY;
				
				rayLength = hit.distance;

				if (collisions.climbingSlope) 
				{
					moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}

		if (collisions.climbingSlope) 
		{
			float directionX = Mathf.Sign(moveAmount.x);
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * directionX,rayLength,collisionMask);

			if (hit) 
			{
				float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
				if (slopeAngle != collisions.slopeAngle) 
				{
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
					collisions.slopeNormal = hit.normal;
				}
			}
		}
	}

	private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal) 
	{
		float moveDistance = Mathf.Abs (moveAmount.x);
		float climbmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbmoveAmountY) 
		{
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
			collisions.slopeNormal = slopeNormal;
		}
	}

	private void DescendSlope(ref Vector2 moveAmount) 
	{
		RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast (raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, collisionMask);
		RaycastHit2D maxSlopeHitRight = Physics2D.Raycast (raycastOrigins.bottomRight, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, collisionMask);
		if (maxSlopeHitLeft ^ maxSlopeHitRight) 
		{
			SlideDownMaxSlope (maxSlopeHitLeft, ref moveAmount);
			SlideDownMaxSlope (maxSlopeHitRight, ref moveAmount);
		}

		if (!collisions.slidingDownMaxSlope) 
		{
			float directionX = Mathf.Sign (moveAmount.x);
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

			if (hit) 
			{
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle) 
				{
					if (Mathf.Sign (hit.normal.x) == directionX) 
					{
						if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (moveAmount.x)) 
						{
							float moveDistance = Mathf.Abs (moveAmount.x);
							float descendmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
							moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
							moveAmount.y -= descendmoveAmountY;

							collisions.slopeAngle = slopeAngle;
							collisions.descendingSlope = true;
							collisions.below = true;
							collisions.slopeNormal = hit.normal;
						}
					}
				}
			}
		}
	}

	private void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount) 
	{

		if (hit) 
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle > maxSlopeAngle) 
			{
				moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs (moveAmount.y) - hit.distance) / Mathf.Tan (slopeAngle * Mathf.Deg2Rad);

				collisions.slopeAngle = slopeAngle;
				collisions.slidingDownMaxSlope = true;
				collisions.slopeNormal = hit.normal;
			}
		}

	}

	public void OnJumpInputDown()
	{
		if (wallSliding && canStickWall)
		{
			if (wallDirX == directionalInput.x)
			{
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			}
			else if (directionalInput.x == 0)
			{
				velocity.x = -wallDirX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
			}
			else
			{
				velocity.x = -wallDirX * wallLeap.x;
				velocity.y = wallLeap.y;
			}

			OnJump.Invoke();
		}
		else if (collisions.below)
		{
			if (collisions.slidingDownMaxSlope)
			{
				if (directionalInput.x != -Mathf.Sign(collisions.slopeNormal.x))
				{ // not jumping against max slope
					velocity.y = maxJumpVelocity * collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * collisions.slopeNormal.x;

					OnJump.Invoke();
				}
			}
			else
			{
				velocity.y = maxJumpVelocity;
				OnJump.Invoke();
			}
		}
	}

	public void OnJumpInputUp()
	{
		if (velocity.y > minJumpVelocity)
		{
			velocity.y = minJumpVelocity;
		}
	}

	private void HandleWallSliding()
	{
		if (!currentAbilityToStickWall) return;

		wallDirX = (collisions.left) ? -1 : 1;
		wallSliding = false;
		if ((collisions.left || collisions.right) && !collisions.below && velocity.y < 0)
		{
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax)
			{
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0)
			{
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInput.x != wallDirX && directionalInput.x != 0)
				{
					timeToWallUnstick -= Time.fixedDeltaTime;
				}
				else
				{
					timeToWallUnstick = wallStickTime;
				}
			}
			else
			{
				timeToWallUnstick = wallStickTime;
			}

		}

	}

	private void CalculateVelocity()
	{
		float targetVelocityX = directionalInput.x * moveSpeed;

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

		velocity.y += (velocity.y < 0 ? fallMultiplier : 1) * gravity * Time.fixedDeltaTime;
	}

	private void ResetFallingThroughPlatform() 
	{
		collisions.fallingThroughPlatform = false;
	}

	[System.Serializable]
	public struct CollisionInfo 
	{
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public bool slidingDownMaxSlope;

		public float slopeAngle, slopeAngleOld;
		public Vector2 slopeNormal;
		public Vector2 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		public bool Colliding => above || below || left || right;

		public void Reset() 
		{
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			slidingDownMaxSlope = false;
			slopeNormal = Vector2.zero;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}
