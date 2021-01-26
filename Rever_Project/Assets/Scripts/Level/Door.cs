using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class Door : Saveable
{
	[BoxGroup("Open")][SerializeField] private Vector3 openedPosition;
	[BoxGroup("Open")][SerializeField] private float openDuration;
	[BoxGroup("Open")][SerializeField][CurveRange(0, 0, 1, 1)] private AnimationCurve openEase;
	[BoxGroup("Close")][SerializeField] private Vector3 closedPosition;
	[BoxGroup("Close")][SerializeField] private float closeDuration;
	[BoxGroup("Close")][SerializeField][CurveRange(0, 0, 1, 1)] private AnimationCurve closeEase;
	[Space]
	[SerializeField] private Transform doorGraphics;
	[SerializeField] private Collider2D doorCollider;
    [SerializeField] private MonoBehaviour switcher;
	[SerializeField] private bool openAtStart;

	private bool opened;
	private bool moving;

	public bool Opened => opened;

	public override void Rise()
	{
		if(switcher)
		{
			(switcher as ISwitcher).EnableEvent.AddListener(OpenDoor);
		}

		if(openAtStart)
		{
			doorGraphics.localPosition = openedPosition;
			opened = true;
		}
		else
		{
			doorGraphics.localPosition = closedPosition;
			opened = false;
		}
	}

	public void OpenDoor()
	{
		Debug.Log("open");

		if(opened || moving)
		{
			return;
		}

		moving = true;

		doorGraphics.DOLocalMove(openedPosition, openDuration).SetEase(openEase).OnComplete(() => 
		{
			doorCollider.enabled = false;
			opened = true;
			moving = false;
		});

	}

	public void CloseDoor()
	{
		if(!opened || moving)
		{
			return;
		}

		doorCollider.enabled = true;
		opened = false;
		moving = true;

		doorGraphics.DOLocalMove(closedPosition, closeDuration).SetEase(closeEase).OnComplete(() =>
		{
			moving = false;
		});
	}

	#region saving
	public override object CaptureState()
	{
		return opened;
	}

	public override void RestoreState(string state)
	{
		opened = JsonConvert.DeserializeObject<bool>(state);

		if(opened)
		{
			OpenDoor();
		}
		else
		{
			CloseDoor();
		}
	}
	#endregion

	public override void OnRemove()
	{
		if(switcher)
		{
			(switcher as ISwitcher).EnableEvent.AddListener(OpenDoor);
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position + openedPosition, 0.1f);
		Gizmos.DrawWireSphere(transform.position + closedPosition, 0.1f);
	}
#endif
}
