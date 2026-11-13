public class ReducePlayerFireRateEvent : IGameEvent
{
    public string EventName { get; } = "Fire Rate";

    private float increasePercentage;

    public ReducePlayerFireRateEvent(float increase)
    {
        increasePercentage = increase;
    }

    public void Execute()
    {
        var player = GameReferences.Instance.Player.Player;

        if (player == null)
            return;

        player.FireRate *= 1f + increasePercentage;
    }

}
