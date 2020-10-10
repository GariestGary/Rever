using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DefaultCharacter: MonoBehaviour, ICharacter, ITick, IFixedTick, IAwake
{
	protected CharacterMovement movement;
	protected Animator anim;

	public bool Process => isProcessing;

	protected bool isProcessing = false;
	protected bool isAlive = true;

	public virtual void OnAwake()
	{
		Toolbox.GetManager<UpdateManager>().Add(this);
		movement = GetComponent<CharacterMovement>();
		anim = GetComponent<Animator>();
	}

	public void Kill()
	{
		isAlive = false;
	}

	public virtual void OnTick()
	{

	}

	public virtual void OnFixedTick()
	{
		
	}

	protected virtual void AnimationStateUpdate()
	{
		
	}

	public void ChangeAnimationState(string stateName)
	{
		if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName)) return;

		anim.Play(stateName);
	}

	protected virtual void OnEnable()
	{
		isProcessing = true;
	}

	protected virtual void OnDisable()
	{
		isProcessing = false;
	}
}
