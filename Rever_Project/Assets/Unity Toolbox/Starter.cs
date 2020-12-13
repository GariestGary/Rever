using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

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

		Toolbox.GetManager<MessageManager>()?.Broker.Receive<MessageBase>().Where(x => x.id == ServiceShareData.SCENE_CHANGE).Subscribe(_ => OnSceneChange()).AddTo(Toolbox.Instance.Disposables);
	}

	[ContextMenu("Apply Framerate")]
	public void ApplyFramerate()
	{
		Application.targetFrameRate = targetFrameRate;
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
