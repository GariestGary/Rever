using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Triple Jump")]
public class TripleJump : DefaultAbility
{
	private bool secondJumpPerformed = false;
	private bool thirdJumpPerformed = false;

	public override AbilityType Type => AbilityType.TRIPLE_JUMP;

	public override void AbilityUpdate()
	{
		if (secondJumpPerformed && thirdJumpPerformed)
		{
			secondJumpPerformed = thirdJumpPerformed = !playerController.Collisions.below;
		}
	}

	public override void StartUse()
	{
		if (!enabled) return;

		if (!playerController.Collisions.below && !(playerController.Collisions.left || playerController.Collisions.right) && !playerController.WallSliding)
		{
			if(!secondJumpPerformed)
			{
				secondJumpPerformed = true;
				return;
			}

			if(!thirdJumpPerformed)
			{
				playerController.ForceJump();
				thirdJumpPerformed = true;
			}
			
		}
	}

	public override void StopUse()
	{
		if (!enabled) return;

		if (thirdJumpPerformed && secondJumpPerformed)
		{
			playerController.OnJumpInputUp();
		}

	}
}
