using RedBlueGames.LiteFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MechanicalArmsBoss : Boss
{
	[BoxGroup("Right Arm")] [SerializeField] private Transform rightArmTarget;
	[BoxGroup("Right Arm")] [SerializeField] private Transform rightArmTargetParent;
	[BoxGroup("Left Arm")] [SerializeField] private Transform leftArmTarget;
	[BoxGroup("Left Arm")] [SerializeField] private Transform leftArmTargetParent;
	[Range(0, 1)]
	[SerializeField] private float noiseInfluence;
	[SerializeField] private float leftXMaxPosition;
	[SerializeField] private float rightXMaxPosition;
	[SerializeField] private float followSmooth;
	[Range(0, 25)]
	[SerializeField] private float xFollowPositionCalibrate;
	[InfoBox("Correspond each arm positions's position in list to state position in enum")]
	[SerializeField] private bool writePositions;
	[ShowIf("writePositions")]
	[OnValueChanged("ShowPositions")]
	[SerializeField] private States stateToPreview = States.Idle;
	[ShowIf("writePositions")]
	[SerializeField] private List<ArmPositions> positions = new List<ArmPositions>();
	[SerializeField] private List<States> attackStatesToChoose;

	private float currentTimer;
	private Vector3 wholeTargetPosition;
	private Vector3 initLeftArmTargetPos;
	private Vector3 initRightArmTargetPos;
	private ArmPositions currentPosition;

	[System.Serializable]
	public class ArmPositions
	{
		[ReadOnly]
		public string name;
		[OnValueChanged("ChangeName")]
		[AllowNesting]
		public States state;
		public float minDuration;
		public float maxDuration;
		public float additionalTime;
		public Ease ease = Ease.InQuad;
		[InfoBox("X for left arm, Y for right arm")]
		[Space]
		public Vector2 noiseInfluence;
		public Vector3 leftArm;
		public Vector3 rightArm;

		private void ChangeName()
		{
			name = state.ToString();
		}
	}

	[ShowIf("writePositions")]
	[Button]
	public void WritePositions()
	{
		positions[(int)stateToPreview].leftArm = leftArmTarget.localPosition;
		positions[(int)stateToPreview].rightArm = rightArmTarget.localPosition;
	}

	[ShowIf("writePositions")]
	[Button]
	public void ShowPositions()
	{
		leftArmTarget.localPosition = positions[(int)stateToPreview].leftArm;
		rightArmTarget.localPosition = positions[(int)stateToPreview].rightArm;
	}

	public enum States 
	{
		Idle, 
		FollowPlayer, 
		ChargeAttackOneHand, 
		AttackOneHand, 
		SlideToSide, 
		ChargeSlide, 
		SlideSaw, 
		SlideToCenter, 
		Death, 
		Sleep, 
		Awakening, 
		ChargeAttackBothHand, 
		SlideBothHandsTogether,
		AttackBothHand, 
		Dead, 
		Damaged,
	}

	private StateMachine<States> fsm;

	public override void Rise()
	{
		base.Rise();

		initLeftArmTargetPos = leftArmTarget.position;
		initRightArmTargetPos = rightArmTarget.position;
	}

	public override void Ready()
	{
		SetupFSM();
	}

	private void DoArmsMove(States state, bool overrideLeftX = false, float leftX = 0, bool overrideRightX = false, float rightX = 0)
	{
		currentPosition = positions.Where(x => x.state == state).FirstOrDefault();

		Vector3 left = currentPosition.leftArm;
		Vector3 right = currentPosition.rightArm;

		if(overrideLeftX)
		{
			left.x = leftX;
		}

		if(overrideRightX)
		{
			right.x = rightX;
		}

		leftArmTarget.DOLocalMove(left, Random.Range(currentPosition.minDuration, currentPosition.maxDuration)).SetEase(currentPosition.ease);
		rightArmTarget.DOLocalMove(right, Random.Range(currentPosition.minDuration, currentPosition.maxDuration)).SetEase(currentPosition.ease);

		//leftArmTarget.DOShakePosition(currentPosition.duration, currentPosition.noiseInfluence.x * noiseInfluence);
		//rightArmTarget.DOShakePosition(currentPosition.duration, currentPosition.noiseInfluence.y * noiseInfluence);

		currentTimer = currentPosition.minDuration + currentPosition.additionalTime;
	}

	public override void Tick()
	{
		base.Tick();

		fsm.Update(Time.deltaTime);
		WholePositionUpdate();
	}

	private void WholePositionUpdate()
	{
		transform.localPosition = Vector3.Lerp(transform.localPosition, wholeTargetPosition, Time.deltaTime * followSmooth);
		transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, leftXMaxPosition, rightXMaxPosition), transform.localPosition.y);
	}

	private Vector3 GetPlayerDelta()
	{
		return new Vector3(game.CurrentPlayer.transform.position.x - transform.position.x + xFollowPositionCalibrate, transform.localPosition.y);
	}

	protected virtual void SetupFSM()
	{
		var states = new List<State<States>>();
		states.Add(new State<States>(States.Idle, EnterIdle, null, UpdateIdle));
		states.Add(new State<States>(States.Damaged, EnterDamaged, null, UpdateDamaged));
		states.Add(new State<States>(States.FollowPlayer, EnterFollowPlayer, null, UpdateFollowPlayer));
		states.Add(new State<States>(States.ChargeAttackOneHand, EnterChargeAttackOneHand, null, UpdateChargeAttackOneHand));
		states.Add(new State<States>(States.AttackOneHand, EnterAttackOneHand, null, UpdateAttackOneHand));
		states.Add(new State<States>(States.SlideToSide, EnterSlideToSide, null, UpdateSlideToSide));
		states.Add(new State<States>(States.ChargeSlide, EnterChargeSlide, null, UpdateChargeSlide));
		states.Add(new State<States>(States.SlideSaw, EnterSlideSaw, null, UpdateSlideSaw));
		states.Add(new State<States>(States.SlideToCenter, EnterSlideToCenter, null, UpdateSlideToCenter));
		states.Add(new State<States>(States.Death, EnterDeath, null, UpdateDeath));
		states.Add(new State<States>(States.Sleep, EnterSleep, null, UpdateSleep));
		states.Add(new State<States>(States.Awakening, EnterAwakening, null, UpdateAwakening));
		states.Add(new State<States>(States.ChargeAttackBothHand, EnterChargeAttackBothHand, null, UpdateChargeAttackBothHand));
		states.Add(new State<States>(States.AttackBothHand, EnterAttackBothHand, null, UpdateAttackBothHand));
		states.Add(new State<States>(States.SlideBothHandsTogether, EnterSlideBothHandsTogether, null, UpdateSlideBothHandsTogether));
		states.Add(new State<States>(States.Dead, EnterDead, null, UpdateDead));
	
		fsm = new StateMachine<States>(states.ToArray(), States.Idle);
	}

	#region states

	//IDLE========================================================
	protected virtual void EnterIdle()
	{
		DoArmsMove(States.Idle);
	}

	protected virtual void UpdateIdle(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.FollowPlayer);
		}

		wholeTargetPosition = GetPlayerDelta();

		currentTimer -= d;
	}

	//DAMAGED=========================================================
	protected virtual void EnterDamaged()
	{
		DoArmsMove(States.Damaged);
	}

	protected virtual void UpdateDamaged(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.Idle);
		}
	}

	//FOLLOWPLAYER========================================================
	protected virtual void EnterFollowPlayer()
	{
		DoArmsMove(States.FollowPlayer);
	}

	protected virtual void UpdateFollowPlayer(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(attackStatesToChoose[Random.Range(0, attackStatesToChoose.Count)]);
		}

		currentTimer -= d;
	}

	//CHARGEATTACKONEHAND========================================================
	protected virtual void EnterChargeAttackOneHand()
	{
		DoArmsMove(States.ChargeAttackOneHand);
	}

	protected virtual void UpdateChargeAttackOneHand(float d)
	{
		if (currentTimer <= 0)
		{
			fsm.ChangeState(States.AttackOneHand);
		}

		wholeTargetPosition = GetPlayerDelta();

		currentTimer -= d;
	}

	//ATTACKONEHAND========================================================
	protected virtual void EnterAttackOneHand()
	{
		DoArmsMove(fsm.CurrentStateID);
	}

	protected virtual void UpdateAttackOneHand(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.FollowPlayer);
		}

		currentTimer -= d;
	}

	//SLIDETOSIDE========================================================
	protected virtual void EnterSlideToSide()
	{
		DoArmsMove(States.SlideToSide);
		wholeTargetPosition = new Vector3(rightXMaxPosition, transform.localPosition.y);
	}

	protected virtual void UpdateSlideToSide(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.ChargeSlide);
		}

		currentTimer -= d;
	}

	//CHARGESLIDE========================================================
	protected virtual void EnterChargeSlide()
	{
		DoArmsMove(States.ChargeSlide);
	}

	protected virtual void UpdateChargeSlide(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.SlideSaw);
		}

		currentTimer -= d;
	}

	//SLIDESAW========================================================
	protected virtual void EnterSlideSaw()
	{
		DoArmsMove(States.SlideSaw);

		wholeTargetPosition = new Vector3(leftXMaxPosition, transform.localPosition.y);
	}

	protected virtual void UpdateSlideSaw(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.FollowPlayer);
		}

		currentTimer -= d;
	}

	//SLIDETOCENTER========================================================
	protected virtual void EnterSlideToCenter()
	{

	}

	protected virtual void UpdateSlideToCenter(float d)
	{

	}

	//DEATH========================================================
	protected virtual void EnterDeath()
	{

	}

	protected virtual void UpdateDeath(float d)
	{

	}

	//SLEEP========================================================
	protected virtual void EnterSleep()
	{

	}

	protected virtual void UpdateSleep(float d)
	{

	}

	//AWAKENING========================================================
	protected virtual void EnterAwakening()
	{

	}

	protected virtual void UpdateAwakening(float d)
	{

	}

	//CHARGEATTACKBOTHHAND========================================================
	protected virtual void EnterChargeAttackBothHand()
	{
		DoArmsMove(States.ChargeAttackBothHand);
	}

	protected virtual void UpdateChargeAttackBothHand(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.AttackBothHand);
		}

		wholeTargetPosition = GetPlayerDelta();

		currentTimer -= d;
	}

	//SLIDEBOTHHANDSTOGETHER================================================
	protected virtual void EnterSlideBothHandsTogether()
	{
		DoArmsMove(States.SlideBothHandsTogether);
	}

	protected virtual void UpdateSlideBothHandsTogether(float d)
	{
		if (currentTimer <= 0)
		{
			fsm.ChangeState(States.FollowPlayer);
		}

		currentTimer -= d;
	}

	//ATTACKBOTHHAND========================================================
	protected virtual void EnterAttackBothHand()
	{
		DoArmsMove(States.AttackBothHand);
	}

	protected virtual void UpdateAttackBothHand(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.SlideBothHandsTogether);
		}

		currentTimer -= d;
	}

	//DEAD========================================================
	protected virtual void EnterDead()
	{

	}

	protected virtual void UpdateDead(float d)
	{

	}

	#endregion states

//#if UNITY_EDITOR
//	private void OnDrawGizmos()
//	{
//		if ((int)stateToPreview > positions.Count ||!writePositions) return;

//		Gizmos.color = Color.cyan;
//		Gizmos.DrawSphere(leftArmTargetParent.position + positions[(int)stateToPreview].leftArm, 0.35f);
//		Gizmos.color = Color.magenta;
//		Gizmos.DrawSphere(leftArmTargetParent.position + positions[(int)stateToPreview].rightArm, 0.35f);
//	}
//#endif
}
