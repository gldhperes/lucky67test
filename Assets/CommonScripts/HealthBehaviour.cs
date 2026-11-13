
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface IDamageable
{
    void TakeDamage(float damage);
}

public class HealthBehaviour : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
#if UNITY_EDITOR
    [ReadOnly]
#endif
    [SerializeField] private float maxHealth;
    public float MaxHealth => maxHealth;
#if UNITY_EDITOR
    [ReadOnly]
#endif
    [field: SerializeField]
    public float CurrentHealth { get; private set; }

    [Header("Invulnerability Settings")]
    [SerializeField] private bool enableInvulnerability;
    [SerializeField, Min(0f)] private float invulnerabilityDuration = 1f;

    public bool IsInvulnerable { get; private set; }
    public bool IsDead { get; private set; }

    private Coroutine invulnerabilityRoutine;

    private Collider2D ownCollider;
    private readonly HashSet<Collider2D> ignoredColliders = new();

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        ownCollider = GetComponent<Collider2D>();
    }

    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        CurrentHealth = maxHealth;
        IsDead = false;
        IsInvulnerable = false;
    }

    public void TakeDamage(float damage)
    {
        TryTakeDamage(damage);
    }

    public bool TryTakeDamage(float damage, Collider2D attackerCollider = null)
    {
        if (IsDead || damage <= 0f)
            return false;

        // Already invulnerable: ignore this enemy's collision too.
        if (enableInvulnerability && IsInvulnerable)
        {
            IgnoreCollisionWith(attackerCollider);
            return false;
        }

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0f)
        {
            Die();
            return true;
        }

        if (enableInvulnerability && invulnerabilityDuration > 0f)
        {
            StartInvulnerability(attackerCollider);
        }

        return true;
    }

    private void StartInvulnerability(Collider2D attackerCollider)
    {
        IsInvulnerable = true;

        IgnoreCollisionWith(attackerCollider);

        invulnerabilityRoutine = StartCoroutine(
            InvulnerabilityCoroutine()
        );
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        yield return new WaitForSeconds(invulnerabilityDuration);

        IsInvulnerable = false;

        RestoreIgnoredCollisions();

        invulnerabilityRoutine = null;
    }

    private void IgnoreCollisionWith(Collider2D otherCollider)
    {
        if (ownCollider == null ||
            otherCollider == null ||
            otherCollider == ownCollider)
            return;

        if (ignoredColliders.Contains(otherCollider))
            return;

        Physics2D.IgnoreCollision(
            ownCollider,
            otherCollider,
            true
        );

        ignoredColliders.Add(otherCollider);
    }

    private void RestoreIgnoredCollisions()
    {
        foreach (Collider2D otherCollider in ignoredColliders)
        {
            if (ownCollider != null && otherCollider != null)
            {
                Physics2D.IgnoreCollision(
                    ownCollider,
                    otherCollider,
                    false
                );
            }
        }

        ignoredColliders.Clear();
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        CurrentHealth = Mathf.Min(
            maxHealth,
            CurrentHealth + amount
        );

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;
        IsInvulnerable = false;

        if (invulnerabilityRoutine != null)
        {
            StopCoroutine(invulnerabilityRoutine);
            invulnerabilityRoutine = null;
        }

        RestoreIgnoredCollisions();

        OnDeath?.Invoke();

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (invulnerabilityRoutine != null)
        {
            StopCoroutine(invulnerabilityRoutine);
            invulnerabilityRoutine = null;
        }

        IsInvulnerable = false;
        RestoreIgnoredCollisions();
    }
}
