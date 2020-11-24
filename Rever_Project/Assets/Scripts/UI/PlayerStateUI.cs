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
			switch(x.tag)
			{
				case "health": UpdateHealth((HitPoints)x.data); return;
			}
		});
	}

	private void UpdateHealth(HitPoints hp)
	{
		health.fillAmount = hp.currentHitPoints / hp.maxHitPoints;
	}
}
