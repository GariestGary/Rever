

public interface IConsoleCommand
{
    bool Active { get; }
    string CommandWord { get; }
    CommandProcessedMessage Process(string[] args);
}