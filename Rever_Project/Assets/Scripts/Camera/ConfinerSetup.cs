using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfinerSetup : MonoCached
{
	[SerializeField] private bool setConfiner;

	public override void Rise()
	{
		if (setConfiner)
			Toolbox.GetManager<GameManager>().SetCameraConfiner(GetComponent<PolygonCollider2D>());
	}
}
