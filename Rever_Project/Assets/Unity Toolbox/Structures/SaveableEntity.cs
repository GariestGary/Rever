using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class SaveableEntity : Saveable
{
	[SerializeField] private SaveData data = new SaveData();

	public override object CaptureState()
	{
		return data;
	}

	public override void RestoreState(string state)
	{

		var restoreData = JsonConvert.DeserializeObject<SaveData>(state);

		data.lvl = restoreData.lvl;
		data.exp = restoreData.exp;
		data.msg = restoreData.msg;
	}

	[Serializable]
	class SaveData
	{
		public int lvl;
		public float exp;
		public string msg;
	}
}
