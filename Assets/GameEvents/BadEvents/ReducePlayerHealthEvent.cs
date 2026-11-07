
public class ReducePlayerHealthEvent : IGameEvent
{
     public string EventName = "Player Health";
    private float damageAmount;

    public ReducePlayerHealthEvent(float damage)
    {
        damageAmount = damage;
    }

    public void Execute()
    {
        // Obtém a referência do Player
        var player = GameReferences.Instance.Player.Player;
        HealthBehaviour playerHealth = player.gameObject.GetComponent<HealthBehaviour>();

        if (player == null)
            return;

        // Aplica o dano
        playerHealth.TakeDamage(damageAmount);
    }
}
