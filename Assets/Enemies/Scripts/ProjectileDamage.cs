
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    public void SetDamage(float amount)
    {
        damage = amount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponent<IDamageable>();

        if (target != null)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
