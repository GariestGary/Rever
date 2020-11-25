using UnityEngine;
﻿using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Toolbox/Commands/Log Command", fileName = "Log Command")]
public class LogCommand : DefaultCommand
{
	public override CommandProcessedMessage Process(string[] args)
	{
		return CommandProcessedMessage.Send(true, string.Join(" ", args));
	}
}
