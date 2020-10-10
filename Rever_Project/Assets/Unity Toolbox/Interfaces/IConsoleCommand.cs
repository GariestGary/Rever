

public interface IConsoleCommand
{
    bool Active { get; }
    string CommandWord { get; }
    bool Process(string[] args);
}