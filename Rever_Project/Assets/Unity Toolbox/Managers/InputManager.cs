using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Input", menuName = "Toolbox/Managers/Input Manager")]
public class InputManager : ManagerBase, IExecute, ISceneChange
{
	[SerializeField] private bool externalInput = false;

	//Create input actions assets named "Controls"
	private Controls controls;

	public Vector2 PointerPosition { get; private set; }
	public Vector2 PointerDelta { get; private set; }
	public Vector2 MoveInput { get; private set; } = Vector2.zero;

	public event Action OnClickDown;
	public event Action OnClickUp;

	public event Action JumpStart;
	public event Action JumpEnd;

	public event Action Dash;

	public event Action Interact;

	private bool disabledByGameplay;

	public bool Clicked { get; private set; } = false;

	public Controls GetBindings()
	{
		return controls;
	}

	public void SetMovementInput(Vector2 input)
	{
		if (!externalInput) return;

		MoveInput = input;
	}

	private void OnEnable()
	{
		controls?.Enable();
	}

	private void OnDisable()
	{
		controls?.Disable();
	}

	private void OnDestroy()
	{
		controls?.Dispose();
	}

	private void InitializeControls()
	{
		controls = new Controls();

		controls.Default.PointerPosition.performed += ctx => PointerPosition = ctx.ReadValue<Vector2>();
		controls.Default.PointerDelta.performed += ctx => PointerDelta = ctx.ReadValue<Vector2>();

		controls.Default.Movement.performed += ctx => 
		{
			if (externalInput) return;

			MoveInput = ctx.ReadValue<Vector2>();
		};

		controls.Default.Click.performed += _ => 
		{
			Clicked = true;
			OnClickDown?.Invoke();
		};

		controls.Default.Click.canceled += _ =>
		{
			Clicked = false;
			OnClickUp?.Invoke();
		};

		controls.Default.Jump.performed += _ => JumpStart?.Invoke();
		controls.Default.Jump.canceled += _ => JumpEnd?.Invoke();

		controls.Default.Interact.performed += _ => Interact?.Invoke();

		controls.Default.Dash.performed += _ => Dash?.Invoke();

		controls.Enable();
	}

	public void OnExecute()
	{
		controls?.Dispose();

		InitializeControls();
	}

	public void TrySetDefaultInputActive(bool state, bool byGameplay)
	{
		if(state)
		{
			if(byGameplay || !disabledByGameplay)
			{
				disabledByGameplay = false;
				controls?.Default.Enable();
			}
		}
		else
		{
			disabledByGameplay = byGameplay;
			controls?.Default.Disable();
		}
	}

	public void OnSceneChange()
	{
		
	}
}
