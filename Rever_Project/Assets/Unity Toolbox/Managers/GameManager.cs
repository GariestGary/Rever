using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Cinemachine;

[CreateAssetMenu(menuName = "Toolbox/Managers/Game Manager", fileName = "Game")]
public class GameManager : ManagerBase, IExecute
{
	[SerializeField] private string lookCameraTag;
	[SerializeField] private string spawnPointTag;
	[SerializeField] private string playerPrefabName;
	[SerializeField] private string initLevelName;
	[SerializeField] private string LevelsFolderPath;

	private ObjectPoolManager pool;
	private ResourcesManager res;
	private MessageManager msg;
	private UpdateManager upd;
	private DiContainer _container;

	private LookCamera cam;
	private GameObject spawnPoint;
	private GameObject playerPrefab;

	private LevelHandler currentLevelHandler;

	private GameObject instantiatedPlayer;
	private Player currentPlayer;
	private Inventory currentInventory;

	private string currentSceneName;
	private bool changingScene = false;
	private bool nextSceneLoaded;
	private bool previousSceneUnloaded;

	public LevelHandler CurrentLevelHandler => currentLevelHandler;
	public Player CurrentPlayer => currentPlayer;
	public Inventory CurrentInventory => currentInventory;

	[Inject]
	public void Constructor(ObjectPoolManager pool, ResourcesManager res, MessageManager msg, UpdateManager upd, DiContainer _container)
	{
		this.pool = pool;
		this.res = res;
		this.msg = msg;
		this.upd = upd;
		this._container = _container;
	}

	public void OnExecute()
	{
		currentSceneName = "";

		LoadLevel(initLevelName);

		cam = GameObject.FindGameObjectWithTag(lookCameraTag).GetComponent<LookCamera>();
		playerPrefab = res.GetResourceByName<GameObject>(playerPrefabName);
	}

	public void SetCameraConfiner(Collider2D bounds)
	{
		cam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = bounds;
	}

	public void ResetPlayer()
	{
		instantiatedPlayer.transform.position = spawnPoint.transform.position;
		instantiatedPlayer.GetComponent<Player>().Respawn();
	}

	private void SpawnPlayer()
	{
		if(spawnPoint && playerPrefab && cam)
		{
			if(instantiatedPlayer == null)
			{
				instantiatedPlayer = pool.Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity, true);
				currentPlayer = instantiatedPlayer.GetComponent<Player>();
				currentInventory = instantiatedPlayer.GetComponent<Inventory>();
				cam.SetTarget(instantiatedPlayer.transform);
			}
			else
			{
				instantiatedPlayer.transform.position = spawnPoint.transform.position;
			}

			Debug.Log("Spawned player");
		}
	}

	private void TryOpenScene()
	{
		if (!nextSceneLoaded || !previousSceneUnloaded) return;

		AddUpdatesFromScene(SceneManager.GetSceneByName(currentSceneName));

		spawnPoint = GameObject.FindGameObjectWithTag(spawnPointTag);

		SpawnPlayer();

		//TODO: 
	}

	public void LoadLevel(string name)
	{
		if (!Application.CanStreamedLevelBeLoaded(LevelsFolderPath + "/" + name))
		{
			Debug.LogWarning("Scene with name '" + name + "' doesn't exist");
			return;
		}

		nextSceneLoaded = false;
		previousSceneUnloaded = false;

		if (string.IsNullOrEmpty(currentSceneName))
		{
			previousSceneUnloaded = true;
		}
		else
		{
			FindObjectsOfType<MonoBehaviour>().Where(x => x is ISceneChange).ToList().ForEach(x => (x as ISceneChange).OnSceneChange());
			RemoveUpdatesFromScene(SceneManager.GetSceneByName(currentSceneName));
			AsyncOperation unloadingLevel = SceneManager.UnloadSceneAsync(currentSceneName);
			unloadingLevel.completed += _ => { previousSceneUnloaded = true; TryOpenScene(); };
		}

		
		AsyncOperation loadingLevel = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
		
		loadingLevel.completed += _ => { nextSceneLoaded = true; currentSceneName = name; TryOpenScene(); };
		
	}

	private void AddUpdatesFromScene(Scene scene)
	{
		scene.GetRootGameObjects().ToList().ForEach(x =>
		{
			_container.InjectGameObject(x);

			x.GetComponentsInChildren<ITick>().ToList().ForEach(tick => upd.Add(tick));
			x.GetComponentsInChildren<ILateTick>().ToList().ForEach(tick => upd.Add(tick));
			x.GetComponentsInChildren<IFixedTick>().ToList().ForEach(tick => upd.Add(tick));
			x.GetComponentsInChildren<IAwake>().ToList().ForEach(a => a.OnAwake());

			if(x.TryGetComponent(out LevelHandler lvlHandler))
			{
				currentLevelHandler = lvlHandler;
			}
		});
	}

	private void RemoveUpdatesFromScene(Scene scene)
	{
		scene.GetRootGameObjects().ToList().ForEach(x =>
		{
			x.GetComponentsInChildren<ITick>().ToList().ForEach(tick => upd.Remove(tick));
			x.GetComponentsInChildren<ILateTick>().ToList().ForEach(tick => upd.Remove(tick));
			x.GetComponentsInChildren<IFixedTick>().ToList().ForEach(tick => upd.Remove(tick));
		});

		currentLevelHandler.Dispose();
	}
}