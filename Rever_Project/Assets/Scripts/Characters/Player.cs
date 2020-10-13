using UnityEngine;
using System.Collections;
using Zenject;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour, ITick, IFixedTick, IAwake
{

	

	public bool Process { get; private set; }

	

	private Controller2D controller;
	private InputManager input;

	private bool subscribed = false;
	

	[Inject]
	public void Constructor(InputManager input)
	{
		this.input = input;
	}

	public void OnAwake() 
	{
		controller = GetComponent<Controller2D>();

		SetSubscribe(true);
	}
	public void OnTick() 
	{
		controller.SetDirectionalInput(input.MoveInput);
	}
	public void OnFixedTick()
	{
		controller.Move();
	}

	private void SetSubscribe(bool state)
	{
		if(state)
		{
			if(!subscribed)
			{
				//Put all subs here

				if(input && controller)
				{
					input.JumpStart += controller.OnJumpInputDown;
					input.JumpEnd += controller.OnJumpInputUp;
				}
				else
				{
					return;
				}

				subscribed = true;
			}
		}
		else
		{
			if (subscribed)
			{
				//Put all unsubs here

				if (input && controller)
				{
					input.JumpStart -= controller.OnJumpInputDown;
					input.JumpEnd -= controller.OnJumpInputUp;
				}
				else
				{
					return;
				}

				subscribed = false;
			}
		}
	}

	private void OnEnable()
	{
		Process = true;

		SetSubscribe(true);
	}

	private void OnDisable()
	{
		Process = false;

		SetSubscribe(false);
	}

}
