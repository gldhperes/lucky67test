
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") return;

        if (collision.gameObject.TryGetComponent<HealthBehaviour>(out var enemyHealth))
        {
            enemyHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
