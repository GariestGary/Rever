using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LookCamera : MonoCached
{
    private CinemachineVirtualCamera cam;

	public override void Rise()
	{
		cam = GetComponent<CinemachineVirtualCamera>();
	}

	public void SetTarget(Transform target)
	{
		if(!cam)
		{
			cam = GetComponent<CinemachineVirtualCamera>();
		}

		if (target == null || cam == null) return;

		cam.Follow = target;

		Debug.Log("Target set to camera");
	}
}
