using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour, IPooledObject, IAwake
{
	[SerializeField] private SpriteRenderer sprite;

	private Item currentItem;

	public void OnAwake()
	{
		SetItem(Item.GetMock());
	}

	public void OnSpawn(object data)
	{
		SetItem((Item)data);
	}

	public void SetItem(Item itemToSet)
	{
		currentItem = itemToSet;

		sprite.sprite = currentItem.IconInWorld;
	}
}
