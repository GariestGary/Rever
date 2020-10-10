using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : CharacterMovement
{
	[SerializeField] private float jumpThresholdY;
	[SerializeField] private float wallCheckDistance;

	protected override void CalculateVelocity(Vector2 input)
	{
		base.CalculateVelocity(input);

		CheckwallForJump();
	}

	private void CheckwallForJump()
	{
		
	}
}
