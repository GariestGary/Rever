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

	private SaveManager save;
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

	private GameObject instantiatedPlayer;
	private Player currentPlayer;
	private Inventory currentInventory;

	private AsyncOperation unloadingLevel;
	private AsyncOperation loadingLevel;
	private Scene mainScene;
	private string currentSceneName;

	public string CurrentSceneName => currentSceneName;
	public string MainSceneName => "Main";
	public Transform CameraTransform => cam.transform;
	public LevelHandler CurrentLevelHandler => currentLevelHandler;
	public Player CurrentPlayer => currentPlayer;
	public Inventory CurrentInventory => currentInventory;
	public Scene MainScene => mainScene;

	[Inject]
	public void Constructor(ObjectPoolManager pool, ResourcesManager res, MessageManager msg, UpdateManager upd, SaveManager save, InputManager input, DiContainer _container)
	{
		this.save = save;
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

		cam = GameObject.FindGameObjectWithTag(lookCameraTag).GetComponent<LookCamera>();
		playerPrefab = res.GetResourceByName<GameObject>(playerPrefabName);

		if(!string.IsNullOrEmpty(initLevelData.nextLevelName))
		{
			LoadLevel(initLevelData);
		}
	}

	public void SetCameraConfiner(Collider2D bounds)
	{
		cam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = bounds;
	}

	public void ResetPlayer()
	{
		instantiatedPlayer.transform.position = spawnPoint.transform.position;
		currentPlayer.PlayerHealth.ResetHP();
	}

	private void SpawnPlayer()
	{
		if(playerPrefab && cam)
		{
			if(instantiatedPlayer == null)
			{
				instantiatedPlayer = pool.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
				currentPlayer = instantiatedPlayer.GetComponent<Player>();
				currentInventory = instantiatedPlayer.GetComponent<Inventory>();
				cam.SetTarget(instantiatedPlayer.transform);
			}
		}
	}

	private void OpenScene(LevelChangeData data)
	{
		if(instantiatedPlayer == null) SpawnPlayer();

		AddUpdatesFromScene(SceneManager.GetSceneByName(currentSceneName));

		currentLevelHandler.SetupLevel(instantiatedPlayer.transform, data.nextSpawnPointTag);

		save.Load();

		Toolbox.Instance.StartCoroutine(SceneOpenedDelay(data.direction));
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

		if (string.IsNullOrEmpty(currentSceneName))
		{
			unloadingLevel = null;
		}
		else
		{
			FindObjectsOfType<MonoBehaviour>().Where(x => x is ISceneChange).ToList().ForEach(x => (x as ISceneChange).OnSceneChange());
			RemoveUpdatesFromScene(SceneManager.GetSceneByName(currentSceneName));
			save.Save();
			unloadingLevel = SceneManager.UnloadSceneAsync(currentSceneName);
		}

		loadingLevel = SceneManager.LoadSceneAsync(data.nextLevelName, LoadSceneMode.Additive);

		Toolbox.Instance.StartCoroutine(LoadWaitCoroutine(data));
	}

	private IEnumerator LoadWaitCoroutine(LevelChangeData data)
	{
		if(unloadingLevel == null)
		{
			while (!loadingLevel.isDone)
			{
				yield return null;
			}
		}
		else
		{
			while (!unloadingLevel.isDone && !loadingLevel.isDone)
			{
				yield return null;
			}
		}

		currentSceneName = data.nextLevelName;

		OpenScene(data);

		yield break;
	}

	private void AddUpdatesFromScene(Scene scene)
	{
		currentLevelHandler = LevelHandler.Instance;

		List<GameObject> sceneGO = scene.GetRootGameObjects().ToList();

		sceneGO.ForEach(x => 
		{
			_container.InjectGameObject(x);
			upd.RiseGameObject(x);
		});

		sceneGO.ForEach(x =>
		{
			upd.AddGameObject(x);
		});
	}

	private void RemoveUpdatesFromScene(Scene scene)
	{
		scene.GetRootGameObjects().ToList().ForEach(x =>
		{
			upd.RemoveGameObject(x);
		});

		currentLevelHandler.Dispose();
	}
}

