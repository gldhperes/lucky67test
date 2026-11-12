
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthBehaviour))]
public class EnemyBehaviour : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyData enemyData;

    [Header("Hit Audio")]
    [SerializeField] private AudioClip hitSfx;
    [SerializeField, Range(0f, 1f)] private float hitVolume = 1f;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Attack")]
    [SerializeField] private EnemyAttackBehaviour attackBehaviour;


    [Header("Hit Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = Color.black;
    [SerializeField] private float hitDuration = 0.08f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 1.5f;
    [SerializeField] private float knockbackDuration = 0.08f;

    private Color originalColor;
    private Coroutine hitRoutine;
    private Coroutine knockbackRoutine;
    private bool isKnockedBack;

    private Rigidbody2D rb;
    private HealthBehaviour health;

    public EnemyData Data => enemyData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthBehaviour>();

        if (enemyData != null)
            health.Initialize(enemyData.MaxHealth);

        if (attackBehaviour == null)
            attackBehaviour = GetComponent<EnemyAttackBehaviour>();

        if (attackBehaviour != null && target != null)
            attackBehaviour.Initialize(this, target);

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (health.IsDead || target == null)
            return;

        attackBehaviour?.PerformAttack();
    }

    private void FixedUpdate()
    {
        if (isKnockedBack)
            return;

        if (health.IsDead || target == null || enemyData == null)
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

        if (attackBehaviour != null && target != null)
            attackBehaviour.Initialize(this, target);
    }

    public void ReceiveHit(Vector2 hitDirection)
    {
        if (health.IsDead)
            return;

        AudioManager.Instance?.PlaySFX(hitSfx);
        
        if (hitRoutine != null)
            StopCoroutine(hitRoutine);

        hitRoutine = StartCoroutine(HitFlashRoutine());

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockbackRoutine(hitDirection));
    }

    private IEnumerator HitFlashRoutine()
    {
        spriteRenderer.color = hitColor;

        yield return new WaitForSeconds(hitDuration);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        hitRoutine = null;
    }

    private IEnumerator KnockbackRoutine(Vector2 direction)
    {
        isKnockedBack = true;

        rb.linearVelocity = direction.normalized * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
        knockbackRoutine = null;
    }
}
