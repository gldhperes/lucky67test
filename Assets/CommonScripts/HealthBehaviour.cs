using UnityEngine;
using System;


public interface IDamageable
{
    void TakeDamage(float damage);
}

public class HealthBehaviour : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [ReadOnly][SerializeField] private float maxHealth;

    [ReadOnly][field: SerializeField] public float CurrentHealth { get; private set; }


    public bool IsDead { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    public void Initialize(float maxHealth)
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {

        // Debug.Log($"{this.gameObject.name} Take {damage} of Damage");
        if (IsDead || damage <= 0f)
            return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        OnDeath?.Invoke();

        Destroy(gameObject);
    }
}

