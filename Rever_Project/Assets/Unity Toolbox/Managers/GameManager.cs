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

	[Inject]
	public void Constructor(ObjectPoolManager pool, ResourcesManager res)
	{
		this.pool = pool;
		this.res = res;
	}

	public void OnExecute()
	{
		SpawnPlayer();
	}

	private void SpawnPlayer()
	{
		GameObject spawnPoint = GameObject.FindGameObjectWithTag(spawnPointTag);
		GameObject player = res.GetResourceByName<GameObject>(playerPrefabName);
		LookCamera cam = GameObject.FindGameObjectWithTag(lookCameraTag).GetComponent<LookCamera>();

		if(spawnPoint && player && cam)
		{
			GameObject instantiatedPlayer = pool.Instantiate(player, spawnPoint.transform.position, Quaternion.identity);

			cam.SetTarget(instantiatedPlayer.transform);
		}
	}
}