using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IAwake
{
	[SerializeField] private GameObject doorCollider;
    [SerializeField] private SwitcherWrapper switcher;

	public void OnAwake()
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
