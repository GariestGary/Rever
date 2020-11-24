using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour, IAwake
{
	[SerializeField] private Image health;

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
			Debug.Log("Updating UI");

			switch(x.tag)
			{
				case "health": UpdateHealth((HitPoints)x.data); return;
			}
		}).AddTo(Toolbox.Instance.Disposables);
	}

	private void UpdateHealth(HitPoints hp)
	{
		Debug.Log(hp.ToString());
		health.fillAmount = (float)hp.currentHitPoints / (float)hp.maxHitPoints;
	}
}
