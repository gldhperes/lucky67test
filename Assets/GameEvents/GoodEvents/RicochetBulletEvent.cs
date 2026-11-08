
public class RicochetBulletEvent : IGameEvent
{
    public string EventName { get; } = "Ricochet Bullet";

    public void Execute()
    {
        PlayerBehaviour player = GameReferences.Instance.Player.Player;

        if (player == null)
            return;

        player.EnableRicochet();
    }
}
