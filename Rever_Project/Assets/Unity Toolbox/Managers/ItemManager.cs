using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Toolbox/Managers/Item Manager", fileName = "Item")]
public class ItemManager : ManagerBase, IExecute
{
	[SerializeField] private List<ScriptableObject> items = new List<ScriptableObject>();

	public void OnExecute()
	{
		
	}

	public Item GetItemByName(string name)
	{
		return items.Where(x => x is Item && (x as Item).ItemName == name).Cast<Item>().FirstOrDefault();
	}
}
