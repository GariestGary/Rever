using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
	[SerializeField] private int initialHitPoints;
	[Space]
	[SerializeField] private bool canBeInvulnerable;
	[SerializeField] private float invulnerabilityTime;

	public UnityEvent<HitInfo> HealthChangeEvent;
	public UnityEvent DeathEvent;

	public HitPoints HP => hp;
	public bool Dead => dead;
	public bool IsInvulnerable => currentInvulnerabilityTime > 0;

	private HitPoints hp;
	private bool dead;
	private float currentInvulnerabilityTime = 0;

	public void Initialize()
	{
		dead = false;
		hp = new HitPoints();
		hp.SetMaxHitPoints(initialHitPoints);
		hp.Reset();
	}

	public void HealthUpdate()
	{
		if(canBeInvulnerable)
		{
			InvulnerabilityUpdate();
		}
	}

	private void InvulnerabilityUpdate()
	{
		if (currentInvulnerabilityTime > 0)
		{
			currentInvulnerabilityTime -= Time.deltaTime;
		}
	}

	public void Kill()
	{
		Hit(new HitInfo(hp.maxHitPoints, transform.position, 0));
	}

	public void Hit(HitInfo info)
	{
		if (dead) return;

		hp.Hit(info.damage);
		HealthChangeEvent?.Invoke(info);

		if(hp.currentHitPoints <= 0)
		{
			DeathEvent?.Invoke();
			dead = true;
			return;
		}

		if(canBeInvulnerable)
		{
			currentInvulnerabilityTime = invulnerabilityTime;
		}
	}

	public void ResetHP()
	{
		dead = false;
		hp.Reset();
		HealthChangeEvent?.Invoke(new HitInfo(0, transform.position, 0));
		currentInvulnerabilityTime = 0;
	}

}

[System.Serializable]
public struct HitPoints
{
	public int maxHitPoints { get; private set; }
	public int currentHitPoints { get; private set; }

	public void Hit(int amount)
	{
		currentHitPoints -= Mathf.Abs(amount);
	}

	public void SetMaxHitPoints(int amount)
	{
		maxHitPoints = Mathf.Abs(amount);
	}

	public void AddMaxHitPoints(int amount)
	{
		maxHitPoints += Mathf.Abs(amount);
	}

	public void Reset()
	{
		currentHitPoints = maxHitPoints;
	}

	public override string ToString()
	{
		return "current hp - " + currentHitPoints + ". max hp - " + maxHitPoints;
	}
}
