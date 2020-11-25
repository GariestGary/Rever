using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Toolbox/Commands/Hit Command", fileName = "Hit Command")]
public class HitCharacterCommand : DefaultCommand
{
	public override CommandProcessedMessage Process(string[] args)
	{
		int hitAmount = 0;

		if (args.Length > 2)
		{
			return CommandProcessedMessage.Send(false, "Too many arguments");
		}

		if(!int.TryParse(args[0], out hitAmount))
		{
			return CommandProcessedMessage.Send(false, "Second parameter of this command must contain a hit value");
		}

		if(hitAmount != 0)
		{
			MessageManager msg = Toolbox.GetManager<MessageManager>();

			switch (args[1])
			{
				case "player": msg?.Send(ServiceShareData.HIT_CHARACTER, this, hitAmount, args[1]); break;
			}
			
		}
		
		return CommandProcessedMessage.Send(true, "Character " + args[1] + " dealed " + args[0] + " damage");
	}
}
