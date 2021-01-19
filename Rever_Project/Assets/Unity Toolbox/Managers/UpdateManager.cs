using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Update", menuName = "Toolbox/Managers/Update Manager")]
public class UpdateManager : ManagerBase, IExecute
{
	private List<MonoCached> monos = new List<MonoCached>();

	public void Add(MonoCached monoToAdd)
	{
		if(monos.Contains(monoToAdd))
		{
			Debug.LogWarning("Mono " + monoToAdd + " already updates");
			return;
		}

		monos.Add(monoToAdd);
	}

	public void Remove(MonoCached monoToRemove)
	{
		if(!monos.Contains(monoToRemove))
		{
			Debug.LogWarning("Mono " + monoToRemove + " don't updates");
			return;
		}

		monos.Remove(monoToRemove);
	}

	public void RiseGameObject(GameObject obj)
	{
		var allMonos = obj.GetComponentsInChildren<MonoCached>();

		for (int i = 0; i < allMonos.Length; i++)
		{
			allMonos[i].Rise();
		}
	}

	public void AddGameObject(GameObject obj)
	{
		var allMonos = obj.GetComponentsInChildren<MonoCached>();

		for (int i = 0; i < allMonos.Length; i++)
		{
			Add(allMonos[i]);
		}
	}

	public void RemoveGameObject(GameObject obj)
	{
		var allMonos = obj.GetComponentsInChildren<MonoCached>();

		for (int i = 0; i < allMonos.Length; i++)
		{
			Remove(allMonos[i]);
		}
	}

	public void Tick()
	{
		for (int i = 0; i < monos.Count; i++)
		{
			monos[i].Tick();
		}
	}

	public void FixedTick()
	{
		for (int i = 0; i < monos.Count; i++)
		{
			monos[i].FixedTick();
		}
	}

	public void LateTick()
	{
		for (int i = 0; i < monos.Count; i++)
		{
			monos[i].LateTick();
		}
	}

	public void OnExecute()
	{
		monos.Clear();

		InitializeFromScene();

		UpdateManagerComponent umc = GameObject.Find("[ENTRY]").AddComponent<UpdateManagerComponent>();
		umc.enabled = true;
		umc.Setup(this);
	}

	public void InitializeFromScene()
	{
		Debug.Log("Initializing from scene");

		MonoCached[] allMonos = FindObjectsOfType<MonoCached>();

		Debug.Log("founded " + allMonos.Count() + " updateables: " + string.Join(" ", allMonos.ToList()));

		for (int i = 0; i < allMonos.Length; i++)
		{
			Add(allMonos[i]);
			RiseGameObject(allMonos[i].gameObject);
		}
	}
}
