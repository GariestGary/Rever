using UnityEngine;

public abstract class DefaultCommand : ScriptableObject, IConsoleCommand
{
	[SerializeField] private bool isActive;
	[SerializeField] private string commandWord;

	public bool Active => isActive;

	public string CommandWord => commandWord;

	public abstract bool Process(string[] args);
}