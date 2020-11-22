﻿using UnityEngine;
using System.Collections;
using Zenject;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.U2D.IK;
using UnityEngine.U2D.Animation;
using UniRx;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour, ITick, IFixedTick, IAwake
{
	[SerializeField] private Animator anim;
	[SerializeField] private SpriteSkin skin;
	[SerializeField] private Transform graphicsRoot;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private BoxCollider2D colliderBox;
	[Space]
	[SerializeField] private LayerMask interactableLayer;
	[SerializeField] private float interactRadius;
	[Space]
	[SerializeField] private List<ScriptableObject> abilities = new List<ScriptableObject>();
	public bool Process { get; private set; }

	private Controller2D controller;
	private InputManager input;
	private GameManager game;
	private MessageManager msg;

	private IUseable currentAbility;
	private int currentAbilityIndex;
	private Transform t;
	private Camera mainCam;
	private float currentGroundAngle;
	private IInteractable currentInteractable;
	

	private bool subscribed = false;
	private bool facingRight = true;

	private Action startUse;
	private Action stopUse;
	private Action onJump;
	private Action tryInteract;

	[Inject]
	public void Constructor(InputManager input, GameManager game, MessageManager msg)
	{
		this.input = input;
		this.game = game;
		this.msg = msg;
	}

	public void OnAwake() 
	{
		controller = GetComponent<Controller2D>();
		t = transform;
		mainCam = Camera.main;

		abilities.ForEach(a => { if (a is IUseable) (a as IUseable).AbilityAwake(t, anim); });

		currentAbilityIndex = 0;
		SetAbility(currentAbilityIndex);

		tryInteract = delegate 
		{
			if(currentInteractable != null)
			{
				currentInteractable.Interact();
				input.SetDefaultInputActive(false);
			}
		};

		startUse = delegate { currentAbility.StartUse(mainCam.ScreenToWorldPoint(input.PointerPosition)); };
		stopUse = delegate { currentAbility.StopUse(mainCam.ScreenToWorldPoint(input.PointerPosition)); };
		onJump = delegate { anim.SetTrigger("Jump"); };

		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.DIALOG_CLOSED).Subscribe(_ => 
		{
			input.SetDefaultInputActive(true);
		}).AddTo(Toolbox.Instance.Disposables);

		SetSubscribe(true);

	}
	public void OnTick() 
	{
		controller.HandleInput(input.MoveInput);

		AnimationUpdate();
		TurnHandle(input.MoveInput.x);
		InteractHandle();
	}

	private void SetAbility(int index)
	{
		if (index < 0 || index >= abilities.Count) return;

		if (currentAbility != null) StopCoroutine(currentAbility.AbilityUpdate());

		currentAbilityIndex = index;

		currentAbility = abilities[currentAbilityIndex] as IUseable;

		Debug.Log("Set ability " + index);

		StartCoroutine(currentAbility.AbilityUpdate());
	}

	private void TurnHandle(float xInput)
	{
		if (xInput == 0) return;

		if (xInput > 0 && !facingRight)
		{
			//TODO: make animation turn
			facingRight = true;
		}

		if (xInput < 0 && facingRight)
		{
			facingRight = false;
		}
	}

	private void InteractHandle()
	{
		if (controller.Velocity == Vector3.zero) return;

		Collider2D interactableCollider = Physics2D.OverlapCircle(t.position, interactRadius, interactableLayer);

		if (interactableCollider == null)
		{
			if (currentInteractable != null)
			{
				currentInteractable.Exited();
				currentInteractable = null;
			}

			return;
		}

		IInteractable interactable = interactableCollider.GetComponent<IInteractable>();

		if(interactable != null)
		{
			if(currentInteractable != null && interactable != currentInteractable)
			{
				currentInteractable.Exited();
				currentInteractable = null;
			}

			currentInteractable = interactable;

			currentInteractable.Entered();
		}
	}

	public void OnFixedTick()
	{
		controller.Move();
	}

	private void AnimationUpdate()
	{
		anim.SetFloat("Velocity X", controller.Velocity.x);
		anim.SetFloat("Velocity Y", controller.Velocity.y);
		anim.SetBool("Grounded", controller.Collisions.above || controller.Collisions.below);
		anim.SetBool("Sliding Wall", controller.WallSliding);
		anim.SetFloat("Wall Sliding Input X", input.MoveInput.x * -controller.WallDirectionX);
	}

	private void SetSubscribe(bool state)
	{
		if(state)
		{
			if(!subscribed)
			{
				//Put all subs here

				if(input && controller && anim)
				{
					input.Interact += tryInteract;
					input.OnClickDown += startUse;
					input.OnClickUp += stopUse;
					input.JumpStart += controller.OnJumpInputDown;
					input.JumpEnd += controller.OnJumpInputUp;
					controller.OnJump += onJump;

					subscribed = true;
				}
			}
		}
		else
		{
			if (subscribed)
			{
				//Put all unsubs here

				if (input && controller && anim)
				{
					input.Interact -= tryInteract;
					input.OnClickDown -= startUse;
					input.OnClickUp -= stopUse;
					input.JumpStart -= controller.OnJumpInputDown;
					input.JumpEnd -= controller.OnJumpInputUp;
					controller.OnJump -= onJump;

					subscribed = false;
				}
			}
		}
	}

	private void OnEnable()
	{
		Process = true;

		SetSubscribe(true);
	}

	private void OnDisable()
	{
		Process = false;

		SetSubscribe(false);
	}

}