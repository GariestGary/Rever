using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitcherWrapper : MonoBehaviour, ISwitcher
{
	public event Action Enabled;

	protected void InvokeEnable()
	{
		Enabled?.Invoke();
	}
}
