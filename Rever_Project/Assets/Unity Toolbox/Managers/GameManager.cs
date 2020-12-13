using DG.Tweening;
using System;
using System.Collections;
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
	[SerializeField] private string playerPrefabName;
	[SerializeField] private LevelChangeData initLevelData;
	[SerializeField] private string LevelsFolderPath;
	[SerializeField] private float openedSceneDelay;

	private ObjectPoolManager pool;
	private ResourcesManager res;
	private MessageManager msg;
	private UpdateManager upd;
	private InputManager input;
	private DiContainer _container;

	private LookCamera cam;
	private GameObject spawnPoint;
	private GameObject playerPrefab;

	private LevelHandler currentLevelHandler;

	private GameObject instantiatedPlayerTransform;
	private Player currentPlayer;
	private Inventory currentInventory;

	private Scene mainScene;
	private string currentSceneName;
	private bool changingScene = false;
	private bool nextSceneLoaded;
	private bool previousSceneUnloaded;

	public LevelHandler CurrentLevelHandler => currentLevelHandler;
	public Player CurrentPlayer => currentPlayer;
	public Inventory CurrentInventory => currentInventory;
	public Scene MainScene => mainScene;

	[Inject]
	public void Constructor(ObjectPoolManager pool, ResourcesManager res, MessageManager msg, UpdateManager upd, InputManager input, DiContainer _container)
	{
		this.pool = pool;
		this.res = res;
		this.msg = msg;
		this.upd = upd;
		this.input = input;
		this._container = _container;
	}

	public void OnExecute()
	{
		mainScene = SceneManager.GetActiveScene();

		currentSceneName = "";

		LoadLevel(initLevelData);

		cam = GameObject.FindGameObjectWithTag(lookCameraTag).GetComponent<LookCamera>();
		playerPrefab = res.GetResourceByName<GameObject>(playerPrefabName);
	}

	public void SetCameraConfiner(Collider2D bounds)
	{
		cam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = bounds;
	}

	public void ResetPlayer()
	{
		instantiatedPlayerTransform.transform.position = spawnPoint.transform.position;
		currentPlayer.PlayerHealth.ResetHP();
	}

	private void SpawnPlayer()
	{
		if(playerPrefab && cam)
		{
			if(instantiatedPlayerTransform == null)
			{
				instantiatedPlayerTransform = pool.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, true);
				currentPlayer = instantiatedPlayerTransform.GetComponent<Player>();
				currentInventory = instantiatedPlayerTransform.GetComponent<Inventory>();
				cam.SetTarget(instantiatedPlayerTransform.transform);
			}
		}
	}

	private void TryOpenScene(LevelChangeData data)
	{
		if (!nextSceneLoaded || !previousSceneUnloaded) return;

		if(!instantiatedPlayerTransform) SpawnPlayer();

		AddUpdatesFromScene(SceneManager.GetSceneByName(currentSceneName));

		currentLevelHandler.SetupLevel(instantiatedPlayerTransform.transform, data.nextSpawnPointTag);

		Toolbox.Instance.StartCoroutine(SceneOpenedDelay(data.direction));
		//TODO: 
	}

	private IEnumerator SceneOpenedDelay(float direction)
	{
		float currentTime = 0;
		input.TrySetDefaultInputActive(false, true);

		input.SetMovementInput(new Vector2(direction, 0));

		while (currentTime < openedSceneDelay)
		{
			currentTime += Time.deltaTime;
			yield return null;
		}

		input.SetMovementInput(Vector2.zero);
		input.TrySetDefaultInputActive(true, true);
		msg.Send(ServiceShareData.SCENE_READY);
	}

	public void LoadLevel(LevelChangeData data)
	{
		if (!Application.CanStreamedLevelBeLoaded(LevelsFolderPath + "/" + data.nextLevelName))
		{
			Debug.LogWarning("Scene with name '" + data.nextLevelName + "' doesn't exist");
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
			unloadingLevel.completed += _ => { previousSceneUnloaded = true; TryOpenScene(data); };
		}

		
		AsyncOperation loadingLevel = SceneManager.LoadSceneAsync(data.nextLevelName, LoadSceneMode.Additive);
		
		loadingLevel.completed += _ => { nextSceneLoaded = true; currentSceneName = data.nextLevelName; TryOpenScene(data); };
		
	}

	private void AddUpdatesFromScene(Scene scene)
	{
		currentLevelHandler = LevelHandler.Instance;

		scene.GetRootGameObjects().ToList().ForEach(x =>
		{
			_container.InjectGameObject(x);

			x.GetComponentsInChildren<ITick>().ToList().ForEach(tick => upd.Add(tick));
			x.GetComponentsInChildren<ILateTick>().ToList().ForEach(tick => upd.Add(tick));
			x.GetComponentsInChildren<IFixedTick>().ToList().ForEach(tick => upd.Add(tick));
			x.GetComponentsInChildren<IAwake>().ToList().ForEach(a => a.OnAwake());
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

