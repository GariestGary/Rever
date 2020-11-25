using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Toolbox/Commands/Reset Player Command", fileName = "Reset Player Command")]
public class ResetPlayerCommand : DefaultCommand
{
	public override CommandProcessedMessage Process(string[] args)
	{
		Toolbox.GetManager<GameManager>().ResetPlayer();
		return CommandProcessedMessage.Send(true, "Player resetted");
	}
}
