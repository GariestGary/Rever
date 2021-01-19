using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemyTasks : EnemyBase
{
	[SerializeField] protected LayerMask groundLayer;
	[SerializeField] protected Transform edgeCheckTransform;
	[SerializeField] protected float edgeCheckDistance;
	[SerializeField] protected List<Transform> wallCheckTransforms;
	[SerializeField] protected float wallCheckDistance;
	[Space]
	[SerializeField] private bool canJump;
	[SerializeField] protected float jumpVelocity;
	[Space]
	[SerializeField] protected float walkSpeed;
	[SerializeField] protected float chasingSpeed;

	protected void IsGoingIntoWall()
	{
		for (int i = 0; i < wallCheckTransforms.Count; i++)
		{

		}
	}
}
