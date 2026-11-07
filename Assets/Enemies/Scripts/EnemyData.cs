using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("General Attributes")]
    [SerializeField] private string enemyName;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Attack Attributes")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1f;

    public string EnemyName => enemyName;
    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public float AttackDamage => attackDamage;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
}
