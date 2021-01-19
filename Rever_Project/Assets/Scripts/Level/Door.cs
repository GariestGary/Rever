using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Saveable
{
	[SerializeField] private GameObject doorCollider;
    [SerializeField] private MonoBehaviour switcher;

	private bool opened;

	public bool Opened => opened;

	public override void Rise()
	{
		(switcher as ISwitcher).OnEnable += OpenDoor;
	}

	//TODO: visuals for opening and closing

	public void OpenDoor()
	{
		if(opened)
		{
			return;
		}

		doorCollider.SetActive(false);
		opened = true;
	}

	public void CloseDoor()
	{
		if(!opened)
		{
			return;
		}

		doorCollider.SetActive(true);
		opened = false;
	}

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

	private void OnDisable()
	{
		(switcher as ISwitcher).OnEnable -= OpenDoor;
	}
}
