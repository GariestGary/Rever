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

	public UnityEvent<HealthChangeInfo> HealthChangeEvent;
	public UnityEvent HitEvent;
	public UnityEvent HealEvent;
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
		ChangeHealth(new HealthChangeInfo(hp.maxHitPoints, transform.position, 0));
	}

	public void ChangeHealth(HealthChangeInfo info)
	{
		if (dead || IsInvulnerable || info.amount == 0) return;

		hp.ChangeHealth(info.amount);
		HealthChangeEvent?.Invoke(info);

		if(info.amount > 0)
		{
			HealEvent?.Invoke();
		}
		else
		{
			HitEvent?.Invoke();
		}

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
		HealthChangeEvent?.Invoke(new HealthChangeInfo(0, transform.position, 0));
		currentInvulnerabilityTime = 0;
	}

}

[System.Serializable]
public struct HitPoints
{
	public int maxHitPoints { get; private set; }
	public int currentHitPoints { get; private set; }

	public void ChangeHealth(int amount)
	{
		currentHitPoints += amount;
		currentHitPoints = Mathf.Clamp(currentHitPoints, 0, maxHitPoints);
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

public class HealthChangeInfo
{
	public int amount;
	public float force;
	public Vector2 from;

	public HealthChangeInfo(int amount, Vector2 from, float force = 1)
	{
		this.amount = amount;
		this.from = from;
		this.force = force;
	}
}
