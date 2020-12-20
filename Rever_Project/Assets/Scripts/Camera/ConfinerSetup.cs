using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfinerSetup : MonoBehaviour, IAwake
{
	[SerializeField] private bool setConfiner;

	public void OnAwake()
	{
		if (setConfiner)
			Toolbox.GetManager<GameManager>().SetCameraConfiner(GetComponent<PolygonCollider2D>());
	}
}
