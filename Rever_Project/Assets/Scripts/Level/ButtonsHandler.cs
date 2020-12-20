using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsHandler : SwitcherWrapper, IAwake
{
    [SerializeField] private List<Button> buttons = new List<Button>();

	public void OnAwake()
	{
		foreach (var button in buttons)
		{
			button.OnPress += ButtonCheck;
		}
	}

	private void ButtonCheck()
	{
		bool pressedAll = true;

		foreach (var button in buttons)
		{
			if(!button.Pressed)
			{
				pressedAll = false;
			}

			if(button.StayingObjectsCount <= 0)
			{
				button.Release();
			}
		}

		if(pressedAll)
		{
			InvokeEnable();
		}
	}

	private void OnDisable()
	{
		foreach (var button in buttons)
		{
			button.OnPress -= ButtonCheck;
		}
	}
}
