
using System.Collections;
using UnityEngine;

public class ReducePlayerFireRateEvent : IGameEvent
{
    public string EventName = "Fire Rate";

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

        var runner = player.GetComponent<MonoBehaviour>();

        if (runner != null)
            runner.StartCoroutine(ReduceFireRateCoroutine(player));
    }

    private IEnumerator ReduceFireRateCoroutine(PlayerBehaviour player)
    {
        var shooting = player.FireRate;

        float originalFireRate = shooting;

        player.FireRate = originalFireRate * (1f + increasePercentage);

        yield return new WaitForSeconds(duration);

        player.FireRate = originalFireRate;
    }
}
