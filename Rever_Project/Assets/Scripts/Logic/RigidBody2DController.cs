using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RigidBody2DController : MonoCached
{
	[SerializeField] protected PhysicsMaterial2D fullFriction;
	[SerializeField] protected PhysicsMaterial2D noFriction;
	[Space]
	[SerializeField] private float maxSlopeAngle;

	private const float skinWidth = 0.05f;

    private Rigidbody2D rb;
	private Transform t;
	private BoxCollider2D collider;
	private List<ContactPoint2D> contacts = new List<ContactPoint2D>();

	private CollisionInfo collisionInfo;
	private Vector2 moveVelocity;
	private float prevAngle;
	private bool isFullFriction;

	public Rigidbody2D RB => rb;
	public CollisionInfo Collision => collisionInfo;
	public float MaxSlopeAngle => maxSlopeAngle;

	public override void Rise()
	{
		rb = GetComponent<Rigidbody2D>();
		t = transform;
		collider = GetComponent<BoxCollider2D>();
	}

	public override void Tick()
	{
		CollisionInfoUpdate();

	}

	public override void FixedTick()
	{
		MaxSlopeHandle();
	}

	public void SetFullFriction()
	{
		if (rb == null) return;

		isFullFriction = true;
		rb.sharedMaterial = fullFriction;
	}

	public void SetNoFriction()
	{
		if (rb == null) return;

		isFullFriction = false;
		rb.sharedMaterial = noFriction;
	}

	public void SetVelocity(Vector2 velocity)
	{
		if (rb == null) return;

		rb.velocity = velocity;
	}

	public void Move(int direction, float speed)
	{
		if (direction == 0) return;

		if (direction > 0) direction = 1;
		if (direction < 0) direction = -1;

		if(collisionInfo.slopeAngle < maxSlopeAngle)
		{
			if(collisionInfo.climbingSlope)
			{
				rb.velocity = collisionInfo.directionAlongSlope * direction * speed;
			}
			else
			{
				rb.velocity = Vector2.right * direction * speed;
			}
		}
	}

	private void MaxSlopeHandle()
	{
		if(collisionInfo.climbingSlope)
		{
			if(collisionInfo.slopeAngle >= maxSlopeAngle)
			{
				t.Translate(-collisionInfo.directionAlongSlope * -Physics2D.gravity.y * Time.fixedDeltaTime); 
			}
		}
	}

	private void CollisionInfoUpdate()
	{
		collisionInfo.Reset();
		rb.GetContacts(contacts);

		float angle;

		if(contacts.Count == 0)
		{
			SetNoFriction();
		}

		for (int i = 0; i < contacts.Count; i++)
		{
			//if contact lower
			if (contacts[i].point.y < collider.bounds.min.y + skinWidth)
			{
				angle = Vector2.SignedAngle(Vector2.up, contacts[i].normal);

				if(Mathf.Abs(angle) < maxSlopeAngle)
				{
					collisionInfo.below = true;
				}

				if(angle != 0 && Mathf.Abs(angle) < 90)
				{
					if(angle > 0)
					{
						collisionInfo.directionAlongSlope = -Vector2.Perpendicular(contacts[i].normal);
					}
					else
					{
						collisionInfo.directionAlongSlope = -Vector2.Perpendicular(contacts[i].normal);
					}
					
					collisionInfo.climbingSlope = true;
					collisionInfo.slopeAngle = Mathf.Abs(angle);
				}

				if (rb.velocity != Vector2.zero && isFullFriction)
				{
					SetNoFriction();
				}
				else
				{
					if (angle != prevAngle)
					{
						if (angle != 0)
						{
							SetFullFriction();
						}
						else
						{
							SetNoFriction();
						}
					}
				}

				prevAngle = angle;
			}

			//if contact higher
			if(contacts[i].point.y > collider.bounds.max.y - skinWidth)
			{
				angle = Vector2.SignedAngle(Vector2.down, contacts[i].normal);

				if (Mathf.Abs(angle) < maxSlopeAngle)
				{
					collisionInfo.above = true;
				}
			}

			//if contact on right
			if(contacts[i].point.x > collider.bounds.max.x - skinWidth)
			{
				angle = Vector2.SignedAngle(Vector2.left, contacts[i].normal);

				if (Mathf.Abs(angle) < maxSlopeAngle)
				{
					collisionInfo.right = true;
				}
			}

			//if contact on left
			if (contacts[i].point.x < collider.bounds.min.x + skinWidth)
			{
				angle = Vector2.SignedAngle(Vector2.right, contacts[i].normal);

				if (Mathf.Abs(angle) < maxSlopeAngle)
				{
					collisionInfo.left = true;
				}
			}
		}

		Debug.DrawRay(t.position, collisionInfo.directionAlongSlope, Color.green);
	}

	[System.Serializable]
	public struct CollisionInfo
	{
		public bool below;
		public bool above;
		public bool left;
		public bool right;

		public bool climbingSlope;

		public float slopeAngle;
		public Vector2 directionAlongSlope;

		public void Reset()
		{
			below = false;
			above = false;
			left = false;
			right = false;

			climbingSlope = false;

			slopeAngle = 0;
			directionAlongSlope = Vector2.zero;
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Handles.Label(transform.position, collisionInfo.slopeAngle.ToString());
	}
#endif
}


