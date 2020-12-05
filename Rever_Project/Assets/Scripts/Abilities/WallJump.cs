using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Wall Jump")]
public class WallJump : ScriptableObject, IAbility
{
	private bool enabled;
	private Player player;
	private Controller2D controller;

	public bool Enabled => enabled;

	public AbilityType Type => AbilityType.WALL_JUMP;

	public void AbilityAwake(Transform character, Animator anim)
	{
		player = character.GetComponent<Player>();
		controller = character.GetComponent<Controller2D>();
	}

	public void AbilityFixedUpdate()
	{
		
	}

	public void AbilityUpdate()
	{
		
	}

	public void Disable()
	{
		controller.CanJumpWall = false;
	}

	public void Enable()
	{
		controller.CanJumpWall = true;
	}

	public void StartUse()
	{
		
	}

	public void StopUse()
	{
		
	}
}
