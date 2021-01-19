using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManagerComponent : MonoBehaviour
{
	private UpdateManager mng;

	public void Setup(UpdateManager updMng)
	{
		mng = updMng;
	}

	private void Update()
	{
		mng.Tick();
	}

	private void FixedUpdate()
	{
		mng.FixedTick();
	}


	private void LateUpdate()
	{
		mng.LateTick();
	}
}
