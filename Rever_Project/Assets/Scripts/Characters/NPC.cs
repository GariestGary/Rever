using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Zenject;

public class NPC : MonoBehaviour, IInteractable
{
	[SerializeField] private string npcName;
	[SerializeField] private string playerTag;
	[SerializeField] private GameObject hint;
	[SerializeField] private Flowchart dialog;

	private MessageManager msg;


	[Inject]
	public void Constructor(MessageManager msg)
	{
		this.msg = msg;
	}

	public void ExitDialog()
	{
		msg.Send(ServiceShareData.DIALOG_CLOSED);
	}

	public void Entered()
	{
		hint.SetActive(true);
	}

	public void Exited()
	{
		hint.SetActive(false);
	}

	public void Interact(Player player)
	{
		Debug.Log("Interacting with " + npcName);
		dialog.SendFungusMessage(npcName);
	}
}
