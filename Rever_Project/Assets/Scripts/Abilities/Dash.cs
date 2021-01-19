using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.LiteFSM;
using UniRx;

[CreateAssetMenu(menuName = "Game/Abilities/Dash")]
public class Dash : DefaultAbility
{
	[SerializeField] private float timeForDash;
	[SerializeField] private float dashDistance;
	[SerializeField] private float jumpBeforeDashAmount;
	public override AbilityType Type => AbilityType.DASH;

	private float dashForce;
	private float currentTime;
	private int dashDirection;

	private StateMachine<DashState> fsm;

	private enum DashState
	{
		IN_DASH,
		NONE,
	}

	public override void AbilityAwake(Transform character, Animator anim)
	{
		base.AbilityAwake(character, anim);

		dashForce = dashDistance / timeForDash;
		StateMachineAwake();
	}

	private void StateMachineAwake()
	{
		var stateList = new List<State<DashState>>();
		stateList.Add(new State<DashState>(DashState.IN_DASH, this.InDashEnter, null, this.InDashUpdate));
		stateList.Add(new State<DashState>(DashState.NONE, this.NoneEnter, null, null));
		this.fsm = new StateMachine<DashState>(stateList.ToArray(), DashState.NONE);
	}

	public override void AbilityFixedUpdate()
	{
		if (enabled)
		{
			fsm.Update(Time.fixedDeltaTime);
		}
	}

	private void InDashEnter()
	{
		playerController.UseVerticalCollisions = false;
		playerController.SetGravity(0);
		playerTransform.Translate(new Vector3(0, jumpBeforeDashAmount, 0));
		currentTime = timeForDash;
		playerController.useInput = false;
		playerAnimator.SetTrigger("Dash");

		if(playerController.WallSliding)
		{
			dashDirection = -playerController.WallDirectionX;
			player.IsUpdatingTurn = false;
			player.Turn(!(playerController.WallDirectionX > 0));
		}
		else
		{
			dashDirection = (int)playerController.LastInputFacing.x;
		}
	}

	private void InDashUpdate(float d)
	{
		if (currentTime > 0)
		{
			currentTime -= d;
			playerController.SetForce(new Vector3(dashForce * dashDirection, 0, 0));
		}
		else
		{
			fsm.ChangeState(DashState.NONE);
		}
	}

	private void NoneEnter()
	{
		playerController.UseVerticalCollisions = true;
		playerController.ResetGravity();
		playerController.useInput = true;
		player.IsUpdatingTurn = true;
	}

	public override void StartUse()
	{
		if (enabled)
		{
			fsm.ChangeState(DashState.IN_DASH);
		}
	}

	public override void Enable()
	{
		base.Enable();
		fsm.ChangeState(DashState.NONE);
	}

	public override void Disable()
	{
		base.Disable();
		fsm.ChangeState(DashState.NONE);
	}

}
