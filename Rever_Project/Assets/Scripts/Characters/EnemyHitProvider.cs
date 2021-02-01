using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitProvider : MonoBehaviour, IHitProvider
{
    [SerializeField] private Health health;
	[SerializeField] private Controller2D controller;
	

	public void ProvideHit(HealthChangeInfo info)
	{
		health.ChangeHealth(info);
	}
}
