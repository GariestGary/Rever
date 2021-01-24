using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saveable : MonoCached, ISaveable
{
	[SerializeField] protected string id;
	[Button] public void GenerateNewID() => GenerateID();

	public string ID => id;

	/// <summary>
	/// captures object state
	/// </summary>
	/// <returns></returns>
	public virtual object CaptureState()
	{
		return null;
	}

	/// <summary>
	/// Restores object state
	/// </summary>
	/// <param name="state">json string</param>
	public virtual void RestoreState(string state)
	{
		
	}
	
	private void GenerateID()
	{
		id = Guid.NewGuid().ToString();
	}
}
