using UnityEngine;
﻿using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Toolbox/Commands/Log Command", fileName = "Log Command")]
public class LogCommand : DefaultCommand
{
	public override bool Process(string[] args)
	{
		Debug.Log(string.Join(" ", args));
		return true;
	}
}
