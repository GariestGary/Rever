using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using Zenject;

[CreateAssetMenu(fileName = "Update", menuName = "Toolbox/Managers/Update Manager")]
public class UpdateManager : ManagerBase, IExecute
{
	private List<MonoCached> monos = new List<MonoCached>();
	private DiContainer _container;

	[Inject]
	public void Constructor(DiContainer _container)
	{
		this._container = _container;
	}

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

	public void RemoveAllGameObjects(GameObject[] objects)
	{
		for (int i = 0; i < objects.Length; i++)
		{
			RemoveGameObject(objects[i]);
		}
	}

	public void PrepareAllGameObjects(GameObject[] objects)
	{
		for (int i = 0; i < objects.Length; i++)
		{
			_container.InjectGameObject(objects[i]);
			AddGameObject(objects[i]);

			var allMonos = objects[i].GetComponentsInChildren<MonoCached>();

			for (int j = 0; j < allMonos.Length; j++)
			{
				allMonos[j].Rise();
			}
		}

		for (int i = 0; i < objects.Length; i++)
		{
			var allMonos = objects[i].GetComponentsInChildren<MonoCached>();

			for (int j = 0; j < allMonos.Length; j++)
			{
				allMonos[j].Ready();
			}
		}
	}

	public void PrepareGameObject(GameObject obj)
	{
		var allMonos = obj.GetComponentsInChildren<MonoCached>();

		_container.InjectGameObject(obj);

		for (int i = 0; i < allMonos.Length; i++)
		{
			allMonos[i].Rise();
		}

		for (int i = 0; i < allMonos.Length; i++)
		{
			allMonos[i].Ready();
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
			PrepareGameObject(allMonos[i].gameObject);
		}
	}
}
