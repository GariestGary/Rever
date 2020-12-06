using UnityEngine;
using System.Collections;
using Zenject;
using System.Collections.Generic;
using System;
using System.Linq;
using UniRx;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour, ITick, IFixedTick, IAwake
{
	[SerializeField] private Animator anim;
	[SerializeField] private Transform graphicsRoot;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private BoxCollider2D colliderBox;
	[Space]
	[SerializeField] private LayerMask interactableLayer;
	[SerializeField] private float interactRadius;
	[Space]
	[SerializeField] private List<ScriptableObject> abilities = new List<ScriptableObject>();
	public bool Process => process;
	public Health PlayerHealth => health;

	private Controller2D controller;
	private InputManager input;
	private GameManager game;
	private MessageManager msg;

	private Dictionary<AbilityType, IAbility> abilitiesDictionary = new Dictionary<AbilityType, IAbility>();
	private Transform t;
	private Camera mainCam;
	private Health health;
	private IInteractable currentInteractable;
	private bool process;

	private bool subscribed = false;
	private bool facingRight = true;

	private Action onJump;
	private Action onHealthChange;

	[Inject]
	public void Constructor(InputManager input, GameManager game, MessageManager msg)
	{
		this.input = input;
		this.game = game;
		this.msg = msg;
	}

	public void OnAwake() 
	{
		InitializeFields();
		InitializeAbilities();
		InitializeDelegates();
		InitializeSubscribes();

		health.Initialize();
	}

	private void InitializeSubscribes()
	{
		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.DIALOG_CLOSED).Subscribe(_ =>
		{
			input.TrySetDefaultInputActive(true, true);
		}).AddTo(Toolbox.Instance.Disposables);

		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.HIT_CHARACTER && x.tag == "player").Subscribe(x => 
		{
			health.Hit((int)x.data);
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
		onHealthChange = delegate { msg.Send(ServiceShareData.UPDATE_UI, this, health.HP, "health"); };
		onJump = delegate { anim.SetTrigger("Jump"); };
	}

	private void InitializeFields()
	{
		controller = GetComponent<Controller2D>();
		health = GetComponent<Health>();
		t = transform;
		mainCam = Camera.main;
	}

	private void InitializeAbilities()
	{
		abilities.ForEach(a => 
		{
			if (a is IAbility)
			{
				(a as IAbility).AbilityAwake(t, anim);
				//TODO: Temp
				(a as IAbility).Disable();
				abilitiesDictionary.Add((a as IAbility).Type, a as IAbility); 
			}
		});
	}

	public void OnTick() 
	{
		controller.HandleInput(input.MoveInput);

		AnimationUpdate();
		TurnHandle(input.MoveInput.x);
		InteractHandle();

		for (int i = 0; i < abilities.Count; i++)
		{
			(abilities[i] as IAbility).AbilityUpdate();
		}
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
		if (xInput == 0) return;

		if (xInput > 0 && !facingRight)
		{
			//TODO: make animation turn... or not...
			graphicsRoot.localEulerAngles = new Vector3(0, 0, 0);
			facingRight = true;
		}

		if (xInput < 0 && facingRight)
		{
			graphicsRoot.localEulerAngles = new Vector3(0, 180, 0);
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

		for (int i = 0; i < abilities.Count; i++)
		{
			(abilities[i] as IAbility).AbilityFixedUpdate();
		}
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

			if (input && controller && anim && health)
			{
				health.OnHealthChange += onHealthChange;

				input.Interact += TryIntercat;

				input.JumpStart += controller.OnJumpInputDown;
				input.JumpEnd += controller.OnJumpInputUp;

				input.JumpStart += abilitiesDictionary[AbilityType.DOUBLE_JUMP].StartUse;
				input.JumpEnd += abilitiesDictionary[AbilityType.DOUBLE_JUMP].StopUse;

				input.JumpStart += abilitiesDictionary[AbilityType.TRIPLE_JUMP].StartUse;
				input.JumpEnd += abilitiesDictionary[AbilityType.TRIPLE_JUMP].StopUse;

				input.Dash += abilitiesDictionary[AbilityType.DASH].StartUse;

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

			if (input && controller && anim && health)
			{
				health.OnHealthChange -= onHealthChange;

				input.Interact -= TryIntercat;

				input.JumpStart -= controller.OnJumpInputDown;
				input.JumpEnd -= controller.OnJumpInputUp;

				input.JumpStart -= abilitiesDictionary[AbilityType.DOUBLE_JUMP].StartUse;
				input.JumpEnd -= abilitiesDictionary[AbilityType.DOUBLE_JUMP].StopUse;

				input.JumpStart -= abilitiesDictionary[AbilityType.TRIPLE_JUMP].StartUse;
				input.JumpEnd -= abilitiesDictionary[AbilityType.TRIPLE_JUMP].StopUse;

				input.Dash -= abilitiesDictionary[AbilityType.DASH].StartUse;

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

	private void OnEnable()
	{
		process = true;

		SubscribeInput();
	}

	private void OnDisable()
	{
		process = false;

		UnsubscribeInput();
	}
}