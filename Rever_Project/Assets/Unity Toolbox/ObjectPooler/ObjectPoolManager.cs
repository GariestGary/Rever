using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Zenject;

[CreateAssetMenu(menuName = "Toolbox/Managers/Object Pool Manager", fileName = "Object Pool")]
public class ObjectPoolManager : ManagerBase, IExecute, ISceneChange
{
    [SerializeField] private int defaultPoolSize = 50;
    [SerializeField] private int minObjectsCountToCreateNew = 5;
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
        ObjectPoolParent.name = "PoolParent";

        for (int i = 0; i < PoolsList.Count; i++)
        {
            AddPool(PoolsList[i].tag, PoolsList[i].pooledObject, PoolsList[i].size, false);
        }
    }

    public void AddPool(Pool poolToAdd)
	{
        if (Pools.ContainsKey(poolToAdd.tag))
        {
            return;
        }

        LinkedList<GameObject> objectPool = new LinkedList<GameObject>();

        for (int j = 0; j < poolToAdd.size; j++)
        {
            CreateNewPoolObject(poolToAdd.pooledObject, objectPool);
        }

        PoolsList.Add(poolToAdd);
        Pools.Add(poolToAdd.tag, objectPool);
    }

    public void AddPool(string tag, GameObject obj, int size, bool removeOnLevelChange)
    {
        if (Pools.ContainsKey(tag))
        {
            return;
        }

        LinkedList<GameObject> objectPool = new LinkedList<GameObject>();

        for (int j = 0; j < size; j++)
        {
            CreateNewPoolObject(obj, objectPool);
        }

        Pools.Add(tag, objectPool);
    }

    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool callAwakes = true, Transform parent = null, string poolTag = "", bool removePoolOnLevelChange = false)
	{
        GameObject obj;

        if(!string.IsNullOrEmpty(poolTag))
		{
            if(Pools.ContainsKey(poolTag))
			{
                CreateNewPoolObject(prefab, Pools[poolTag]);
			}
            else
			{
                AddPool(poolTag, prefab, defaultPoolSize, removePoolOnLevelChange);
			}

            obj = TryGetObject(poolTag, position, rotation, parent);
		}
        else
		{
            obj = _container.InstantiatePrefab(prefab, position, rotation, parent);
            _container.Inject(obj);

            if(callAwakes)
			{
                obj.GetComponentsInChildren<IAwake>().ToList().ForEach(a => a.OnAwake());
            }

            obj.GetComponentsInChildren<ITick>().ToList().ForEach(tick => upd.Add(tick));
            obj.GetComponentsInChildren<ILateTick>().ToList().ForEach(tick => upd.Add(tick));
            obj.GetComponentsInChildren<IFixedTick>().ToList().ForEach(tick => upd.Add(tick));
        }

        return obj;
	}

    public GameObject TryGetObject(string poolTag, Vector3 position, Quaternion rotation, Transform parent = null, object data = null)
    {
        if (!Pools.ContainsKey(poolTag))
        {
            Debug.LogWarning("Object pool with tag " + poolTag + " doesn't exist");
            return null;
        }

        if (Pools[poolTag].Last.Value.activeSelf)
        {
            CreateNewPoolObject(Pools[poolTag].Last.Value, Pools[poolTag]);
        }

        GameObject obj = Pools[poolTag].Last.Value;
        Pools[poolTag].RemoveLast();

        if (obj == null)
        {
            return null;
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.SetParent(parent);
        obj.SetActive(true);

        IPooledObject[] pooled = obj.GetComponents<IPooledObject>();


		for (int i = 0; i < pooled.Length; i++)
		{
            if (pooled[i] != null)
            { 
                pooled[i].OnSpawn(data);
            }
		}
       
        Pools[poolTag].AddFirst(obj);
        return (obj);
    }

    private GameObject CreateNewPoolObject(GameObject obj, LinkedList<GameObject> pool)
    {
        GameObject poolObj = Instantiate(obj);
        poolObj.name = obj.name;
        poolObj.transform.SetParent(ObjectPoolParent);
        
        poolObj.gameObject.SetActive(false);
        pool.AddLast(poolObj);

        return poolObj;
    }

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
                ObjectToDespawn.SetActive(false);
                ObjectToDespawn.transform.SetParent(ObjectPoolParent);
            });
        }
        else
		{
            ObjectToDespawn.SetActive(false);
            ObjectToDespawn.transform.SetParent(ObjectPoolParent);
        }

        //TODO: delete from update manager
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
                obj.GetComponentsInChildren<ITick>().ToList().ForEach(tick => upd.Remove(tick));
                obj.GetComponentsInChildren<ILateTick>().ToList().ForEach(tick => upd.Remove(tick));
                obj.GetComponentsInChildren<IFixedTick>().ToList().ForEach(tick => upd.Remove(tick));
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
