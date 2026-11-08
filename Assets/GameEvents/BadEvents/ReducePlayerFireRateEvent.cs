
using System.Collections;
using UnityEngine;

public class ReducePlayerFireRateEvent : IGameEvent
{
    public string EventName { get; } = "Fire Rate";

    private float increasePercentage;
    private float duration;

    public ReducePlayerFireRateEvent(float increase, float duration = 6.7f)
    {
        increasePercentage = increase;
        this.duration = duration;
    }

    public void Execute()
    {
        var player = GameReferences.Instance.Player.Player;

        if (player == null)
            return;

        player.FireRate *= 1f + increasePercentage;
    }

}
