using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : Saveable
{
	[SerializeField] private string enemyTag;

	private EnemyBase currentEnemy;
	private SpawnerState spawnerState;

	private ObjectPoolManager pool;

	[Inject]
	public void Constructor(ObjectPoolManager pool)
	{
		this.pool = pool;
	}

	public override object CaptureState()
	{
		return spawnerState;
	}

	public override void RestoreState(string state)
	{
		if(state == null)
		{
			SpawnEnemy();
			return;
		}

		spawnerState = JsonConvert.DeserializeObject<SpawnerState>(state);

		if (currentEnemy == null)
		{
			SpawnEnemy();
		}

		if(spawnerState.enemyDead)
		{
			//currentEnemy.ForceDeath();
			currentEnemy.transform.position = spawnerState.deathPosition;
		}
	}

	private void EnemyDeathHandle()
	{
		spawnerState.deathPosition = currentEnemy.transform.position;
		spawnerState.enemyDead = true;
		currentEnemy.SelfHealth.DeathEvent.RemoveListener(EnemyDeathHandle);
	}

	private void SpawnEnemy()
	{
		GameObject enemy = pool.Spawn(enemyTag, transform.position, Quaternion.identity);
		enemy.TryGetComponent(out currentEnemy);
		currentEnemy.SelfHealth.DeathEvent.AddListener(EnemyDeathHandle);
	}

	private struct SpawnerState
	{
		public bool enemyDead;
		public Vector3 deathPosition;
	}

	private void OnDisable()
	{
		if(currentEnemy)
		{
			currentEnemy.SelfHealth.DeathEvent.RemoveListener(EnemyDeathHandle);
		}
	}

}
