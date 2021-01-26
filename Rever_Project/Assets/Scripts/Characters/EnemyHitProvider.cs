using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitProvider : MonoBehaviour, IHitProvider
{
    [SerializeField] private Health health;
	[SerializeField] private Controller2D controller;
	[SerializeField] private float recoilMultiplier;

	public void ProvideHit(HitInfo info)
	{
		health.Hit(info);
		controller.AddForce((transform.position - new Vector3(info.from.x, info.from.y)).normalized * recoilMultiplier);
	}
}
