using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStateUI : MonoBehaviour, IAwake
{
	[SerializeField] private Image health;
	[Header("Item Notifier")]
	[SerializeField] private RectTransform ItemNotifier;
	[SerializeField] private RectTransform ItemNotifierOutPosition;
	[SerializeField] private RectTransform ItemNotifierAppearPosition;
	[SerializeField] private Text notifierText;
	[SerializeField] private Image notifierIcon;
	[SerializeField] private float appearDuration;
	[SerializeField] private float stayDuration;
	[SerializeField] private Ease notifierEase;

	private Sequence notifierSequence;
	private MessageManager msg;

	[Inject]
	public void Constructor(MessageManager msg)
	{
		this.msg = msg;
	}

	public void OnAwake()
	{
		msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.UPDATE_UI).Subscribe(x => 
		{
			switch(x.tag)
			{
				case "health": UpdateHealth((HitPoints)x.data); return;
			}
		}).AddTo(Toolbox.Instance.Disposables);

		//msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.ITEM_TAKED).Subscribe(x => { Debug.Log(x.data.GetType()); ItemNotify(x.data as Item); });

		notifierSequence = DOTween.Sequence();

		ItemNotifier.localPosition = ItemNotifierOutPosition.localPosition;
		notifierSequence.Append(ItemNotifier.DOAnchorPosX(ItemNotifierAppearPosition.position.x, appearDuration).SetEase(notifierEase)).
			Append(ItemNotifier.DOAnchorPosX(ItemNotifierAppearPosition.position.x, stayDuration)).
			Append(ItemNotifier.DOAnchorPosX(ItemNotifierOutPosition.position.x, appearDuration).SetEase(notifierEase)).SetAutoKill(false).SetRecyclable(true); ;

		notifierSequence.Pause();
	}

	private void UpdateHealth(HitPoints hp)
	{
		Debug.Log(hp.ToString());
		health.fillAmount = (float)hp.currentHitPoints / (float)hp.maxHitPoints;
	}

	//private void ItemNotify(Item item)
	//{
	//	notifierIcon.sprite = item.IconInInventory;
	//	notifierText.text = item.name;

	//	notifierSequence.Restart();
	//	//notifierSequence.Play();
	//}
}
