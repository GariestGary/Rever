using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InteractableAbilityProvider : MonoBehaviour, IInteractable, IAwake
{
	[SerializeField] private AbilityType abilityToProvide;
	[SerializeField] private bool usePhysics;
	[SerializeField] private GameObject hint;

	private Rigidbody2D rb;

	private ObjectPoolManager pool;
	private MessageManager msg;


	[Inject]
	public void Constructor(ObjectPoolManager pool, MessageManager msg)
	{
		this.pool = pool;
		this.msg = msg;
	}

	public void OnAwake()
	{
		rb = GetComponent<Rigidbody2D>();

		SetPhysicsState();
	}

	private void SetPhysicsState()
	{
		if(usePhysics)
		{
			rb.bodyType = RigidbodyType2D.Dynamic;
		}
		else
		{
			rb.bodyType = RigidbodyType2D.Static;
		}
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
		player.TryEnableAbility(abilityToProvide);
		msg.Send(ServiceShareData.DIALOG_CLOSED);
		pool.Despawn(gameObject);
	}

}
