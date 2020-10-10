using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Toolbox/Managers/Game Scene Manager", fileName = "Game Scene")]
public class GameSceneManager : ManagerBase
{
    public void AddScene(string name)
	{
		bool loaded = false;

		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			if(SceneManager.GetSceneAt(i) == SceneManager.GetSceneByName(name))
			{
				loaded = true;
				break;
			}
		}

		if(!loaded)
		{
			SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
		}
	}

	public void RemoveScene(string name)
	{
		SceneManager.UnloadSceneAsync(name);
	}
}
