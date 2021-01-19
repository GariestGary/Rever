using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
	private bool pressed;

	private bool process;

	public bool Process => process;
	public bool Pressed => pressed;
	public int StayingObjectsCount => stayingObjectsCount;

	public event Action OnPress;
    public event Action OnRelease;

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
		OnPress?.Invoke();
	}

	public void Release()
	{
		if (!pressed) return;

		pressed = false;
		renderer.sprite = releasedSprite;
		OnRelease?.Invoke();
	}

	private void OnEnable()
	{
		process = true;
	}

	private void OnDisable()
	{
		process = false;
	}
}
