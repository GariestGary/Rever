using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEditor;
using UnityEngine;

public sealed class Toolbox : Singleton<Toolbox>, ISceneChange
{
    public Toolbox()
	{
        destroyOnLoad = true;
	}

    private Dictionary<Type, object> managers = new Dictionary<Type, object>();
    private CompositeDisposable disposables = new CompositeDisposable();

    public CompositeDisposable Disposables => disposables;

    public static void AddManager(object obj)
    {
        if (obj is ManagerBase)
        {
            var add = obj as ManagerBase;
            Instance.managers.Add(obj.GetType(), add);

            if (obj is IExecute)
            {
                (obj as IExecute).OnExecute();
            }
        }
    }

    public static T GetManager<T>()
    {
        Instance.managers.TryGetValue(typeof(T), out object obj);
        return (T)obj;
    }

    public static bool TryGetManager<T>(out T type)
    {
        Instance.managers.TryGetValue(typeof(T), out object obj);

        type = (T)obj;

        return obj != null;
    }

    public static void AddDisposable(IDisposable disposable)
    {
        if (disposable == null) return;

        Instance.disposables.Add(disposable);
    }

    public static void Dispose(IDisposable disposable)
    {
        if (disposable == null && Instance == null) return;

        Instance.disposables.Remove(disposable);
        disposable.Dispose();
    }

    public static void ClearAll()
    {
        Instance.disposables.Dispose();
        Instance.OnSceneChange();
        Instance.managers.Clear();
        DOTween.KillAll();
    }

	private void OnApplicationQuit()
	{
        ClearAll();
	}

	public void OnSceneChange()
	{
        foreach (KeyValuePair<Type, object> entry in Instance.managers)
        {
            if (entry.Value is ISceneChange)
            {
                (entry.Value as ISceneChange).OnSceneChange();
            }
        }
    }
}
