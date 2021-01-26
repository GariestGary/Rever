using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
	[SerializeField] private int initialHitPoints;

	public UnityEvent HealthChangeEvent;
	public UnityEvent DeathEvent;

	public HitPoints HP => hp;
	public bool Dead => dead;

	private HitPoints hp;
	private bool dead;

    public void Initialize()
	{
		dead = false;
		hp = new HitPoints();
		hp.SetMaxHitPoints(initialHitPoints);
		hp.Reset();
	}

	public void Kill()
	{
		Hit(hp.maxHitPoints);
	}

	public void Hit(int amount)
	{
		Debug.Log("hitted " + amount);
		if (dead) return;

		hp.Hit(amount);
		HealthChangeEvent?.Invoke();

		if(hp.currentHitPoints <= 0)
		{
			DeathEvent?.Invoke();
			dead = true;
		}
	}

	public void ResetHP()
	{
		dead = false;
		hp.Reset();
		HealthChangeEvent?.Invoke();
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
