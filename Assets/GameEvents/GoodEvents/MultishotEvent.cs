
public class MultishotEvent : IGameEvent
{
    public string EventName { get; } = "Multishot";

    public void Execute()
    {
        PlayerBehaviour player = GameReferences.Instance.Player.Player;

        if (player == null)
            return;

        player.EnableMultishot();
    }
}
