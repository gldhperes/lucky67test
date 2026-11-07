public interface IGameEvent
{
    string EventName { get; }
    void Execute();
}