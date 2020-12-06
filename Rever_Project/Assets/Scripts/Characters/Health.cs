using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
	[SerializeField] private int initialHitPoints;

	public event Action OnHealthChange;

	public HitPoints HP => hp;

	private HitPoints hp;

    public void Initialize()
	{
		hp = new HitPoints();
		hp.SetMaxHitPoints(initialHitPoints);
		hp.Reset();
	}

	public void Hit(int amount)
	{
		hp.Hit(amount);
		OnHealthChange?.Invoke();
	}

	public void ResetHP()
	{
		hp.Reset();
		OnHealthChange?.Invoke();
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
