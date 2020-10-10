using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CharacterMovement : MonoBehaviour, IAwake
{
	[SerializeField] protected float movementSpeed = 8;
	[SerializeField] protected float jumpForce = 10;
	[SerializeField] protected float fallMultiplier = 1.2f;
	[SerializeField] protected float forgiveTime = 0.1f;
	[SerializeField] protected float maxAbsVelocityY = 150;
	[SerializeField] protected float maxGroundAngle = 60;
	[Space]
	[SerializeField] protected Transform groundCheck;
	[SerializeField] protected float groundCheckRadius = 0.5f;
	[SerializeField] protected float slopeCheckDistance = 0.05f;
	[SerializeField] protected LayerMask groundLayer;
	[Space]
	[SerializeField] protected PhysicsMaterial2D fullFrictionMaterial;
	[SerializeField] protected PhysicsMaterial2D noFrictionMaterial;
 
	protected Transform t;
	protected Rigidbody2D rb;
	protected CapsuleCollider2D capsule;

	protected float currentForgiveTime;
	protected bool grounded;
	protected bool jumping;
	protected bool falling;
	protected bool canJump;
	protected bool facingRight;
	protected bool canWalkSlope;
	protected float slopeDownAngle;
	protected float slopeSideAngle;
	protected Vector2 currentInput;
	protected Vector2 newVelocity;
	protected Vector2 newForce;
	protected Vector2 slopeNormalPerpendicular;

	public Vector2 Velocity => rb.velocity;
	public bool IsFacingRight => facingRight;
	public bool IsJumping => jumping;
	public bool IsFalling => falling;
	public bool IsGrounded => grounded;

	public virtual void OnAwake()
	{
		Toolbox.GetManager<UpdateManager>().Add(this);

		t = transform;
		rb = GetComponent<Rigidbody2D>();
		capsule = GetComponent<CapsuleCollider2D>();
	}

	public virtual void FixedMovementUpdate()
	{
		ApplyMovement();
	}
	
	public virtual void MovementUpdate(Vector2 input)
	{
		currentInput = input;

		CheckInput();
		CheckGround();
		SlopeCheck();


		//CalculateVelocity(input);
	}

	private void CheckGround()
	{
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

		if (rb.velocity.y <= 0.0f)
		{
			jumping = false;
		}

		if (grounded && !jumping && slopeDownAngle <= maxGroundAngle)
		{
			canJump = true;
		}

	}

	private void SlopeCheck()
	{
		Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, capsule.bounds.extents.y));

		SlopeCheckHorizontal(checkPos);
		SlopeCheckVertical(checkPos);
	}

	private void SlopeCheckHorizontal(Vector2 checkPos)
	{
		RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundLayer);
		RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundLayer);

		if (slopeHitFront)
		{
			slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
		}
		else if (slopeHitBack)
		{
			slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
		}
		else
		{
			slopeSideAngle = 0.0f;
		}

	}

	private void SlopeCheckVertical(Vector2 checkPos)
	{
		RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

		if (hit)
		{

			slopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;

			slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

			Debug.DrawRay(hit.point, slopeNormalPerpendicular, Color.blue);
			Debug.DrawRay(hit.point, hit.normal, Color.green);

		}

		if (slopeDownAngle > maxGroundAngle || slopeSideAngle > maxGroundAngle)
		{
			canWalkSlope = false;
		}
		else
		{
			canWalkSlope = true;
		}

		if (canWalkSlope && currentInput.x == 0.0f)
		{
			rb.sharedMaterial = fullFrictionMaterial;
		}
		else
		{
			rb.sharedMaterial = noFrictionMaterial;
		}
	}

	private void CheckInput()
	{
		if (currentInput.x == 1 && facingRight == false)
		{
			Turn(true);
		}
		else if (currentInput.x == -1 && facingRight == true)
		{
			Turn(false);
		}
	}

	private void ApplyMovement()
	{
		if (grounded && canWalkSlope && !jumping && currentInput != Vector2.zero) //If on slope
		{
			newVelocity.Set(movementSpeed * slopeNormalPerpendicular.x * -currentInput.x, movementSpeed * slopeNormalPerpendicular.y * -currentInput.x);
			rb.velocity = newVelocity;
		}
		else if (!grounded) //If in air
		{
			newVelocity.Set(movementSpeed * currentInput.x, rb.velocity.y);
			rb.velocity = newVelocity;
		}

	}

	public virtual void Turn(bool isRight)
	{
		if(isRight)
		{
			t.eulerAngles = new Vector3(0, 0, 0);
		}
		else
		{
			t.eulerAngles = new Vector3(0, 180, 0);
		}
	}

	public virtual void JumpStart()
	{
		if (canJump)
		{
			canJump = false;
			jumping = true;
			newVelocity.Set(0.0f, 0.0f);
			rb.velocity = newVelocity;
			newForce.Set(0.0f, jumpForce);
			rb.AddForce(newForce, ForceMode2D.Impulse);
		}
	}

	public virtual void JumpEnd()
	{
		if(jumping)
		{
			//jumping = false;
			//newVelocity.Set(0.0f, 0.0f);
			//rb.velocity = newVelocity;
		}
	}

	protected virtual void CalculateVelocity(Vector2 input)
	{
		if(grounded)
		{
			rb.velocity.Set(movementSpeed * -input.x * slopeNormalPerpendicular.x, movementSpeed * -input.x * slopeNormalPerpendicular.y);
			jumping = false;
			falling = false;
		}
		else
		{
			if(rb.velocity.y < 0)
			{
				falling = true;
				jumping = false;
				rb.velocity.Set(rb.velocity.x, rb.velocity.y * -fallMultiplier);
			}
			else
			{
				rb.velocity.Set(movementSpeed * input.x, rb.velocity.y);
			}

		}
	}

}
