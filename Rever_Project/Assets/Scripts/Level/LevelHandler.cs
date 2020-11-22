using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class LevelHandler : Singleton<LevelHandler>
{
    public LevelHandler()
	{
        destroyOnLoad = true;
	}

    private CompositeDisposable levelDisposables = new CompositeDisposable();

    public CompositeDisposable LevelDisposables => levelDisposables;

    public void Dispose()
	{
        levelDisposables.Dispose();
	}
}
