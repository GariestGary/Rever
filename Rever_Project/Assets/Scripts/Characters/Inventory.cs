using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using Zenject;

public class Inventory : MonoCached
{
	[SerializeField] private string itemTag;
    //private List<Item> items = new List<Item>();

	private MessageManager msg;

	//public Item[] Items => items.ToArray();

	[Inject]
	public void Constructor(MessageManager msg)
	{
		this.msg = msg;
	}
	public void OnAwake()
	{
		//msg.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.ITEM_TAKED).Subscribe(x => AddItem(x.data as Item));
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag(itemTag))
		{
			
		}
	}

}
