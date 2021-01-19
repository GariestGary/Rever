using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Zenject;
using UnityEngine.Events;

public class NPC : MonoBehaviour, IInteractable
{
	[SerializeField] private string npcName;
	[SerializeField] private string playerTag;

	public UnityEvent<string> InteractEvent;
	public UnityEvent EnteredEvent;
	public UnityEvent ExitedEvent;

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
		EnteredEvent.Invoke();
	}

	public void Exited()
	{
		ExitedEvent.Invoke();
	}

	public void Interact(Player player)
	{
		InteractEvent.Invoke(npcName);
	}
}
