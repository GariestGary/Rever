using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelPool : MonoBehaviour, IAwake
{
    [SerializeField] private List<Pool> poolsList = new List<Pool>();

	private ObjectPoolManager pool;

	[Inject]
	public void Constructor(ObjectPoolManager pool)
	{
		this.pool = pool;
	}

	public void OnAwake()
	{
		foreach (var poolToAdd in poolsList)
		{
			pool.AddPool(poolToAdd);
		}
	}
}
