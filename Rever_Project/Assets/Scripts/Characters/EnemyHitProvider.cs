using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitProvider : MonoBehaviour, IHitProvider
{
    [SerializeField] private Health health;
	[SerializeField] private Controller2D controller;
	[SerializeField] private float recoilMultiplier;

	public void ProvideHit(int amount, Vector3 position)
	{
		health.Hit(amount);
		controller.AddForce((transform.position - position).normalized * recoilMultiplier);
	}
}
