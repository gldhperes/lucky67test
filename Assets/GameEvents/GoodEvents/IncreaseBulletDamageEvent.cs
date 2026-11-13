
public class IncreaseBulletDamageEvent : IGameEvent
{
    public string EventName { get; } = "Increase Bullet Damage";

    private readonly float damageIncrease;

    public IncreaseBulletDamageEvent(float damageIncrease = 1.5f)
    {
        this.damageIncrease = damageIncrease;
    }

    public void Execute()
    {
        ProjectileBehaviour ProjectileBehaviour = GameReferences.Instance.Player.ProjectileBehaviour;

        if (ProjectileBehaviour == null)
            return;

        ProjectileBehaviour.IncreaseDamage();
    }
}
