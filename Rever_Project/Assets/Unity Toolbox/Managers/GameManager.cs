using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

[CreateAssetMenu(menuName = "Toolbox/Managers/Game Manager", fileName = "Game")]
public class GameManager : ManagerBase, IExecute
{
	[SerializeField] private string lookCameraTag;
	[SerializeField] private string spawnPointTag;
	[SerializeField] private string playerPrefabName;

	private ObjectPoolManager pool;
	private ResourcesManager res;

	private LookCamera cam;
	private GameObject spawnPoint;
	private GameObject player;

	private GameObject instantiatedPlayer;

	[Inject]
	public void Constructor(ObjectPoolManager pool, ResourcesManager res)
	{
		this.pool = pool;
		this.res = res;
	}

	public void OnExecute()
	{
		cam = GameObject.FindGameObjectWithTag(lookCameraTag).GetComponent<LookCamera>();
		spawnPoint = GameObject.FindGameObjectWithTag(spawnPointTag);
		player = res.GetResourceByName<GameObject>(playerPrefabName);

		SpawnPlayer();
	}

	private void SpawnPlayer()
	{
		if(spawnPoint && player && cam)
		{
			instantiatedPlayer = pool.Instantiate(player, spawnPoint.transform.position, Quaternion.identity, false);

			cam.SetTarget(instantiatedPlayer.transform);

			Debug.Log("Spawned player");
		}
	}
}