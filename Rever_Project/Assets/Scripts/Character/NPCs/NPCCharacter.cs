using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCharacter : DefaultCharacter
{
	public override void OnTick()
	{
		//movement.MovementUpdate(Vector2.zero);
		AnimationStateUpdate();
	}

	public override void OnFixedTick()
	{
		movement.FixedMovementUpdate();
	}

	protected override void AnimationStateUpdate()
	{
		if (movement.Velocity.x == 0 && movement.IsGrounded)
		{
			ChangeAnimationState("Idle");
		}

		if (movement.Velocity.x != 0 && movement.IsGrounded)
		{
			ChangeAnimationState("Run");
		}

		if (movement.IsJumping && !movement.IsGrounded)
		{
			ChangeAnimationState("Jump");
		}

		if (movement.IsFalling && !movement.IsGrounded)
		{
			ChangeAnimationState("Fall");
		}
	}
}
