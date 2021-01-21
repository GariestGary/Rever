using UnityEngine;
using System.Collections;
using Zenject;
using System.Collections.Generic;
using System;
using System.Linq;
using UniRx;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoCached
{
	[SerializeField] private Animator anim;
	[SerializeField] private Transform graphicsRoot;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private BoxCollider2D colliderBox;
	[Space]
	[SerializeField] private float invulnerabilityTime;
	[Space]
	[SerializeField] private LayerMask interactableLayer;
	[SerializeField] private float interactRadius;
	[Space]
	[SerializeField] private bool enableAbilitiesAtStart;
	[Space]
	[SerializeField] private List<ScriptableObject> abilities = new List<ScriptableObject>();
	public Health PlayerHealth { get; private set; }
	public bool IsUpdatingTurn { get; set; }
	public bool IsInvulnerable => currentInvulnerabilityTime > 0;
	public Vector2 position => new Vector2(t.position.x, t.position.y);

	private Controller2D controller;
	private InputManager input;
	private GameManager game;
	private MessageManager msg;

	private Dictionary<AbilityType, IAbility> abilitiesDictionary = new Dictionary<AbilityType, IAbility>();
	private Transform t;
	private IInteractable currentInteractable;

	private bool subscribed = false;
	private bool facingRight = true;

	private float currentInvulnerabilityTime = 0;

	private Action onJump;
	private Action onHealthChange;

	[Inject]
	public void Constructor(InputManager input, GameManager game, MessageManager msg)
	{
		this.input = input;
		this.game = game;
		this.msg = msg;
	}

	public override void Rise() 
	{
		InitializeFields();
		InitializeAbilities();
		InitializeDelegates();
		InitializeSubscribes();

		PlayerHealth.Initialize();
	}
	public override void Tick() 
	{
		controller.HandleInput(input.MoveInput);
		
		AnimationUpdate();
		TurnHandle(input.MoveInput.x);
		InteractHandle();
		InvulnerabilityUpdate();

		for (int i = 0; i < abilities.Count; i++)
		{
			(abilities[i] as IAbility).AbilityUpdate();
		}
	}
	public override void FixedTick()
	{
		controller.FixedInputUpdating();

		for (int i = 0; i < abilities.Count; i++)
		{
			(abilities[i] as IAbility).AbilityFixedUpdate();
		}
	}

	private void InitializeSubscribes()
	{
		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.DIALOG_CLOSED).Subscribe(_ =>
		{
			input.TrySetDefaultInputActive(true, true);
		}).AddTo(Toolbox.Instance.Disposables);

		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.HIT_CHARACTER && x.tag == "player").Subscribe(x => 
		{
			PlayerHealth.Hit((int)x.data);
		}).AddTo(Toolbox.Instance.Disposables);

		SubscribeInput();
	}

	public void TryIntercat()
	{
		if (currentInteractable != null && controller.Collisions.below)
		{
			input.TrySetDefaultInputActive(false, true);
			currentInteractable.Interact(this);
		}
	}

	private void InitializeDelegates()
	{
		onHealthChange = delegate { msg.Send(ServiceShareData.UPDATE_UI, this, PlayerHealth.HP, "health"); };
		onJump = delegate { anim.SetTrigger("Jump"); };
	}

	private void InitializeFields()
	{
		controller = GetComponent<Controller2D>();
		PlayerHealth = GetComponent<Health>();
		t = transform;
		IsUpdatingTurn = true;
	}

	private void InitializeAbilities()
	{
		abilities.ForEach(a => 
		{
			if (a is IAbility)
			{
				(a as IAbility).AbilityAwake(t, anim);

				if (enableAbilitiesAtStart)
				{
					(a as IAbility).Enable();
				}
				else
				{
					(a as IAbility).Disable();
				}

				abilitiesDictionary.Add((a as IAbility).Type, a as IAbility); 
			}
		});
	}

	public void TryEnableAbility(AbilityType type)
	{
		abilitiesDictionary.TryGetValue(type, out IAbility abilityToEnable);
		abilityToEnable?.Enable();
	}

	public void TryDisableAbility(AbilityType type)
	{
		abilitiesDictionary.TryGetValue(type, out IAbility abilityToEnable);
		abilityToEnable?.Disable();
	}

	private void TurnHandle(float xInput)
	{
		if (xInput == 0 || !IsUpdatingTurn) return;

		if (xInput > 0 && !facingRight)
		{
			Turn(true);
		}

		if (xInput < 0 && facingRight)
		{
			Turn(false);
		}
	}

	public void Turn(bool right)
	{
		graphicsRoot.localEulerAngles = new Vector3(0, right ? 0 : 180, 0);
		facingRight = right;
		controller.ChangeFacingDirection(right ? 1 : -1);
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

	private void InvulnerabilityUpdate()
	{
		if(currentInvulnerabilityTime > 0)
		{
			currentInvulnerabilityTime -= Time.deltaTime;
		}
	}

	public void TryTakeDamage(HitInfo info)
	{
		if (IsInvulnerable)
		{
			return;
		}

		//TODO: side handle
		//TODO: hit effect
		Debug.Log("player hitted at " + info.damage + " hp from " + info.from);

		PlayerHealth.Hit(info.damage);
		currentInvulnerabilityTime = invulnerabilityTime;
	}

	private void AnimationUpdate()
	{
		anim.SetFloat("Velocity X", controller.Velocity.x);
		anim.SetFloat("Velocity Y", controller.Velocity.y);
		anim.SetBool("Grounded", controller.Collisions.above || controller.Collisions.below);
		anim.SetBool("Sliding Wall", controller.WallSliding);
		anim.SetFloat("Wall Sliding Input X", input.MoveInput.x * -controller.WallDirectionX);
	}

	private void SubscribeInput()
	{
		if (!subscribed)
		{
			//Put all subs here

			if (input && controller && anim && PlayerHealth)
			{
				PlayerHealth.OnHealthChange += onHealthChange;

				input.Interact += TryIntercat;

				input.JumpStart += controller.OnJumpInputDown;
				input.JumpEnd += controller.OnJumpInputUp;

				input.JumpStart += abilitiesDictionary[AbilityType.DOUBLE_JUMP].StartUse;
				input.JumpEnd += abilitiesDictionary[AbilityType.DOUBLE_JUMP].StopUse;

				input.JumpStart += abilitiesDictionary[AbilityType.TRIPLE_JUMP].StartUse;
				input.JumpEnd += abilitiesDictionary[AbilityType.TRIPLE_JUMP].StopUse;

				input.Dash += abilitiesDictionary[AbilityType.DASH].StartUse;

				input.Attack += abilitiesDictionary[AbilityType.SLASH].StartUse;

				controller.OnJump += onJump;

				subscribed = true;
			}
		}
	}

	private void UnsubscribeInput()
	{
		if (subscribed)
		{
			//Put all unsubs here

			if (input && controller && anim && PlayerHealth)
			{
				PlayerHealth.OnHealthChange -= onHealthChange;

				input.Interact -= TryIntercat;

				input.JumpStart -= controller.OnJumpInputDown;
				input.JumpEnd -= controller.OnJumpInputUp;

				input.JumpStart -= abilitiesDictionary[AbilityType.DOUBLE_JUMP].StartUse;
				input.JumpEnd -= abilitiesDictionary[AbilityType.DOUBLE_JUMP].StopUse;

				input.JumpStart -= abilitiesDictionary[AbilityType.TRIPLE_JUMP].StartUse;
				input.JumpEnd -= abilitiesDictionary[AbilityType.TRIPLE_JUMP].StopUse;

				input.Dash -= abilitiesDictionary[AbilityType.DASH].StartUse;

				input.Attack -= abilitiesDictionary[AbilityType.SLASH].StartUse;

				controller.OnJump -= onJump;

				subscribed = false;
			}
		}
	}

	private void  BindAbilityToInput(Action actionPerformed, Action actionCancelled, AbilityType abilityType)
	{
		if(abilitiesDictionary.TryGetValue(abilityType, out IAbility ability))
		{
			if (actionPerformed != null)
				actionPerformed += ability.StartUse;

			if (actionCancelled != null)
				actionCancelled += ability.StopUse;
		}
	}

	private void UnbindAbilityFromInput(Action actionPerformed, Action actionCancelled, AbilityType abilityType)
	{
		if (abilitiesDictionary.TryGetValue(abilityType, out IAbility ability))
		{
			if (actionPerformed != null)
				actionPerformed -= ability.StartUse;

			if (actionCancelled != null)
				actionCancelled -= ability.StopUse;
		}
	}
}