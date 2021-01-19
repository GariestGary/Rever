using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoCached
{
	[SerializeField] private GameObject doorCollider;
    [SerializeField] private SwitcherWrapper switcher;

	public override void Rise()
	{
		switcher.Enabled += OpenDoor;
	}

	public void OpenDoor()
	{
		doorCollider.SetActive(false);
	}

	private void OnDisable()
	{
		switcher.Enabled -= OpenDoor;
	}
}
