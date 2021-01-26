using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class Button : MonoCached
{
	[Header("TEMPORARY")]
	[SerializeField] private SpriteRenderer renderer;
	[SerializeField] private Sprite pressedSprite;
	[SerializeField] private Sprite releasedSprite;
	[Space]
	[SerializeField] private Transform checkPosition;
	[SerializeField] private Vector3 checkDirection;
	[Space]
	[SerializeField] private bool oncePressed;

	private int stayingObjectsCount;
	private bool pressed = false;

	public bool Pressed => pressed;
	public int StayingObjectsCount => stayingObjectsCount;

	public UnityEvent PressEvent;
    public UnityEvent ReleaseEvent;

	public override void Rise()
	{
		Release();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (pressed && oncePressed) return;

		if (collision.GetComponent<ButtonPresser>())
		{
			stayingObjectsCount++;

			if (!pressed)
			{
				Press();

				if (oncePressed)
				{
					stayingObjectsCount = 0;
				}
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (pressed && oncePressed) return;

		if (collision.GetComponent<ButtonPresser>())
		{
			if(stayingObjectsCount > 0)
				stayingObjectsCount--;

			if (pressed && stayingObjectsCount <= 0)
			{
				Release();
			}
		}
	}

	public void Press()
	{
		if (pressed) return;
		
		pressed = true;
		renderer.sprite = pressedSprite;
		PressEvent?.Invoke();

		if(oncePressed)
		{
			GetComponent<Collider2D>().enabled = false;
		}
	}

	public void Release()
	{
		if (!pressed) return;

		if (oncePressed) return;

		pressed = false;
		renderer.sprite = releasedSprite;
		ReleaseEvent?.Invoke();
	}
}
