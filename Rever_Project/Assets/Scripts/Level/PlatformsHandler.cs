using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformsHandler : Saveable
{
    [SerializeField] private MonoBehaviour switcher;
	[SerializeField] private bool enableAtStart;
	
	private List<PlatformController> platforms = new List<PlatformController>();

	private Dictionary<int, bool> platformStates = new Dictionary<int, bool>();

	public override void Rise()
	{
		FetchPlatforms();

		if (enableAtStart)
		{
			foreach (var platform in platforms)
			{
				platform.Enable();
			}
		}
		else
		{
			if (switcher != null)
			{
				foreach (var platform in platforms)
				{
					platform.Disable();
					(switcher as ISwitcher).EnableEvent.AddListener(platform.Enable);
				}
			}
			else
			{
				Debug.LogWarning("Switcher on " + this.name + " not attached");
			}
		}
	}

	private void FetchPlatforms()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).TryGetComponent(out PlatformController platform);
			platforms.Add(platform);
		}
	}

	public override object CaptureState()
	{
		platformStates.Clear();

		for (int i = 0; i < platforms.Count; i++)
		{
			platformStates.Add(i, platforms[i].Running);
		}

		return platformStates;
	}

	public override void RestoreState(string state)
	{
		platformStates = JsonConvert.DeserializeObject<Dictionary<int, bool>>(state);

		for (int i = 0; i < platforms.Count; i++)
		{
			if (platformStates.ContainsKey(i))
			{
				if (platformStates[i])
				{
					platforms[i].Enable();
				}
				else
				{
					platforms[i].Disable();
				}
			}
		}
	}

	private void OnDisable()
	{
		if(switcher != null)
		{
			foreach (var platform in platforms)
			{
				(switcher as ISwitcher).EnableEvent.AddListener(platform.Enable);
			}
		}
	}
}
