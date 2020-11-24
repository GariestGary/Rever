using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Toolbox/Commands/Hit Command", fileName = "Hit Command")]
public class HitCharacterCommand : DefaultCommand
{
	public override bool Process(string[] args)
	{
		Debug.Log("processing hit command, args: " + string.Join(" ", args));

		int hitAmount = 0;

		if (args.Length > 2)
		{
			Debug.Log("Too many arguments");
			return false;
		}

		if(!int.TryParse(args[0], out hitAmount))
		{
			Debug.Log("Second parameter of this command must contain a hit value");
			return false;
		}

		if(hitAmount != 0)
		{
			Toolbox.GetManager<MessageManager>()?.Send(ServiceShareData.HIT_CHARACTER, this, hitAmount, args[1]);
		}
		
		return true;
	}
}
