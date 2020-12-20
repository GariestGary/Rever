using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Wall Jump")]
public class WallJump : DefaultAbility
{
	public override AbilityType Type => AbilityType.WALL_JUMP;

	public override void Disable()
	{
		base.Disable();
		playerController.CanJumpWall = false;
	}

	public override void Enable()
	{
		base.Enable();
		playerController.CanJumpWall = true;
	}
}
