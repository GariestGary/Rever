using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using Fungus;

[CreateAssetMenu(menuName = "Toolbox/Managers/Save Manager")]
public class SaveManager : ManagerBase, IExecute
{
	[SerializeField] private string saveFolder = "";
	[SerializeField] private string saveFileName = "";

	private GameData gameData;

	public void OnExecute()
	{
		if(saveFolder == "")
		{
			SetSaveFolder(Application.persistentDataPath);
		}

		gameData = new GameData();

		Debug.Log("Save folder is " + saveFolder);
	}

	public void SetSaveFolder(string path)
	{
		saveFolder = path;

		Debug.Log("Save folder set to " + path);
	}

	[ContextMenu("Save")]
	public void Save()
	{
		var state = CaptureState();
		SaveJsonFile(state);
	}

	[ContextMenu("Load")]
	public void Load()
	{
		var state = LoadJsonFile();

		if(!string.IsNullOrEmpty(state))
		{
			RestoreState(state);
		}
	}

	private Saveable[] GetAllSaveablesInScene(string sceneName)
	{
		Saveable[] saveables = new Saveable[0];

		SceneManager.GetSceneByName(sceneName).GetRootGameObjects().ToList().ForEach(x =>
		{
			Saveable[] inRoot = x.GetComponentsInChildren<Saveable>();
			if(inRoot.Length > 0)
			{
				saveables = saveables.Concat(inRoot).ToArray();
			}
		});
		return saveables;
	}

	private string CaptureState()
	{
		string sceneToSave = Toolbox.GetManager<GameManager>().CurrentSceneName;

		gameData.currentSceneName = sceneToSave;

		Saveable[] objects = GetAllSaveablesInScene(sceneToSave);
		
		if (!gameData.scenesData.ContainsKey(gameData.currentSceneName))
		{
			gameData.scenesData.Add(gameData.currentSceneName, new Dictionary<string, string>());
		}

		foreach (var saveable in objects)
		{
			string dataToCapture = JsonConvert.SerializeObject(saveable.CaptureState());

			if(gameData.scenesData[gameData.currentSceneName].ContainsKey(saveable.ID))
			{
				gameData.scenesData[gameData.currentSceneName][saveable.ID] = dataToCapture;
			}
			else
			{
				gameData.scenesData[gameData.currentSceneName].Add(saveable.ID, dataToCapture);
			}
			
		}

		objects = GetAllSaveablesInScene("Main");

		foreach (var saveable in objects)
		{
			string dataToCapture = JsonConvert.SerializeObject(saveable.CaptureState());

			if (gameData.mainData.ContainsKey(saveable.ID))
			{
				gameData.mainData[saveable.ID] = dataToCapture;
			}
			else
			{
				gameData.mainData.Add(saveable.ID, dataToCapture);
			}
		}

		return JsonConvert.SerializeObject(gameData);
	}

	public void RestoreState(string data)
	{
		string sceneToRestore = Toolbox.GetManager<GameManager>().CurrentSceneName;

		var objects = GetAllSaveablesInScene(sceneToRestore);

		gameData = JsonConvert.DeserializeObject<GameData>(data);

		if (gameData.scenesData.ContainsKey(sceneToRestore))
		{
			for (int i = 0; i < objects.Length; i++)
			{
				if(gameData.scenesData[sceneToRestore].ContainsKey(objects[i].ID))
				{
					objects[i].RestoreState(gameData.scenesData[sceneToRestore][objects[i].ID]);
				}
				else
				{
					Debug.LogWarning("object with ID " + objects[i].ID + " has no corresponding data in game data");
				}
			}
		}

		objects = GetAllSaveablesInScene("Main");

		for (int i = 0; i < objects.Length; i++)
		{
			if (gameData.mainData.ContainsKey(objects[i].ID))
			{
				objects[i].RestoreState(gameData.mainData[objects[i].ID]);
			}
			else
			{
				Debug.LogWarning("object with ID " + objects[i].ID + " has no corresponding data in game data");
			}
		}
	}

	public void LoadGameData()
	{
		gameData = JsonConvert.DeserializeObject<GameData>(LoadJsonFile());
	}

	private void SaveJsonFile(string data)
	{
		string filePath = saveFolder + "\\" + saveFileName + ".txt";

		if (!File.Exists(filePath))
		{
			File.Create(filePath).Dispose();
		}

		File.WriteAllText(filePath, data);
	}

	private string LoadJsonFile()
	{
		string filePath = saveFolder + "\\" + saveFileName + ".txt";

		if(!File.Exists(filePath))
		{
			return string.Empty;
		}

	 	return File.ReadAllText(filePath);
	}

	[System.Serializable]
	public class GameData
	{
		public Dictionary<string, Dictionary<string, string>> scenesData = new Dictionary<string, Dictionary<string, string>>();
		public Dictionary<string, string> mainData = new Dictionary<string, string>();

		public string currentSceneName;
	}
}
