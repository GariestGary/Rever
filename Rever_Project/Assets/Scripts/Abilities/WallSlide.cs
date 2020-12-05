using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Wall Slide")]
public class WallSlide : ScriptableObject, IAbility
{
	private bool enabled;
	private Player player;
	private Controller2D controller;

	public bool Enabled => enabled;

	public AbilityType Type => AbilityType.WALL_SLIDE;

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
		
	}

	public void Enable()
	{
		controller.CanStickWall = true;
	}

	public void StartUse()
	{
		controller.CanStickWall = false;
	}

	public void StopUse()
	{
		
	}
}
