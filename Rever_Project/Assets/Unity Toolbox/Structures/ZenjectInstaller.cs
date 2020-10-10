using UnityEngine;
using System;
using System.Collections.Generic;
using Zenject;

public class ZenjectInstaller : MonoInstaller
{
	[SerializeField] private Starter starter;

	public override void InstallBindings()
	{
		var mngs = starter.GetManagersInstances();

		Container.BindInstances(mngs);  

		foreach (var mng in mngs)
		{
			Container.QueueForInject(mng);
		}
	}
}