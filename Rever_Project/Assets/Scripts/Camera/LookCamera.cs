using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class LookCamera : MonoCached
{
	[SerializeField] private float defaultOrtSize;
	[SerializeField] private float sizeChangeDuration;

    private CinemachineVirtualCamera cam;

	public override void Rise()
	{
		cam = GetComponent<CinemachineVirtualCamera>();
	}

	public override void Tick()
	{
		cam.UpdateCameraState(Vector3.up, Time.deltaTime);
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

	public void ResetCamera()
	{
		DOTween.To(() => cam.m_Lens.OrthographicSize, x => cam.m_Lens.OrthographicSize = x, defaultOrtSize, sizeChangeDuration).SetEase(Ease.InOutCubic);
		cam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = null;
	}

	public void SetConfiner(Collider2D bounds)
	{
		cam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = bounds;
	}

	public void SetOrthographicSize(float size)
	{
		DOTween.To(() => cam.m_Lens.OrthographicSize, x => cam.m_Lens.OrthographicSize = x, size, sizeChangeDuration).SetEase(Ease.InOutCubic);
	}
}
