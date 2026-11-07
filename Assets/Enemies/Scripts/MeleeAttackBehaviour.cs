
using UnityEngine;

public class MeleeAttackBehaviour : EnemyAttackBehaviour
{
    private float nextAttackTime;

    public override void PerformAttack()
    {
        if (target == null)
            return;

        if (Time.time < nextAttackTime)
            return;

        float distance = Vector2.Distance(
            transform.position,
            target.position
        );

        if (distance > enemy.Data.AttackRange)
            return;

        HealthBehaviour targetHealth = target.GetComponent<HealthBehaviour>();

        if (targetHealth == null)
            return;

        targetHealth.TakeDamage(enemy.Data.AttackDamage);

        nextAttackTime = Time.time + enemy.Data.AttackCooldown;
    }
}
