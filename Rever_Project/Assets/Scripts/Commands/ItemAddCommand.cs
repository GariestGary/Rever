using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Toolbox/Commands/Item Notify Command", fileName = "Item Notify Command")]
public class ItemAddCommand : DefaultCommand
{
	public override CommandProcessedMessage Process(string[] args)
	{
		//if (args.Length > 1) return CommandProcessedMessage.Send(false, "too many arguments");
		////TODO: if item doesn't exist
		//Item itemToAdd = Toolbox.GetManager<ItemManager>().GetItemByName(args[0]);

		//if (!itemToAdd) return CommandProcessedMessage.Send(false, "Item with given name doesn't exist");

		//Toolbox.GetManager<GameManager>().CurrentInventory.AddItem(itemToAdd);

		//Toolbox.GetManager<MessageManager>().Send(ServiceShareData.ITEM_TAKED, this, itemToAdd);
		return CommandProcessedMessage.Send(true, "Item added");
	}
}
