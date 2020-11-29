using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
	[SerializeField] private string itemTag;
    private List<Item> items = new List<Item>();

    public void AddItem(Item item)
	{
		if(items.Where(x => x.Type == item.Type).Count() > 0)
		{
			//Add to existing
		}
		else
		{
			items.Add(item);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag(itemTag))
		{
			AddItem(collision.GetComponent<WorldItem>().Take());
		}
	}
}
