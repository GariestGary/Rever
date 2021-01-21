using UnityEngine;
using System.Collections;
using System;

public class Controller2D : RaycastController
{
	[SerializeField] protected float maxJumpHeight = 4;
	[SerializeField] protected float minJumpHeight = 1;
	[SerializeField] protected float timeToJumpApex = .4f;
	[SerializeField] protected float fallMultiplier = .2f;
	[Space]
	[SerializeField] protected bool canJumpWall;
	[SerializeField] protected Vector2 wallJumpClimb;
	[SerializeField] protected Vector2 wallJumpOff;
	[SerializeField] protected Vector2 wallLeap;
	[Space]
	[SerializeField] protected float wallSlideSpeedMax = 3;
	[SerializeField] protected float wallStickTime = .25f;
	[Space]
	[SerializeField] protected float accelerationTimeAirborne = .2f;
	[SerializeField] protected float accelerationTimeGrounded = .1f;
	[SerializeField] protected float moveSpeed = 6;
	[SerializeField] protected float maxSlopeAngle = 80;

	public Vector3 Velocity => velocity;
	public bool WallSliding => wallSliding;
	public int WallDirectionX => wallDirX;
	public Vector2 LastInputFacing => lastInputFacing;
	public bool CanJumpWall { get { return canJumpWall; } set { canJumpWall = value; } }
	public CollisionInfo Collisions => collisions;

	public event Action OnJump = delegate { };
	
	public bool useInput { get; set; } = true;
	public bool UseHorizontalCollisions { get; set; } = true;
	public bool UseVerticalCollisions { get; set; } = true;

	protected Vector3 velocity;
	protected Vector2 lastInputFacing;
	protected float gravity;
	protected float maxJumpVelocity;
	protected float minJumpVelocity;
	protected float velocityXSmoothing;
	protected float timeToWallUnstick;

	protected bool wallSliding;
	protected int wallDirX;

	protected Vector2 directionalInput;
	protected CollisionInfo collisions;

	protected Transform t;


	public override void Rise()
	{
		base.Rise();

		t = transform;

		ResetGravity();
		CalculateJumpVelocities();

		collisions.FaceDir = 1;
		lastInputFacing.x = collisions.FaceDir;
	}

	public void ChangeFacingDirection(int direction)
	{
		collisions.FaceDir = direction;
		lastInputFacing.x = direction;
	}

	public void HandleInput(Vector2 input)
	{
		if (useInput)
		{
			if(input != Vector2.zero)
			{
				lastInputFacing = input;
			}

			directionalInput = input;
		}
		else
		{
			directionalInput = Vector2.zero;
		}
	}

	public void FixedInputUpdating()
	{
		CalculateVelocity();
		HandleWallSliding();

		Move(velocity * Time.fixedDeltaTime);

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

	public void Move(Vector2 moveAmount, bool standingOnPlatform = false)
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
			collisions.FaceDir = (int)Mathf.Sign(moveAmount.x);
		}

		HorizontalCollisions(ref moveAmount);
		if (moveAmount.y != 0)
		{
			VerticalCollisions(ref moveAmount);
		}

		t.Translate(moveAmount);

		if (standingOnPlatform)
		{
			collisions.below = true;
		}
	}

	public void SetMoveSpeed(float speed)
	{
		moveSpeed = speed;
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

	public void CalculateJumpVelocities()
	{
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}

	protected void HorizontalCollisions(ref Vector2 moveAmount) 
	{
		if (!UseHorizontalCollisions) return;

		float directionX = collisions.FaceDir;
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

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
		}
	}

	protected void VerticalCollisions(ref Vector2 moveAmount) 
	{
		if (!UseVerticalCollisions) return;

		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i ++) 
		{
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

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

			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
		}

		if (collisions.climbingSlope)
		{
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((collisions.FaceDir == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * collisions.FaceDir, rayLength, collisionMask);

			if (hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle)
				{
					moveAmount.x = (hit.distance - skinWidth) * collisions.FaceDir;
					collisions.slopeAngle = slopeAngle;
					collisions.slopeNormal = hit.normal;
				}
			}
		}
	}

	protected void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal) 
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

	protected void DescendSlope(ref Vector2 moveAmount) 
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

	protected void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount) 
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
		if (wallSliding && canJumpWall)
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
				ForceJump();
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

	public void ForceJump()
	{
		velocity.y = maxJumpVelocity;
		OnJump.Invoke();
	}

	protected void HandleWallSliding()
	{
		if (!canJumpWall) return;

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

	protected void CalculateVelocity()
	{
		float targetVelocityX = directionalInput.x * moveSpeed;

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

		velocity.y += (velocity.y < 0 ? fallMultiplier : 1) * gravity * Time.fixedDeltaTime;
	}

	protected void ResetFallingThroughPlatform() 
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

		private int faceDir;

		public int FaceDir 
		{
			get { return faceDir; }
			set 
			{
				if (value >= 0)
				{
					faceDir = 1;
				}
				else
				{
					faceDir = -1;
				}
			}
		}

		public bool facingRight => FaceDir >= 0 ? true : false;
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
