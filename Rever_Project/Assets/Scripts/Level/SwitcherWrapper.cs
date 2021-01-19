using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitcherWrapper : MonoCached, ISwitcher
{
	public event Action Enabled;

	protected void InvokeEnable()
	{
		Enabled?.Invoke();
	}
}
