using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEventsWrapper : MonoCached
{
    [Header("3D")]
    public UnityEvent<Collision> CollisionEnterEvent;
    public UnityEvent<Collision> CollisionStayEvent;
    public UnityEvent<Collision> CollisionExitEvent;
    [Space]
    public UnityEvent<Collider> TriggerEnterEvent;
    public UnityEvent<Collider> TriggerStayEvent;
    public UnityEvent<Collider> TriggerExitEvent;
    [Space(25)]
    [Header("2D")]
    public UnityEvent<Collision2D> CollisionEnter2DEvent;
    public UnityEvent<Collision2D> CollisionStay2DEvent;
    public UnityEvent<Collision2D> CollisionExit2DEvent;
    [Space]
    public UnityEvent<Collider2D> TriggerEnter2DEvent;
    public UnityEvent<Collider2D> TriggerStay2DEvent;
    public UnityEvent<Collider2D> TriggerExit2DEvent;

	private void OnCollisionEnter(Collision collision)
	{
		CollisionEnterEvent?.Invoke(collision);
	}

	private void OnCollisionStay(Collision collision)
	{
		CollisionStayEvent?.Invoke(collision);
	}

	private void OnCollisionExit(Collision collision)
	{
		CollisionExitEvent?.Invoke(collision);
	}

	private void OnTriggerEnter(Collider other)
	{
		TriggerEnterEvent?.Invoke(other);
	}

	private void OnTriggerStay(Collider other)
	{
		TriggerStayEvent?.Invoke(other);
	}

	private void OnTriggerExit(Collider other)
	{
		TriggerExitEvent?.Invoke(other);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		CollisionEnter2DEvent?.Invoke(collision);
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		CollisionStay2DEvent?.Invoke(collision);
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		CollisionExit2DEvent?.Invoke(collision);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		TriggerEnter2DEvent?.Invoke(collision);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		TriggerStay2DEvent?.Invoke(collision);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		TriggerExit2DEvent?.Invoke(collision);
	}
}
