using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformsHandler : MonoBehaviour, IAwake
{
    [SerializeField] private SwitcherWrapper switcher;
    [SerializeField] private List<PlatformController> platforms = new List<PlatformController>();

	public void OnAwake()
	{
		foreach (var platform in platforms)
		{
			platform.Disable();
			switcher.Enabled += platform.Enable;
		}
	}

	private void OnDisable()
	{
		foreach (var platform in platforms)
		{
			switcher.Enabled -= platform.Enable;
		}
	}
}
