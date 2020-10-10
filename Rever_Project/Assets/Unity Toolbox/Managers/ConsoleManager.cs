using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;


[CreateAssetMenu(fileName = "Console", menuName = "Toolbox/Managers/Console Manager")]
public class ConsoleManager : ManagerBase
{
    [SerializeField] private string prefix;
    [SerializeField] private DefaultCommand[] commands;

	public void ProcessCommand(string inputValue)
    {
        if (!inputValue.StartsWith(prefix)) { return; }

        inputValue = inputValue.Remove(0, prefix.Length);

        string[] inputSplit = inputValue.Split(' ');

        string commandInput = inputSplit[0];
        string[] args = inputSplit.Skip(1).ToArray();

        ProcessCommand(commandInput, args);
    }

    public void ProcessCommand(string commandInput, string[] args)
    {
        foreach (var command in commands)
        {
            if (!commandInput.Equals(command.CommandWord, System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (command.Process(args))
            {
                return;
            }
        }
    }
}
