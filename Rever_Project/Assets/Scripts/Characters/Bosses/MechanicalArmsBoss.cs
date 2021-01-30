using RedBlueGames.LiteFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using DG.Tweening;

public class MechanicalArmsBoss : Boss
{
	[BoxGroup("Right Arm")] [SerializeField] private Transform rightArmTarget;
	[BoxGroup("Right Arm")] [SerializeField] private Transform rightArmTargetParent;
	[BoxGroup("Left Arm")] [SerializeField] private Transform leftArmTarget;
	[BoxGroup("Left Arm")] [SerializeField] private Transform leftArmTargetParent;
	[Range(0, 1)]
	[SerializeField] private float noiseInfluence;
	[SerializeField] private float noiseVibrato;
	[SerializeField] private float leftXMaxPosition;
	[SerializeField] private float rightXMaxPosition;
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
	private float followSmooth;
	private bool awakened = false;
	private bool dead = false;
	private bool pipeHitted = false;
	private bool stabbed = false;
	private Vector3 wholeTargetPosition;
	private Vector3 initLeftArmTargetPos;
	private Vector3 initRightArmTargetPos;

	private Tween leftTween;
	private Tween rightTween;

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
		public float followSmooth;
		public bool useCurve;
		[AllowNesting]
		[ShowIf("notUseCurve")]
		public Ease ease = Ease.InQuad;
		[AllowNesting]
		[ShowIf("useCurve")]
		[CurveRange(0, 0, 1, 1)]
		public AnimationCurve curve;
		[InfoBox("X for left arm, Y for right arm")]
		[Space]
		public Vector2 noiseInfluence;
		public bool fadeOutNoiseLeft;
		public bool fadeOutNoiseRight;
		public Vector3 leftArm;
		public Vector3 rightArm;

		private bool notUseCurve => !useCurve;

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
		SlideAttack,
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
		HitPipe,
		StabSelf,
		TakeOutHammer,
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

	public void SetState(int state)
	{
		fsm.ChangeState((States)state);
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

		float duration = Random.Range(currentPosition.minDuration, currentPosition.maxDuration);

		leftArmTarget.DOKill();
		rightArmTarget.DOKill();

		if(currentPosition.useCurve)
		{
			rightArmTarget.DOLocalMove(right, duration).SetEase(currentPosition.curve);
			leftArmTarget.DOLocalMove(left, duration).SetEase(currentPosition.curve);
		}
		else
		{
			rightArmTarget.DOLocalMove(right, duration).SetEase(currentPosition.ease);
			leftArmTarget.DOLocalMove(left, duration).SetEase(currentPosition.ease);
		}

		currentTimer = duration + currentPosition.additionalTime;
		leftArmTargetParent.DOKill();
		rightArmTargetParent.DOKill();
		leftArmTargetParent.DOShakePosition(currentTimer, noiseInfluence * currentPosition.noiseInfluence.x, (int)(noiseVibrato * currentPosition.noiseInfluence.x), 90, false, currentPosition.fadeOutNoiseLeft);
		rightArmTargetParent.DOShakePosition(currentTimer, noiseInfluence * currentPosition.noiseInfluence.y, (int)(noiseVibrato * currentPosition.noiseInfluence.y), 90, false, currentPosition.fadeOutNoiseRight);

		followSmooth = currentPosition.followSmooth;
	}

	public override void Tick()
	{
		base.Tick();

		if(!dead)
		{
			fsm.Update(Time.deltaTime);
			WholePositionUpdate();
		}
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
		states.Add(new State<States>(States.SlideAttack, EnterSlideAttack, null, UpdateSlideAttack)); 
		states.Add(new State<States>(States.SlideToCenter, EnterSlideToCenter, null, UpdateSlideToCenter));
		states.Add(new State<States>(States.Death, EnterDeath, null, UpdateDeath));
		states.Add(new State<States>(States.Sleep, EnterSleep, null, UpdateSleep));
		states.Add(new State<States>(States.Awakening, EnterAwakening, ExitAwakening, UpdateAwakening));
		states.Add(new State<States>(States.ChargeAttackBothHand, EnterChargeAttackBothHand, null, UpdateChargeAttackBothHand));
		states.Add(new State<States>(States.AttackBothHand, EnterAttackBothHand, null, UpdateAttackBothHand));
		states.Add(new State<States>(States.SlideBothHandsTogether, EnterSlideBothHandsTogether, null, UpdateSlideBothHandsTogether));
		states.Add(new State<States>(States.Dead, EnterDead, null, UpdateDead));
		states.Add(new State<States>(States.HitPipe, EnterHitPipe, null, UpdateHitPipe));
		states.Add(new State<States>(States.StabSelf, EnterStabSelf, null, UpdateStabSelf));
		states.Add(new State<States>(States.TakeOutHammer, EnterTakeOutHammer, null, UpdateTakeOutHammer));
	
		fsm = new StateMachine<States>(states.ToArray(), States.Sleep);
	}

	#region states

	//STAB SELF=====================================================
	protected virtual void EnterStabSelf()
	{
		DoArmsMove(States.StabSelf);
	}

	protected virtual void UpdateStabSelf(float d)
	{
		if(currentTimer <= currentPosition.additionalTime && !stabbed)
		{
			stabbed = true;
			//TODO: particles
		}

		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.TakeOutHammer);
			return;
		}

		currentTimer -= d;
	}

	//TAKE OUT HAMMER=====================================================
	protected virtual void EnterTakeOutHammer()
	{
		DoArmsMove(States.TakeOutHammer);
		pool.Spawn("Ability Provider", leftArmTarget.position, Quaternion.identity, null, AbilityType.SLASH);
	}

	protected virtual void UpdateTakeOutHammer(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.Dead);
		}

		currentTimer -= d;
	}

	//HIT PIPE=====================================================
	protected virtual void EnterHitPipe()
	{
		DoArmsMove(States.HitPipe);

		pipeHitted = false;
	}

	protected virtual void UpdateHitPipe(float d)
	{
		if(currentTimer <= currentPosition.additionalTime && !pipeHitted)
		{
			msg.Send(ServiceShareData.FACTORY_HIT_PIPE);
			pipeHitted = true;
		}

		if(currentTimer <= 0)
		{
			awakened = true;
			fsm.ChangeState(States.Idle);
			return;
		}

		currentTimer -= d;
	}

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
			return;
		}

		currentTimer -= d;
	}

	//DAMAGED=========================================================
	protected virtual void EnterDamaged()
	{
		wholeTargetPosition = Vector3.zero;
		DoArmsMove(States.Damaged);
	}

	protected virtual void UpdateDamaged(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.Idle);
			return;
		}

		currentTimer -= d;
	}

	//FOLLOW PLAYER========================================================
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

	//CHARGE ATTACK ONE HAND========================================================
	protected virtual void EnterChargeAttackOneHand()
	{
		DoArmsMove(States.ChargeAttackOneHand);
	}

	protected virtual void UpdateChargeAttackOneHand(float d)
	{
		if (currentTimer <= 0)
		{
			fsm.ChangeState(States.AttackOneHand);
			return;
		}

		wholeTargetPosition = GetPlayerDelta();

		currentTimer -= d;
	}

	//ATTACK ONE HAND========================================================
	protected virtual void EnterAttackOneHand()
	{
		DoArmsMove(States.AttackOneHand, true, (game.CurrentPlayer.transform.position - leftArmTargetParent.position).x / 3.5f);
	}

	protected virtual void UpdateAttackOneHand(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.FollowPlayer);
			return;
		}

		currentTimer -= d;
	}

	//SLIDE TO SIDE========================================================
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
			return;
		}

		currentTimer -= d;
	}

	//CHARGE SLIDE========================================================
	protected virtual void EnterChargeSlide()
	{
		DoArmsMove(States.ChargeSlide);
	}

	protected virtual void UpdateChargeSlide(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.SlideAttack);
			return;
		}

		currentTimer -= d;
	}

	//SLIDE ATTACK===================================================
	protected virtual void EnterSlideAttack()
	{
		DoArmsMove(States.SlideAttack);
	}

	protected virtual void UpdateSlideAttack(float d)
	{
		if (currentTimer <= 0)
		{
			fsm.ChangeState(States.SlideSaw);
			return;
		}

		currentTimer -= d;
	}

	//SLIDE SAW========================================================
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
			return;
		}

		currentTimer -= d;
	}

	//SLIDE TO CENTER========================================================
	protected virtual void EnterSlideToCenter()
	{

	}

	protected virtual void UpdateSlideToCenter(float d)
	{

	}

	//DEATH========================================================
	protected virtual void EnterDeath()
	{
		wholeTargetPosition = Vector3.zero;
		DoArmsMove(States.Death);
		//TODO: disable colliders and save
	}

	protected virtual void UpdateDeath(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.StabSelf);
			return;
		}

		currentTimer -= d;
	}

	//SLEEP========================================================
	protected virtual void EnterSleep()
	{
		currentPosition = positions[(int)States.Sleep];

		leftArmTarget.localPosition = currentPosition.leftArm;
		rightArmTarget.localPosition = currentPosition.rightArm;

		followSmooth = currentPosition.followSmooth;
	}

	protected virtual void UpdateSleep(float d)
	{
		
	}

	//AWAKENING========================================================
	protected virtual void EnterAwakening()
	{
		DoArmsMove(States.Awakening);
	}

	protected virtual void UpdateAwakening(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.HitPipe);
			return;
		}

		currentTimer -= d;
	}

	protected virtual void ExitAwakening()
	{
		arena.EnablePlayer();
	}

	//CHARGE ATTACK BOTH HAND========================================================
	protected virtual void EnterChargeAttackBothHand()
	{
		DoArmsMove(States.ChargeAttackBothHand);
	}

	protected virtual void UpdateChargeAttackBothHand(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.AttackBothHand);
			return;
		}

		wholeTargetPosition = GetPlayerDelta();

		currentTimer -= d;
	}

	//SLIDE BOTH HANDS TOGETHER================================================
	protected virtual void EnterSlideBothHandsTogether()
	{
		DoArmsMove(States.SlideBothHandsTogether);
	}

	protected virtual void UpdateSlideBothHandsTogether(float d)
	{
		if (currentTimer <= 0)
		{
			fsm.ChangeState(States.FollowPlayer);
			return;
		}

		currentTimer -= d;
	}

	//ATTACK BOTH HAND========================================================
	protected virtual void EnterAttackBothHand()
	{
		DoArmsMove(States.AttackBothHand);
	}

	protected virtual void UpdateAttackBothHand(float d)
	{
		if(currentTimer <= 0)
		{
			fsm.ChangeState(States.SlideBothHandsTogether);
			return;
		}

		currentTimer -= d;
	}

	//DEAD========================================================
	protected virtual void EnterDead()
	{
		wholeTargetPosition = Vector3.zero;
		DoArmsMove(States.Dead);
	}

	protected virtual void UpdateDead(float d)
	{
		if(currentTimer <= 0)
		{
			dead = true;
			msg.Send(ServiceShareData.BOSS_DEFEATED);
		}

		currentTimer -= d;
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
