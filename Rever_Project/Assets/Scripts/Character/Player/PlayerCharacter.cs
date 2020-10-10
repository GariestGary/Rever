using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerCharacter : DefaultCharacter
{
	private InputManager input;
	private bool inputSubscribed;

	[Inject]
	public virtual void Constructor(InputManager input)
	{
		this.input = input;
	}

	public override void OnAwake()
	{
		base.OnAwake();

		if (input && movement && !inputSubscribed)
		{
			input.JumpStart += movement.JumpStart;
			input.JumpEnd += movement.JumpEnd;

			inputSubscribed = true;
		}
	}

	public override void OnTick()
	{
		base.OnTick();

		movement.MovementUpdate(input.MoveInput);
		AnimationStateUpdate();
	}

	public override void OnFixedTick()
	{
		base.OnFixedTick();

		movement.FixedMovementUpdate();
	}

	protected override void AnimationStateUpdate()
	{
		if(movement.Velocity.x == 0 && movement.IsGrounded)
		{
			ChangeAnimationState("Idle");
		}

		if(movement.Velocity.x != 0 && movement.IsGrounded)
		{
			ChangeAnimationState("Run");
		}

		if(movement.IsJumping && !movement.IsGrounded)
		{
			ChangeAnimationState("Jump");
		}

		if(movement.IsFalling && !movement.IsGrounded)
		{
			ChangeAnimationState("Fall");
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if (input && movement && !inputSubscribed)
		{
			input.JumpStart += movement.JumpStart;
			input.JumpEnd += movement.JumpEnd;

			inputSubscribed = true;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (input && movement && inputSubscribed)
		{
			input.JumpStart -= movement.JumpStart;
			input.JumpEnd -= movement.JumpEnd;

			inputSubscribed = false;
		}
	}

}
