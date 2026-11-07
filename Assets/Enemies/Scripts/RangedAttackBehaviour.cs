
using UnityEngine;

public class RangedAttackBehaviour : EnemyAttackBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 8f;

    private float nextAttackTime;

    public override void PerformAttack()
    {
        if (target == null || projectilePrefab == null || firePoint == null)
            return;

        if (Time.time < nextAttackTime)
            return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > enemy.Data.AttackRange)
            return;

        Vector2 direction = ((Vector2)target.position - (Vector2)firePoint.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, rotation);

        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            projectileRb.linearVelocity = direction * projectileSpeed;
        }

        ProjectileDamage projectileDamage = projectile.GetComponent<ProjectileDamage>();

        if (projectileDamage != null)
        {
            projectileDamage.SetDamage(enemy.Data.AttackDamage);
        }

        nextAttackTime = Time.time + enemy.Data.AttackCooldown;
    }
}
