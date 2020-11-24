using UnityEngine;
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

	private IAbility currentAbility;
	private int currentAbilityIndex;
	private Transform t;
	private Camera mainCam;
	private float currentGroundAngle;
	private IInteractable currentInteractable;
	private HitPoints hp;

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
		InitializeFields();
		InitializeSubscribes();
		InitializeAbilities();
		InitializeDelegates();

		hp.SetMaxHitPoints(initialHitPoints);
		hp.Reset();
	}

	private void InitializeSubscribes()
	{
		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.DIALOG_CLOSED).Subscribe(_ =>
		{
			input.SetDefaultInputActive(true);
		}).AddTo(Toolbox.Instance.Disposables);

		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.HIT_CHARACTER && x.tag == "player").Subscribe(x => 
		{
			Hit((int)x.data);
		}).AddTo(Toolbox.Instance.Disposables);

		SetSubscribe(true);
	}

	private void InitializeDelegates()
	{
		tryInteract = delegate
		{
			if (currentInteractable != null)
			{
				currentInteractable.Interact();
				input.SetDefaultInputActive(false);
			}
		};

		startUse = delegate { currentAbility.StartUse(mainCam.ScreenToWorldPoint(input.PointerPosition)); };
		stopUse = delegate { currentAbility.StopUse(mainCam.ScreenToWorldPoint(input.PointerPosition)); };
		onJump = delegate { anim.SetTrigger("Jump"); };
	}

	public void Dash()
	{
		(abilities.Where(x => (x as IAbility).type == AbilityType.DASH).FirstOrDefault() as IAbility).StartUse(Vector2.zero);
	}

	private void InitializeFields()
	{
		controller = GetComponent<Controller2D>();
		t = transform;
		mainCam = Camera.main;
	}

	private void InitializeAbilities()
	{
		abilities.ForEach(a => { if (a is IAbility) (a as IAbility).AbilityAwake(t, anim); });

		currentAbilityIndex = 0;
		SetAbility(currentAbilityIndex);
	}

	public void Respawn()
	{
		hp.Reset();
		msg.Send(ServiceShareData.UPDATE_UI, this, hp, "Health");
	}

	public void Hit(int hitAmount)
	{
		hp.Hit(hitAmount);
		msg.Send(ServiceShareData.UPDATE_UI, this, hp, "health");
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

		currentAbility = abilities[currentAbilityIndex] as IAbility;

		Debug.Log("Set ability " + index);

		StartCoroutine(currentAbility.AbilityUpdate());
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
					input.Dash += Dash;
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
					input.Dash -= Dash;
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