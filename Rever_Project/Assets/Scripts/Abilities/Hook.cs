using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RedBlueGames.LiteFSM;

[CreateAssetMenu(menuName = "Game/Abilities/Hook")]
public class Hook : ScriptableObject, IUseable
{
	[SerializeField] private float distance = 5;
	[SerializeField] private float attractionSpeed = 10;
	[SerializeField] private float dropDistance = 1;
	[SerializeField] private LayerMask groundLayer;
	[Header("Line")]
	[SerializeField] private AnimationCurve startCurve;
	[SerializeField] private float lineWidth;
	[SerializeField] private Material lineMaterial;
	[SerializeField] private float hookSpeed;

	private bool hooking = false;
	private Controller2D characterController = null;
	private Animator anim;
	private Transform characterTransform = null;
	private Vector2 targetPosition;
	private GameObject lineHolder;
	private LineRenderer line;

	private StateMachine<HookState> fsm;

	private enum HookState
	{
		Hooking,
		ReachDestionation,
		Disabled
	}

	public void AbilityAwake(Transform character, Animator animator)
	{
		characterTransform = character; //setting character transform
		characterController = characterTransform.GetComponent<Controller2D>(); //getting character controller
		this.anim = animator;

		//adding line renderer
		lineHolder = new GameObject("Hook Line Holder"); 
		lineHolder.transform.parent = character;
		line = lineHolder.AddComponent<LineRenderer>();
		line.SetWidth(lineWidth, lineWidth);
		line.material = lineMaterial;
		
		StateMachineAwake();
	}

	private void StateMachineAwake()
	{
		var stateList = new List<State<HookState>>();
		stateList.Add(new State<HookState>(HookState.Disabled, this.DisableEnter, null, null));
		stateList.Add(new State<HookState>(HookState.ReachDestionation, this.ReachDestinationEnter, null, this.ReachedDestionationUpdate));
		stateList.Add(new State<HookState>(HookState.Hooking, this.HookingEnter, null, this.HookingUpdate));
		this.fsm = new StateMachine<HookState>(stateList.ToArray(), HookState.Disabled);
	}

	public IEnumerator AbilityUpdate()
	{
		while(true)
		{
			fsm.Update(Time.deltaTime);
			yield return null;
		}
	}

	public void StartUse(Vector2 usePosition)
	{
		anim.SetTrigger("Start Hooking");

		Vector2 charPos = new Vector2(characterTransform.position.x, characterTransform.position.y);

		RaycastHit2D hit;
		hit = Physics2D.Raycast(characterTransform.position, (usePosition - charPos).normalized, distance, groundLayer);

		if(hit)
		{
			targetPosition = hit.point;

			Vector2 hookDir = (targetPosition - charPos).normalized;

			anim.SetFloat("Hooking Angle X", hookDir.x);
			anim.SetFloat("Hooking Angle Y", hookDir.y);

			fsm.ChangeState(HookState.Hooking);
		}
	}

	public void StopUse(Vector2 usePosition)
	{
		fsm.ChangeState(HookState.Disabled);
	}

	private void HookingEnter()
	{
		characterController.currentAbilityToStickWall = false;
		characterController.SetGravity(0);
		characterController.useInput = false;
		hooking = true;
		line.enabled = true;
		line.positionCount = 2;
		line.SetPosition(1, targetPosition);
	}

	private void HookingUpdate(float d)
	{
		line.SetPosition(0, characterTransform.position);
		Vector2 charPos = new Vector2(characterTransform.position.x, characterTransform.position.y);

		//TODO: check colliding side, because current version can cause weird behaviour
		if (Vector2.Distance(charPos, targetPosition) <= dropDistance && characterController.Collisions.Colliding)
		{
			fsm.ChangeState(HookState.ReachDestionation);
		}

		characterController.SetForce((targetPosition - charPos).normalized * attractionSpeed);
	}

	private void ReachDestinationEnter()
	{
		anim.SetTrigger("Hooking Reached");
		characterController.SetGravity(0);
		characterController.useInput = false;
	}

	private void ReachedDestionationUpdate(float d)
	{
		characterController.SetForce(Vector3.zero);
	}

	private void DisableEnter()
	{
		anim.SetTrigger("Stop Hooking");
		characterController.currentAbilityToStickWall = characterController.CanStickWall;
		characterController.ResetGravity();
		characterController.useInput = true;

		hooking = false;
		line.enabled = false;
	}
}
