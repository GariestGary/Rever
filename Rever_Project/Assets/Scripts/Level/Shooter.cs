using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class Shooter: MonoCached
{
	public Transform shootPoint;
	public MonoBehaviour switcher;
	public string shellPoolTag;
	public ShootInfo info;
	public bool readyAfterShoot;
	[HideInInspector]
	public bool ready;

	private ObjectPoolManager pool;

	public UnityEvent ShootEvent;


	[Inject]
	public void Constructor(ObjectPoolManager pool)
	{
		this.pool = pool;
	}

	public override void Rise()
	{
		if (switcher)
		{
			(switcher as ISwitcher).EnableEvent.AddListener(Shoot);
		}
	}

	public void Shoot()
	{
		if (readyAfterShoot)
		{
			ready = true;
		}
		else
		{
			ready = false;
		}

		pool.Spawn(shellPoolTag, shootPoint.position, Quaternion.identity, null, info);
	}

	public void Destroy()
	{
		if (switcher)
		{
			(switcher as ISwitcher).EnableEvent.RemoveListener(Shoot);
		}
	}
}

[System.Serializable]
public struct ShootInfo
{
	public Vector2 direction;
	public float force;
}
