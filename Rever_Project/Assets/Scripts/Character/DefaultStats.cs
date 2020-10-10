using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultStats : MonoBehaviour, IAwake
{
    [SerializeField] protected int maxHealth = 10;

    private int currentHealth;

	public void OnAwake()
	{
		currentHealth = maxHealth;
	}

	public void ChangeHealth(int amount)
	{
		currentHealth += amount;

		currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
	}
}
