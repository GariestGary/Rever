using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[CreateAssetMenu(menuName = "Toolbox/Managers/Save Manager")]
public class SaveManager : ManagerBase, IExecute
{
	[SerializeField] private string saveFolder = "";
	[SerializeField] private string saveFileName = "";

	public void OnExecute()
	{
		if(saveFolder == "")
		{
			saveFolder = Application.persistentDataPath;
		}
	}

	public void SetSaveFolder(string path)
	{
		saveFolder = path;
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
		RestoreState(state);
	}

	private JObject CaptureState()
	{
		JObject data = new JObject();

		var objects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

		foreach (var saveable in objects)
		{
			
			var dataToCapture = JsonConvert.SerializeObject(saveable.CaptureState());

			data.Add(saveable.ID, dataToCapture);
		}

		return data;
	}

	public void RestoreState(JObject data)
	{
		var objects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

		foreach (var restoreData in data)
		{
			var keyObject = objects.Where(x => x.ID == restoreData.Key);

			if(keyObject.Count() > 1)
			{
				throw new System.Exception("Data restore error: two or more objects with same identifier");
			}
			else if(keyObject.Count() > 0)
			{
				JObject dataObject = JsonConvert.DeserializeObject<JObject>((string)restoreData.Value);

				keyObject.First().RestoreState(dataObject);
			}
		}
	}

	private void SaveJsonFile(JObject data)
	{
		string filePath = saveFolder + "\\" + saveFileName + ".txt";

		if (!File.Exists(filePath))
		{
			File.Create(filePath).Dispose();
		}

		File.WriteAllText(filePath, JsonConvert.SerializeObject(data));
	}

	private JObject LoadJsonFile()
	{
		string filePath = saveFolder + "\\" + saveFileName + ".txt";

		if(!File.Exists(filePath))
		{
			return new JObject();
		}

	 	return (JObject)JsonConvert.DeserializeObject(File.ReadAllText(filePath));
	}

}
