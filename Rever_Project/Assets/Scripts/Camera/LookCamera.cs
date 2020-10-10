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
	}

	public void SetTarget(Transform target)
	{
		if (target == null) return;

		cam.Follow = target;
	}
}
