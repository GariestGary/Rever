using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAbility : ScriptableObject, IAbility
{
	protected bool enabled;
	protected bool init = false;
	public bool Enabled => enabled;

	public virtual AbilityType Type => AbilityType.NONE;

	protected Player player;
	protected Transform playerTransform;
	protected Controller2D playerController;
	protected Animator playerAnimator;

	public virtual void AbilityAwake(Transform character, Animator anim)
	{
		playerTransform = character;
		player = character.GetComponent<Player>();
		playerController = character.GetComponent<Controller2D>();
		playerAnimator = anim;
	}

	public virtual void AbilityFixedUpdate()
	{
		
	}

	public virtual void AbilityUpdate()
	{
		
	}


	public virtual void StartUse()
	{
		
	}

	public virtual void StopUse()
	{
		
	}

	public virtual void Disable()
	{
		if (!enabled && init) return;

		enabled = false;
		init = true;
	}

	public virtual void Enable()
	{
		if (enabled && init) return;

		enabled = true;
		init = true;
	}
}
