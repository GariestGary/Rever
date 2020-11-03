using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKHandler : MonoBehaviour
{
	[SerializeField] Player player;

	private void OnAnimatorIK(int layerIndex)
	{
		
	}

	private void OnAnimatorMove()
	{
		//player.UpdateIK();
	}
}
