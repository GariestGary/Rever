using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class SaveableEntity : MonoBehaviour, ISaveable
{
	[SerializeField] private string id = string.Empty;

	[ContextMenu("Generate ID")]
	public void GenerateID() => id = Guid.NewGuid().ToString();
	public string ID => id;

	[SerializeField] private int lvl = 0;
	[SerializeField] private float exp = 0;
	[SerializeField] private string msg = "";

	public object CaptureState()
	{
		return new SaveData { lvl = this.lvl, exp = this.exp, msg = this.msg };
	}

	public void RestoreState(JObject state)
	{

		var data = state.ToObject<SaveData>();

		lvl = data.lvl;
		exp = data.exp;
		msg = data.msg;
	}

	[Serializable]
	struct SaveData
	{
		public int lvl;
		public float exp;
		public string msg;
	}
}
