using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour, IPooledObject, IAwake
{
	[SerializeField] private SpriteRenderer sprite;

	private Item currentItem;
	private ObjectPoolManager pool;

	public void OnAwake()
	{
		SetItem(Item.GetMock());
	}

	public void OnSpawn(object data, ObjectPoolManager pool)
	{
		SetItem((Item)data);
		this.pool = pool;
	}

	public void SetItem(Item itemToSet)
	{
		currentItem = itemToSet;

		sprite.sprite = currentItem.IconInWorld;
	}

	public Item Take()
	{
		Debug.Log("Taked item " + currentItem.Type.ToString());
		pool.Despawn(gameObject);
		return currentItem;
	}
}
