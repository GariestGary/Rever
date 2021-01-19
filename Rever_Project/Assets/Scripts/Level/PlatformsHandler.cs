using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformsHandler : MonoCached
{
    [SerializeField] private SwitcherWrapper switcher;
	[SerializeField] private bool enableAtStart;
	
	private List<PlatformController> platforms = new List<PlatformController>();

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
			if (switcher)
			{
				foreach (var platform in platforms)
				{
					platform.Disable();
					switcher.Enabled += platform.Enable;
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

	private void OnDisable()
	{
		if(switcher)
		{
			foreach (var platform in platforms)
			{
				switcher.Enabled -= platform.Enable;
			}
		}
	}
}
