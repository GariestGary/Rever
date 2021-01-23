using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Zenject;

[CreateAssetMenu(menuName = "Toolbox/Managers/Object Pool Manager", fileName = "Object Pool")]
public class ObjectPoolManager : ManagerBase, IExecute, ISceneChange
{
    [SerializeField] private List<Pool> PoolsList = new List<Pool>();

	private Transform ObjectPoolParent;
    private Dictionary<string, LinkedList<GameObject>> Pools = new Dictionary<string, LinkedList<GameObject>>();
    private DiContainer _container;
    private UpdateManager upd;
    private LinkedList<GameObject> objectsToDestroyOnLevelChange = new LinkedList<GameObject>();

    [Inject]
    public void Constructor(DiContainer _container, UpdateManager upd)
	{
        this._container = _container;
        this.upd = upd;
	}

    public void OnExecute()
    {
        Pools = new Dictionary<string, LinkedList<GameObject>>();

        ObjectPoolParent = new GameObject().transform;
        ObjectPoolParent.name = "Pool Parent";

        for (int i = 0; i < PoolsList.Count; i++)
        {
            AddPool(PoolsList[i].tag, PoolsList[i].pooledObject, PoolsList[i].size);
        }
    }

    public void AddPool(Pool poolToAdd)
	{
        if (Pools.ContainsKey(poolToAdd.tag))
        {
            Debug.LogWarning("Pool with tag " + poolToAdd.tag + " already exist's");
            return;
        }

        LinkedList<GameObject> objectPoolList = new LinkedList<GameObject>();

        for (int j = 0; j < poolToAdd.size; j++)
        {
            CreateNewPoolObject(poolToAdd.pooledObject, objectPoolList);
        }

        PoolsList.Add(poolToAdd);
        Pools.Add(poolToAdd.tag, objectPoolList);
    }

    public void AddPool(string tag, GameObject obj, int size)
    {
        if (Pools.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " already exists");
            return;
        }

        LinkedList<GameObject> objectPool = new LinkedList<GameObject>();

        for (int j = 0; j < size; j++)
        {
            CreateNewPoolObject(obj, objectPool);
        }

        Pools.Add(tag, objectPool);
    }

    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
	{
        GameObject obj;
        obj = _container.InstantiatePrefab(prefab, position, rotation, parent);
        upd.PrepareGameObject(obj);
        upd.AddGameObject(obj);
        return obj;
	}

    /// <summary>
    /// Spawns GameObject from pool with specified tag, then calls all OnSpawn methods in it
    /// </summary>
    /// <param name="poolTag">pool tag with necessary object</param>
    /// <param name="position">initial position</param>
    /// <param name="rotation">initial rotation</param>
    /// <param name="parent">parent transform for GameObject</param>
    /// <param name="data">data to provide in GameObject</param>
    /// <returns>GameObject from pool</returns>
    public GameObject Spawn(string poolTag, Vector3 position, Quaternion rotation, Transform parent = null, object data = null)
    {
        //Returns null if object pool with specified tag doesn't exists
        if (!Pools.ContainsKey(poolTag))
        {
            Debug.LogWarning("Object pool with tag " + poolTag + " doesn't exists");
            return null;
        }

        //Create new object if last in list is active
        if (Pools[poolTag].Last.Value.activeSelf)
        {
            CreateNewPoolObject(Pools[poolTag].Last.Value, Pools[poolTag]);
        }

        //Take last object
        GameObject obj = Pools[poolTag].Last.Value;
        Pools[poolTag].RemoveLast();

        //Return null if last object is null;
        if (obj == null)
        {
            Debug.Log("object from pool " + poolTag + " you trying to spawn is null");
            return null;
        }

        //Setting transform
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.SetParent(parent);
        obj.SetActive(true);

        //Call all spawn methods in gameobject
        IPooledObject[] pooled = obj.GetComponentsInChildren<IPooledObject>();

		for (int i = 0; i < pooled.Length; i++)
		{
            if (pooled[i] != null)
            { 
                pooled[i].OnSpawn(data, this);
            }
		}

        //Add object back to start
        Pools[poolTag].AddFirst(obj);
        return obj;
    }

    private GameObject CreateNewPoolObject(GameObject obj, LinkedList<GameObject> pool)
    {
        GameObject poolObj = Instantiate(obj);
        poolObj.name = obj.name;
        poolObj.transform.SetParent(ObjectPoolParent);

        upd.PrepareGameObject(poolObj);

        poolObj.gameObject.SetActive(false);
        pool.AddLast(poolObj);

        return poolObj;
    }

    /// <summary>
    /// Removes GameObject from scene and returns it to pool
    /// </summary>
    /// <param name="ObjectToDespawn">object to despawn</param>
    /// <param name="delay">delay before despawning</param>
    public void Despawn(GameObject ObjectToDespawn, int delay = 0)
    {
        if (ObjectToDespawn == null)
        {
            return;
        }

        if(delay != 0)
		{
            Observable.Timer(new System.TimeSpan(0, 0, 0, 0, delay)).Subscribe(_ =>
            {
                ReturnToPool(ObjectToDespawn);
            });
        }
        else
		{
            ReturnToPool(ObjectToDespawn);
        }
    }

    /// <summary>
    /// Analogue to Unity's GameObject disable
    /// </summary>
    /// <param name="obj"></param>
    public void DisableGameObject(GameObject obj)
	{
        obj.SetActive(false);

        var allMonos = obj.GetComponentsInChildren<MonoCached>();

        for (int i = 0; i < allMonos.Length; i++)
        {
            allMonos[i].OnRemove();
            upd.Remove(allMonos[i]);
        }
    }

    /// <summary>
    /// Analogue to Unity's GameObject enable
    /// </summary>
    /// <param name="obj"></param>
    public void EnableGameObject(GameObject obj)
	{
        obj.SetActive(true);

        var allMonos = obj.GetComponentsInChildren<MonoCached>();

        for (int i = 0; i < allMonos.Length; i++)
        {
            allMonos[i].OnAdd();
            upd.Add(allMonos[i]);
        }
    }

    private void ReturnToPool(GameObject obj)
	{
        obj.SetActive(false);

        DisableGameObject(obj);

        obj.transform.SetParent(ObjectPoolParent);
	}

	public void OnSceneChange()
	{
        Debug.Log("Cleaning pools");

        PoolsList.Where(x => x.destroyOnLevelChange).ToList().ForEach(x => 
        {
            Debug.Log("Removing pool with tag " + x.tag);
            Pools.TryGetValue(x.tag, out objectsToDestroyOnLevelChange);

			foreach (var obj in objectsToDestroyOnLevelChange)
			{
                obj.GetComponentsInChildren<MonoCached>().ToList().ForEach(tick => upd.Remove(tick));
                Destroy(obj);
			}
            
        });

        PoolsList.RemoveAll(x => x.destroyOnLevelChange);
        Resources.UnloadUnusedAssets();
	}
}

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject pooledObject;
    public int size;
    public bool destroyOnLevelChange;
}
