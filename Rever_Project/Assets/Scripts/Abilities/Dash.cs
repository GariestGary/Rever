using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.LiteFSM;
using UniRx;

[CreateAssetMenu(menuName = "Game/Abilities/Dash")]
public class Dash : ScriptableObject, IAbility
{
	[SerializeField] private float timeForDash;
	[SerializeField] private float dashDistance;
	public AbilityType type => AbilityType.DASH;

	private Controller2D characterController;
	private float dashForce;
	private float currentTime;

	private StateMachine<DashState> fsm;

	private enum DashState
	{
		IN_DASH,
		NONE,
	}

	public void AbilityAwake(Transform character, Animator anim)
	{
		dashForce = dashDistance / timeForDash;
		characterController = character.GetComponent<Controller2D>();

		StateMachineAwake();
	}

	private void StateMachineAwake()
	{
		var stateList = new List<State<DashState>>();
		stateList.Add(new State<DashState>(DashState.IN_DASH, this.InDashEnter, null, this.InDashUpdate));
		stateList.Add(new State<DashState>(DashState.NONE, this.NoneEnter, null, null));
		this.fsm = new StateMachine<DashState>(stateList.ToArray(), DashState.NONE);
	}

	public IEnumerator AbilityUpdate()
	{
		while (true)
		{
			fsm.Update(Time.fixedDeltaTime);
			yield return new WaitForFixedUpdate();
		}
	}

	private void InDashEnter()
	{
		characterController.UseVerticalCollisions = false;
		characterController.SetGravity(0);
		currentTime = timeForDash;
		characterController.useInput = false;
		FMODUnity.RuntimeManager.PlayOneShot("event:/Dash");
	}

	private void InDashUpdate(float d)
	{
		if (currentTime > 0)
		{
			currentTime -= d;
			characterController.SetForce(new Vector3(dashForce * characterController.LastInputFacing, 0, 0));
		}
		else
		{
			fsm.ChangeState(DashState.NONE);
		}
	}

	private void NoneEnter()
	{
		characterController.UseVerticalCollisions = true;
		characterController.ResetGravity();
		characterController.useInput = true;
	}

	public void StartUse(Vector2 usePosition)
	{
		fsm.ChangeState(DashState.IN_DASH);
	}

	public void StopUse(Vector2 usePosition)
	{
		
	}
}
