using UnityEngine;
using System.Collections;
using Zenject;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.U2D.IK;
using UnityEngine.U2D.Animation;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour, ITick, IFixedTick, IAwake
{
	[SerializeField] private Animator anim;
	[SerializeField] private SpriteSkin skin;
	[SerializeField] private Transform graphicsRoot;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private BoxCollider2D colliderBox;
	[Header("Legs IK")]
	[SerializeField] private Transform leftLegRaycastPoint;
	[SerializeField] private Transform rightLegRaycastPoint;
	[Space]
	[SerializeField] private Transform rightLegSolverTransform;
	[SerializeField] private Transform leftLegSolverTransform;
	[Space]
	[SerializeField] private LimbSolver2D rightLegSolver;
	[SerializeField] private LimbSolver2D leftLegSolver;
	[Space]
	[SerializeField] private Transform leftFeetBone;
	[SerializeField] private Transform rightFeetBone;
	[Space]
	[SerializeField] private float legsIKRaycastDistance;
	[SerializeField] private float angleForLegsIK;
	[Header("Skeleton Height")]
	[SerializeField] private Transform playerSkeletonRoot;
	[SerializeField] private float heightCheckDistance;
	[SerializeField] private float skeletonHeightOffset;
	[SerializeField] private float skeletonHeightAdjustSpeed;
	[Space]
	[SerializeField] private List<ScriptableObject> abilities = new List<ScriptableObject>();
	public bool Process { get; private set; }

	private Controller2D controller;
	private InputManager input;
	private GameManager game;
	private IUseable currentAbility;
	private int currentAbilityIndex;
	private Transform t;
	private Camera mainCam;
	private float initialLeftFeetZRotation;
	private float initialRightFeetZRotation;
	private float currentGroundAngle;
	

	private bool subscribed = false;
	private bool facingRight = true;

	private Action startUse;
	private Action stopUse;
	private Action onJump;

	[Inject]
	public void Constructor(InputManager input, GameManager game)
	{
		this.input = input;
		this.game = game;
	}

	public void OnAwake() 
	{
		controller = GetComponent<Controller2D>();
		t = transform;
		mainCam = Camera.main;

		abilities.ForEach(a => { if (a is IUseable) (a as IUseable).AbilityAwake(t); });

		currentAbilityIndex = 0;
		SetAbility(currentAbilityIndex);

		startUse = delegate { currentAbility.StartUse(mainCam.ScreenToWorldPoint(input.PointerPosition)); };
		stopUse = delegate { currentAbility.StopUse(mainCam.ScreenToWorldPoint(input.PointerPosition)); };
		onJump = delegate { anim.SetTrigger("Jump"); };

		initialLeftFeetZRotation = leftFeetBone.localEulerAngles.z;
		initialRightFeetZRotation = rightFeetBone.localEulerAngles.z;

		SetSubscribe(true);

	}
	public void OnTick() 
	{

		controller.HandleInput(input.MoveInput);

		AnimationUpdate();
		TurnHandle(input.MoveInput.x);
		SkeletonHeightHandle();

		UpdateIK();
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

	private void SkeletonHeightHandle()
	{
		RaycastHit2D hit;
		float targetHeight;

		Vector2 feetPos = new Vector2(t.position.x, t.position.y - colliderBox.bounds.extents.y);
		Debug.DrawRay(feetPos, Vector2.down * heightCheckDistance);

		hit = Physics2D.Raycast(feetPos, Vector2.down, heightCheckDistance, groundLayer);

		currentGroundAngle = hit?Math.Abs(Vector2.Angle(Vector2.Perpendicular(hit.normal), Vector2.up)):0;

		if(hit && controller.Collisions.below)
		{
			targetHeight = hit.distance + skeletonHeightOffset;
		}
		else
		{
			targetHeight = skeletonHeightOffset;
		}

		playerSkeletonRoot.localPosition = new Vector3(playerSkeletonRoot.localPosition.x, -targetHeight, playerSkeletonRoot.localPosition.z);
	}

	private void TurnHandle(float xInput)
	{
		if (xInput == 0) return;

		if (xInput > 0 && !facingRight)
		{
			graphicsRoot.localEulerAngles = new Vector3(0, 0, 0);
			facingRight = true;
		}

		if (xInput < 0 && facingRight)
		{
			graphicsRoot.localEulerAngles = new Vector3(0, 180, 0);
			facingRight = false;
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

	public void UpdateIK()
	{
		leftFeetBone.localEulerAngles = new Vector3(0, 0, initialLeftFeetZRotation);
		rightFeetBone.localEulerAngles = new Vector3(0, 0, initialRightFeetZRotation);

		anim.Update(Time.deltaTime);

		if (input.MoveInput.x != 0 || !(controller.Collisions.below && currentGroundAngle > angleForLegsIK))
		{
			return;
		}

		RaycastHit2D leftHit, rightHit;

		leftHit = Physics2D.Raycast(leftLegRaycastPoint.position, Vector2.down, legsIKRaycastDistance, groundLayer);
		rightHit = Physics2D.Raycast(rightLegRaycastPoint.position, Vector2.down, legsIKRaycastDistance, groundLayer);

		Debug.DrawRay(leftLegRaycastPoint.position, Vector2.down * legsIKRaycastDistance, Color.blue);
		Debug.DrawRay(rightLegRaycastPoint.position, Vector2.down * legsIKRaycastDistance, Color.blue);

		if (leftHit)
		{
			leftFeetBone.localEulerAngles = new Vector3(0, 0, Vector2.Angle(Vector2.Perpendicular(leftHit.normal) * (facingRight ? 1 : -1), Vector2.up));
			leftLegSolverTransform.position = leftHit.point;
			leftLegSolver.UpdateIK(1);
		}

		if(rightHit)
		{
			rightFeetBone.localEulerAngles = new Vector3(0, 0, Vector2.Angle(Vector2.Perpendicular(rightHit.normal) * (facingRight?1:-1), Vector2.up));
			rightLegSolverTransform.position = rightHit.point;
			rightLegSolver.UpdateIK(1);
		}

		
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