using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ConsoleManagerComponent: MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject uiCanvas = null;
    [SerializeField] private TMP_InputField inputField = null;

    public void Toggle(InputAction.CallbackContext context)
    {
        if (!context.action.triggered) { return; }

        if (uiCanvas.activeSelf)
        {
            Toolbox.GetManager<InputManager>().TrySetDefaultInputActive(true, false);
            uiCanvas.SetActive(false);
        }
        else
        {
            Toolbox.GetManager<InputManager>().TrySetDefaultInputActive(false, false);
            uiCanvas.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    public void ProcessCommand(string inputValue)
    {
        Toolbox.GetManager<ConsoleManager>()?.ProcessCommand(inputValue);

        inputField.text = string.Empty;
        inputField.ActivateInputField();
    }
}