using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Triple Jump")]
public class TripleJump : ScriptableObject, IAbility
{
	private bool enabled;

	private Player player;
	private Controller2D controller;

	private bool secondJumpPerformed = false;
	private bool thirdJumpPerformed = false;

	public bool Enabled => enabled;

	public AbilityType Type => AbilityType.TRIPLE_JUMP;

	public void AbilityAwake(Transform character, Animator anim)
	{
		controller = character.GetComponent<Controller2D>();
		player = character.GetComponent<Player>();
	}

	public void AbilityFixedUpdate()
	{

	}

	public void AbilityUpdate()
	{
		if (secondJumpPerformed && thirdJumpPerformed)
		{
			secondJumpPerformed = thirdJumpPerformed = !controller.Collisions.below;
		}
	}

	public void StartUse()
	{
		if (!enabled) return;

		if (!controller.Collisions.below && !(controller.Collisions.left || controller.Collisions.right) && !controller.WallSliding)
		{
			if(!secondJumpPerformed)
			{
				secondJumpPerformed = true;
				return;
			}

			if(!thirdJumpPerformed)
			{
				controller.ForceJump();
				thirdJumpPerformed = true;
			}
			
		}
	}

	public void StopUse()
	{
		if (!enabled) return;

		if (thirdJumpPerformed && secondJumpPerformed)
		{
			controller.OnJumpInputUp();
		}

	}

	public void Disable()
	{
		enabled = false;
	}

	public void Enable()
	{
		enabled = true;
	}
}
