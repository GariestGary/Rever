using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Toolbox/Commands/Item Notify Command", fileName = "Item Notify Command")]
public class ItemNotifierCommand : DefaultCommand
{
	public override CommandProcessedMessage Process(string[] args)
	{
		if (args.Length > 1) return CommandProcessedMessage.Send(false, "too many arguments");
		//TODO: if item doesn't exist
		Toolbox.GetManager<MessageManager>().Send(ServiceShareData.ITEM_NOTIFY, this, Toolbox.GetManager<ItemManager>().GetItemByName(args[0]));
		return CommandProcessedMessage.Send(true, "Item notified");
	}
}
