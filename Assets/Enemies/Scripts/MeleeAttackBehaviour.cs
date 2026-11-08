
using UnityEngine;

public class MeleeAttackBehaviour : EnemyAttackBehaviour
{
    private float nextAttackTime;

    public override void PerformAttack()
    {
        if (enemy == null || target == null || enemy.Data == null)
            return;

        if (Time.time < nextAttackTime)
            return;

        float distance = Vector2.Distance(
            transform.position,
            target.position
        );

        if (distance > enemy.Data.AttackRange)
            return;

        HealthBehaviour targetHealth =
            target.GetComponent<HealthBehaviour>();

        if (targetHealth == null)
            targetHealth = target.GetComponentInParent<HealthBehaviour>();

        if (targetHealth == null)
            return;

        targetHealth.TryTakeDamage(enemy.Data.AttackDamage, enemy.GetComponent<Collider2D>());

        nextAttackTime = Time.time + enemy.Data.AttackCooldown;
    }
}
