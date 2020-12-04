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
	[SerializeField] private int initialHitPoints;
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

	private Dictionary<AbilityType, IAbility> abilitiesDictionary = new Dictionary<AbilityType, IAbility>();
	private Transform t;
	private Camera mainCam;
	private IInteractable currentInteractable;
	private HitPoints hp;

	private bool subscribed = false;
	private bool facingRight = true;

	private Action onJump;

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
		InitializeSubscribes();
		InitializeDelegates();

		hp = new HitPoints();
		hp.SetMaxHitPoints(initialHitPoints);
		hp.Reset();
	}

	private void InitializeSubscribes()
	{
		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.DIALOG_CLOSED).Subscribe(_ =>
		{
			input.TrySetDefaultInputActive(true, true);
		}).AddTo(Toolbox.Instance.Disposables);

		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.HIT_CHARACTER && x.tag == "player").Subscribe(x => 
		{
			Hit((int)x.data);
		}).AddTo(Toolbox.Instance.Disposables);

		SubscribeInput();
	}

	public void TryIntercat()
	{
		if (currentInteractable != null)
		{
			currentInteractable.Interact();
			input.TrySetDefaultInputActive(false, true);
		}
	}

	private void InitializeDelegates()
	{
		onJump = delegate { anim.SetTrigger("Jump"); };
	}

	private void InitializeFields()
	{
		controller = GetComponent<Controller2D>();
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
				(a as IAbility).Enable();
				abilitiesDictionary.Add((a as IAbility).Type, a as IAbility); 
			}
		});
	}

	public void Respawn()
	{
		hp.Reset();
		msg.Send(ServiceShareData.UPDATE_UI, null, hp, "health");
	}

	public void Hit(int hitAmount)
	{
		hp.Hit(hitAmount);
		msg.Send(ServiceShareData.UPDATE_UI, null, hp, "health");
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

			if (input && controller && anim)
			{
				input.Interact += TryIntercat;

				input.JumpStart += controller.OnJumpInputDown;
				input.JumpEnd += controller.OnJumpInputUp;

				input.JumpStart += abilitiesDictionary[AbilityType.DOUBLE_JUMP].StartUse;
				input.JumpEnd += abilitiesDictionary[AbilityType.DOUBLE_JUMP].StopUse;

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

			if (input && controller && anim)
			{
				input.Interact -= TryIntercat;

				input.JumpStart -= controller.OnJumpInputDown;
				input.JumpEnd -= controller.OnJumpInputUp;

				input.JumpStart -= abilitiesDictionary[AbilityType.DOUBLE_JUMP].StartUse;
				input.JumpEnd -= abilitiesDictionary[AbilityType.DOUBLE_JUMP].StopUse;

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
		Process = true;

		SubscribeInput();
	}

	private void OnDisable()
	{
		Process = false;

		UnsubscribeInput();
	}
}

[System.Serializable]
public struct HitPoints
{
	public int maxHitPoints { get; private set; }
	public int currentHitPoints { get; private set; }

	public void Hit(int amount)
	{
		currentHitPoints -= Mathf.Abs(amount);
	}

	public void SetMaxHitPoints(int amount)
	{
		maxHitPoints = Mathf.Abs(amount);
	}

	public void AddMaxHitPoints(int amount)
	{
		maxHitPoints += Mathf.Abs(amount);
	}

	public void Reset()
	{
		currentHitPoints = maxHitPoints;
	}

	public override string ToString()
	{
		return "current hp - " + currentHitPoints + ". max hp - " + maxHitPoints;
	}
}