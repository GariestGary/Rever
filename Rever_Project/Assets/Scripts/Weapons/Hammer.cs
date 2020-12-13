using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RedBlueGames.LiteFSM;
using UnityEngine.SceneManagement;

public class Hammer : MonoBehaviour, ILateTick, IAwake
{
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private Transform colliderTransform;
    [SerializeField] private Collider2D colliderHammer;
	[SerializeField] private Vector3 positionOffset;
	[SerializeField] private float followSmoothTime;
    [Space]
    [SerializeField] private string playerTag;
	[Space]
	[SerializeField] private float shakeStrength;
	[SerializeField] private int shakeVibrato;
	[SerializeField] private Vector3 shakeRotationStrength;
	[SerializeField] private float shakeDuration;
	[SerializeField] private float shakeRandom;
	[Space]
	[SerializeField] private float throwDistance;
	[SerializeField] private float throwDuration;
	[SerializeField] private float returnDuration;

	private enum HammerStates
	{
		IDLE_LAY,
		PICKING_UP,
		IDLE_PICKED,
		THROWING,
	}

    private Player player;
    private Controller2D playerController;
    private Transform playerTransform;

	private Sequence shaker;
	private Sequence rotator;
	private Sequence pickup;

	private StateMachine<HammerStates> fsm;
	private HammerStates currentState;
	private Transform t;
	private Vector3 currentVelocity;
	private bool picked = false;

	private bool process;

	public bool Process => process;

	public void OnAwake()
	{
		Debug.Log(Toolbox.GetManager<GameManager>().CurrentPlayer);
		shaker = DOTween.Sequence();
		rotator = DOTween.Sequence();
		pickup = DOTween.Sequence();

		t = transform;

		Initialize(Toolbox.GetManager<GameManager>().CurrentPlayer.transform);
		StateMachineInitialize();
	}

	private void StateMachineInitialize()
	{
		List<State<HammerStates>> states = new List<State<HammerStates>>();
		states.Add(new State<HammerStates>(HammerStates.IDLE_PICKED, Idle_Picked_Enter, null, null));
		states.Add(new State<HammerStates>(HammerStates.THROWING, Throwing_Enter, null, null));
		states.Add(new State<HammerStates>(HammerStates.IDLE_LAY, Idle_Lay_Enter, null, null));
		states.Add(new State<HammerStates>(HammerStates.PICKING_UP, PickingUp_Enter, null, null));

		fsm = new StateMachine<HammerStates>(states.ToArray(), HammerStates.IDLE_LAY);
	}

	[ContextMenu("Throw")]
	public void Throw()
	{
		if (currentState == HammerStates.THROWING) return;

		fsm.ChangeState(HammerStates.THROWING);
	}

	[ContextMenu("Pickup")]
	public void PickUp()
	{
		fsm.ChangeState(HammerStates.PICKING_UP);
	}

	public void OnFixedTick()
	{
		
	}

	public void OnLateTick()
	{
		if(picked)
		{
			t.position = Vector3.SmoothDamp(t.position, playerTransform.position + positionOffset, ref currentVelocity, followSmoothTime);
		}
	}

	public void Initialize(Transform player)
	{
        if (!player.CompareTag(playerTag))
        {
            Debug.Log("Hammer could receive a player object");
            return;
        }

        playerTransform = player;
        this.player = player.GetComponent<Player>();
        playerController = player.GetComponent<Controller2D>();

		colliderHammer.enabled = false;

		shaker.Join(colliderTransform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandom, false, false))
			.Join(colliderTransform.DOShakeRotation(shakeDuration, shakeRotationStrength, shakeVibrato, shakeRandom, false)).SetLoops(-1).SetAutoKill(false).SetRecyclable(true);

		rotator.Join(pivotPoint.DOLocalRotate(new Vector3(0, 0, -719), throwDuration, RotateMode.LocalAxisAdd).SetEase(Ease.InOutCubic))
			//.Join(pivotPoint.DOLocalMove(new Vector3(playerController.LastInputFacing.x * throwDistance, playerController.LastInputFacing.y * throwDistance, 0), throwDuration).SetEase(Ease.OutSine))
			//.Append(pivotPoint.DOLocalMove(Vector3.zero, throwDuration).SetEase(Ease.InSine))
			.Append(pivotPoint.DOLocalRotate(new Vector3(0, 0, 0), returnDuration).SetEase(Ease.OutSine)).SetAutoKill(false).SetRecyclable(true).onComplete += () => { fsm.ChangeState(HammerStates.IDLE_PICKED); };

		pickup.Join(transform.DOShakeRotation(3, new Vector3(0, 0, 15), shakeVibrato, shakeRandom, true))
			.Join(transform.DOMoveY(transform.position.y + 2, 3).SetEase(Ease.InOutCubic))
			.Join(transform.DORotate(Vector3.zero, 3).SetEase(Ease.InOutQuad)).SetAutoKill(false).SetRecyclable(true).onComplete += () => { fsm.ChangeState(HammerStates.IDLE_PICKED); picked = true; };
	}

	#region States
	private void Idle_Picked_Enter()
	{
		currentState = HammerStates.IDLE_PICKED;
		rotator.Pause();
		shaker.Restart();
		colliderHammer.enabled = false;
	}

	private void Throwing_Enter()
	{
		currentState = HammerStates.THROWING;

		pivotPoint.DOLocalMove(new Vector3(playerController.LastInputFacing.x * throwDistance, playerController.LastInputFacing.y * throwDistance, 0), throwDuration).SetEase(Ease.OutSine)
			.onComplete += () => { pivotPoint.DOLocalMove(Vector3.zero, returnDuration).SetEase(Ease.InSine); };

		rotator.Restart();
		shaker.Pause();
		colliderHammer.enabled = true;
	}

	private void Idle_Lay_Enter()
	{
		currentState = HammerStates.IDLE_LAY;
		shaker.Pause();
		rotator.Pause();
		pickup.Pause();
	}

	private void PickingUp_Enter()
	{
		currentState = HammerStates.PICKING_UP;
		SceneManager.MoveGameObjectToScene(gameObject, Toolbox.GetManager<GameManager>().MainScene);
		
		player.OnAttackPressed += Throw;
		//Vector3 initPos = pivotPoint.position; //save pivot init pos
		//transform.parent = playerTransform; //setting hammer parent to player and resetting position
		//transform.localPosition = Vector3.zero;
		//pivotPoint.position = initPos; //set pivot point to init pos;

		pickup.Restart();
	}
	#endregion

	private void OnEnable()
	{
		process = true;
	}

	private void OnDisable()
	{
		if(player)
		{
			player.OnAttackPressed -= Throw;
		}

		process = false;
	}

}
