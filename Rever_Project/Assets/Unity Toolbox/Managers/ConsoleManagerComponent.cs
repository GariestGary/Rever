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
            uiCanvas.SetActive(false);
        }
        else
        {
            uiCanvas.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    public void ProcessCommand(string inputValue)
    {
        Toolbox.GetManager<ConsoleManager>()?.ProcessCommand(inputValue);

        inputField.text = string.Empty;
    }
}