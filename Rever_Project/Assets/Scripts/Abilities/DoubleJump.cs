using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Double Jump")]
public class DoubleJump : DefaultAbility
{
	private bool secondJumpPerformed = false;

	public override AbilityType Type => AbilityType.DOUBLE_JUMP;

	public override void AbilityUpdate()
	{
		if(secondJumpPerformed)
		{
			secondJumpPerformed = !playerController.Collisions.below;
		}
	}

	public override void StartUse()
	{
		if (!enabled) return;

		if (!playerController.Collisions.below && !(playerController.Collisions.left || playerController.Collisions.right) && !playerController.WallSliding && !secondJumpPerformed)
		{
			playerController.ForceJump();
			secondJumpPerformed = true;
		}
	}

	public override void StopUse()
	{
		if (!enabled) return;

		if (secondJumpPerformed)
		{
			playerController.OnJumpInputUp();
		}

	}
}
