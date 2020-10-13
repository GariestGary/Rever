using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour, ITick
{
	public bool Process => true;

	public void OnTick()
	{
		Debug.Log("a");
	}
}
