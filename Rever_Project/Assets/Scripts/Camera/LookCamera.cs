using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LookCamera : MonoBehaviour, IAwake
{
    private CinemachineVirtualCamera cam;

	public void OnAwake()
	{
		cam = GetComponent<CinemachineVirtualCamera>();

		Debug.Log(cam);
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
