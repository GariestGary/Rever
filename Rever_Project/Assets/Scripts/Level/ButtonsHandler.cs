using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsHandler : Saveable, ISwitcher
{
    [SerializeField] private List<Button> buttons = new List<Button>();

	private Dictionary<int, bool> buttonStates = new Dictionary<int, bool>();

	public event Action OnEnable;

	public override void Rise()
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
			OnEnable?.Invoke();
		}
	}

	public override object CaptureState()
	{
		buttonStates.Clear();

		for (int i = 0; i < buttons.Count; i++)
		{
			buttonStates.Add(i, buttons[i].Pressed);
		}

		return buttonStates;
	}

	public override void RestoreState(string state)
	{
		buttonStates = JsonConvert.DeserializeObject<Dictionary<int, bool>>(state);

		for (int i = 0; i < buttons.Count; i++)
		{
			if(buttonStates.ContainsKey(i))
			{
				if(buttonStates[i])
				{
					buttons[i].Press();
				}
				else
				{
					buttons[i].Release();
				}
			}
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
