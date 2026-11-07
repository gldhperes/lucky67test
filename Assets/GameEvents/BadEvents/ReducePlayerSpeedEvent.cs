
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

        MonoBehaviour runner = player.GetComponent<MonoBehaviour>();

        if (runner != null)
        {
            runner.StartCoroutine(ReduceSpeedCoroutine(player));
        }
    }

    private IEnumerator ReduceSpeedCoroutine(PlayerBehaviour player)
    {
        var movement = player.MoveSpeed;

        float originalSpeed = movement;

        player.MoveSpeed = originalSpeed * (1f - reductionPercentage);

        yield return new WaitForSeconds(duration);

        player.MoveSpeed = originalSpeed;
    }
}
