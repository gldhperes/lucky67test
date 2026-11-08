
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 3f;

    [Header("Ricochet")]
    private bool ricochetEnabled;
    private int remainingRicochets;
    private float ricochetSearchRadius;

    private Rigidbody2D rb;

    private readonly HashSet<EnemyBehaviour> hitEnemies = new();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void EnableRicochet(int count, float searchRadius)
    {
        ricochetEnabled = true;
        remainingRicochets = count;
        ricochetSearchRadius = searchRadius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            return;

        HealthBehaviour targetHealth = collision.GetComponentInParent<HealthBehaviour>();

        EnemyBehaviour enemy = collision.GetComponentInParent<EnemyBehaviour>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        if (ricochetEnabled && enemy != null)
        {
            if (!hitEnemies.Add(enemy))
                return;

            if (remainingRicochets > 0 && TryFindNextTarget())
                return;
        }

        Destroy(gameObject);
    }

    private bool TryFindNextTarget()
    {
        EnemyBehaviour[] enemies = FindObjectsByType<EnemyBehaviour>(FindObjectsSortMode.None);

        EnemyBehaviour closestEnemy = null;

        float closestDistance = ricochetSearchRadius * ricochetSearchRadius;

        foreach (EnemyBehaviour enemy in enemies)
        {
            if (enemy == null)
                continue;

            if (hitEnemies.Contains(enemy))
                continue;

            HealthBehaviour health = enemy.GetComponent<HealthBehaviour>();

            if (health == null || health.IsDead)
                continue;

            float distance = ((Vector2)enemy.transform.position - rb.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy == null)
            return false;

        Vector2 direction = ((Vector2)closestEnemy.transform.position - rb.position).normalized;

        float currentSpeed = rb.linearVelocity.magnitude;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        rb.linearVelocity = direction * currentSpeed;

        remainingRicochets--;

        return true;
    }
}
