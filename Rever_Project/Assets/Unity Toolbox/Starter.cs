using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class Starter : MonoBehaviour
{
    [SerializeField] private List<ManagerBase> managers = new List<ManagerBase>();
	[SerializeField] private int targetFrameRate = 60;

	private void Awake()
	{
//#if UNITY_EDITOR
//		Debug.unityLogger.logEnabled = true;
//#else
//		Debug.unityLogger.logEnabled = false;
//#endif

		Application.targetFrameRate = targetFrameRate;

		for (int i = 0; i < managers.Count; i++)
		{
			Toolbox.AddManager(managers[i]);
		}

		Toolbox.GetManager<MessageManager>()?.Subscribe(ServiceShareData.SCENE_CHANGE, () => OnSceneChange());
	}

	public ManagerBase[] GetManagersInstances()
	{
		return managers.ToArray();
	}

	private void OnSceneChange()
	{
		var allChanges = FindObjectsOfType(typeof(MonoBehaviour)).OfType<ISceneChange>();

		foreach (var change in allChanges)
		{
			change.OnSceneChange();
		}

		Toolbox.ClearAll();
	}
}
