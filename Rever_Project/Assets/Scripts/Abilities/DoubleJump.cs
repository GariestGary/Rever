using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Double Jump")]
public class DoubleJump : ScriptableObject, IAbility
{
	private bool enabled;

	private Player player;
	private Controller2D controller;

	private bool secondJumpPerformed = false;

	public bool Enabled => enabled;

	public AbilityType Type => AbilityType.DOUBLE_JUMP;

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
		if(secondJumpPerformed)
		{
			secondJumpPerformed = !controller.Collisions.below;
		}
	}

	public void StartUse()
	{
		if (!enabled) return;

		if (!controller.Collisions.below && !controller.WallSliding && !secondJumpPerformed)
		{
			Debug.Log("DoubleJump");
			controller.ForceJump();
			secondJumpPerformed = true;
		}
	}

	public void StopUse()
	{
		if (!enabled) return;

		if (secondJumpPerformed)
		{
			Debug.Log("DoubleJump Stop");
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
