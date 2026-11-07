
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthBehaviour))]
public class EnemyBehaviour : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyData enemyData;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Attack")]
    [SerializeField] private EnemyAttackBehaviour attackBehaviour;

    private Rigidbody2D rb;
    private HealthBehaviour health;

    public EnemyData Data => enemyData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthBehaviour>();

        if (enemyData != null)
        {
            health.Initialize(enemyData.MaxHealth);
        }
    }

    private void Update()
    {
        if (health.IsDead || target == null)
            return;

        attackBehaviour?.PerformAttack();
    }

    private void FixedUpdate()
    {
        if (health.IsDead || target == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = (
            (Vector2)target.position - rb.position
        ).normalized;

        rb.linearVelocity = direction * enemyData.MoveSpeed;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
