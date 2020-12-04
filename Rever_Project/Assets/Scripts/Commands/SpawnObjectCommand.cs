using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Toolbox/Commands/Spawn Object Command", fileName = "Spawn Object Command")]
public class SpawnObjectCommand : DefaultCommand
{
	public override CommandProcessedMessage Process(string[] args)
	{
		if(args.Length != 8 && args.Length != 2)
		{
			return CommandProcessedMessage.Send(false, "not enough arguments, must have [pool_tag] [item_tag] [x_position] [y_position] [z_position] [x_rotation] [y_rotation] [z_rotation]");
		}

		Vector3 pos;
		Vector3 rot;
		GameObject objToSpawn;

		if(args.Length == 2)
		{
			pos = Vector3.zero;
			rot = Vector3.zero;
		}
		else
		{
			if (!(float.TryParse(args[2], out pos.x) &&
			 float.TryParse(args[3], out pos.y) &&
			 float.TryParse(args[4], out pos.z) &&
			 float.TryParse(args[5], out rot.x) &&
			 float.TryParse(args[6], out rot.y) &&
			 float.TryParse(args[7], out rot.z)))
			{
				return CommandProcessedMessage.Send(false, "invalid arguments, must have [pool_tag] [item_tag] [x_position] [y_position] [z_position] [x_rotation] [y_rotation] [z_rotation]");
			}
		}

		//Item item = Toolbox.GetManager<ItemManager>().GetItemByName(args[1]);

		//if(!item)
		//{
		//	return CommandProcessedMessage.Send(false, "Item with this name doesn't exist");
		//}

		objToSpawn = Toolbox.GetManager<ObjectPoolManager>().TryGetObject(args[0], pos, Quaternion.Euler(rot), null, null);

		if (!objToSpawn)
		{
			return CommandProcessedMessage.Send(false, "pool with this tag doesn't exist");
		}

		return CommandProcessedMessage.Send(true, "item with tag " + args[0] + " and item " + args[1] + " spawned at " + pos.ToString() + " with rotation " + rot.ToString());

	}
}
