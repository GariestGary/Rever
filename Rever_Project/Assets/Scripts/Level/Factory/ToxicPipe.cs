using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using DG.Tweening;

public class ToxicPipe : Saveable
{
	[SerializeField] private SpriteRenderer renderer;
	[SerializeField] private Sprite brokenPipe;
	[SerializeField] private string shellPoolTag;
	[SerializeField] private float restoreTime;
	[Header("Right")]
	[SerializeField] private Transform rightShootPoint;
	[SerializeField] private MonoBehaviour rightSwitcher;
	[SerializeField] private SpriteRenderer rightShooterFill;
	[SerializeField] private ShootInfo rightShootInfo;
	[Header("Left")]
	[SerializeField] private Transform leftShootPoint;
	[SerializeField] private MonoBehaviour leftSwitcher;
	[SerializeField] private SpriteRenderer leftShooterFill;
	[SerializeField] private ShootInfo leftShootInfo;

	public UnityEvent PipeHitEvent;

	private MessageManager msg;
	private ObjectPoolManager pool;
	
	private bool broken = false;
	private bool rightShooted = false;
	private bool leftShooted = false;


	[Inject]
    public void Constructor(MessageManager msg, ObjectPoolManager pool)
	{
		this.msg = msg;
		this.pool = pool;
	}

	public override void Rise()
	{
		if(rightSwitcher)
		{
			(rightSwitcher as ISwitcher).EnableEvent.AddListener(ShootRightPipe);
		}

		if (leftSwitcher)
		{
			(leftSwitcher as ISwitcher).EnableEvent.AddListener(ShootLeftPipe);
		}

		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.FACTORY_HIT_PIPE).Subscribe(_ => HitPipe()).AddTo(LevelHandler.Instance.LevelDisposables);
	}

	public override object CaptureState()
	{
		return broken;
	}

	public override void RestoreState(string state)
	{
		broken = JsonConvert.DeserializeObject<bool>(state);

		if(broken)
		{
			renderer.sprite = brokenPipe;
		}
	}

	public void ShootRightPipe()
	{
		if (rightShooted || !broken) return;

		StartCoroutine(ShootRightPipeCoroutine());
	}

	private IEnumerator ShootRightPipeCoroutine()
	{
		rightShooted = true;
		rightShooterFill.DOFade(0, 0.2f);
		pool.Spawn(shellPoolTag, rightShootPoint.position, Quaternion.identity, null, rightShootInfo);
		yield return new WaitForSeconds(1);
		rightShooterFill.DOFade(1, restoreTime);
		yield return new WaitForSeconds(restoreTime);
		rightShooted = false;
	}

	public void ShootLeftPipe()
	{
		if (leftShooted || !broken) return;

		StartCoroutine(ShootLeftPipeCoroutine());
	}

	private IEnumerator ShootLeftPipeCoroutine()
	{
		leftShooted = true;
		leftShooterFill.DOFade(0, 0.2f);
		pool.Spawn(shellPoolTag, leftShootPoint.position, Quaternion.identity, null, leftShootInfo);
		yield return new WaitForSeconds(1);
		leftShooterFill.DOFade(1, restoreTime);
		yield return new WaitForSeconds(restoreTime);
		leftShooted = false;
	}

	private void HitPipe()
	{
		if (broken) return;

		broken = true;
		renderer.sprite = brokenPipe;
		PipeHitEvent?.Invoke();
	}
}
