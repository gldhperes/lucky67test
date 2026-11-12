
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBehaviour : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip shootSfx;
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private HealthBehaviour playerHealth;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference shootAction;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float fireRate = 0.25f;
    public float FireRate
    {
        get => fireRate;
        set => fireRate = value;
    }
    private float nextFireTime;

    [Header("Ricochet Settings")]
    [SerializeField] private int ricochetCount = 3;
    [SerializeField] private float ricochetSearchRadius = 5f;
    [SerializeField] private bool ricochetEnabled;

    [Header("Multishot Settings")]
    [SerializeField] private bool multishotEnabled;
    [SerializeField] private float multishotAngle = 15f;


    private Vector2 moveInput;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        playerHealth.Initialize(50f);
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        shootAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        shootAction.action.Disable();
    }

    private void Update()
    {
        // Captura movimentação WASD
        moveInput = moveAction.action.ReadValue<Vector2>();

        // Limita a velocidade diagonal
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        // Atualiza a direção visual do personagem
        UpdateDirection();
        // Dispara continuamente enquanto o botão estiver pressionado
        if (shootAction.action.IsPressed() && Time.time >= nextFireTime)
        {
            Shoot();

            nextFireTime = Time.time + fireRate;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void UpdateDirection()
    {
        if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }


    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float cameraDistance = Mathf.Abs(mainCamera.transform.position.z - firePoint.position.z);

        Vector3 mouseScreenPosition = new Vector3(mousePosition.x, mousePosition.y, cameraDistance);

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        Vector2 direction = ((Vector2)mouseWorldPosition - (Vector2)firePoint.position).normalized;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        // Bala central
        ShootProjectile(direction);

        if (multishotEnabled)
        {
            // Bala para um lado
            Vector2 leftDirection = Quaternion.Euler(0f, 0f, -multishotAngle) * direction;

            // Bala para o outro lado
            Vector2 rightDirection = Quaternion.Euler(0f, 0f, multishotAngle) * direction;

            ShootProjectile(leftDirection);
            ShootProjectile(rightDirection);
        }
    }

    private void ShootProjectile(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, rotation);

        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        ProjectileBehaviour projectileBehaviour = projectile.GetComponent<ProjectileBehaviour>();

        if (projectileBehaviour != null && ricochetEnabled)
        {
            projectileBehaviour.EnableRicochet(ricochetCount, ricochetSearchRadius);
        }

        if (projectileRb != null)
        {
            projectileRb.linearVelocity = direction * projectileSpeed;
            AudioManager.Instance?.PlaySFX(shootSfx);
        }
        else
        {
            Debug.LogError("O Projectile precisa de um Rigidbody2D!");
            Destroy(projectile);
        }
    }


    public void EnableRicochet()
    {
        ricochetEnabled = true;
    }

    public void EnableMultishot()
    {
        multishotEnabled = true;
    }
}
