
using System.Collections;
using UnityEngine;

public class ReducePlayerSpeedEvent : IGameEvent
{
    public string EventName { get; } = "Player Speed";

    private float reductionPercentage;
    private float duration = 6.7f;

    public ReducePlayerSpeedEvent(float reduction)
    {
        reductionPercentage = reduction;
    }

    public void Execute()
    {
        PlayerBehaviour player = GameReferences.Instance.Player.Player;

        if (player == null)
            return;

      
        player.MoveSpeed *= 1f - reductionPercentage;
    }
}
