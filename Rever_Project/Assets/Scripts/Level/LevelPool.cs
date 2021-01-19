using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelPool : MonoCached
{
    [SerializeField] private List<Pool> poolsList = new List<Pool>();

	private ObjectPoolManager pool;

	[Inject]
	public void Constructor(ObjectPoolManager pool)
	{
		this.pool = pool;
	}

	public override void Rise()
	{
		foreach (var poolToAdd in poolsList)
		{
			pool.AddPool(poolToAdd);
		}
	}
}
