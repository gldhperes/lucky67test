
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightningStrikeEvent : IGameEvent
{
    public string EventName { get; } = "Lightning Strike";

    private readonly Tilemap arenaTilemap;
    private readonly TileBase arenaTile;
    private readonly GameObject lightningPrefab;

    private readonly float strikeInterval;
    private readonly float damageRadius;
    private readonly float damage;

    private readonly List<Vector3> validPositions = new();

    private Coroutine lightningRoutine;
    private bool isRunning;

    public LightningStrikeEvent(
        Tilemap arenaTilemap,
        TileBase arenaTile,
        GameObject lightningPrefab,
        float strikeInterval = 3f,
        float damageRadius = 1.5f,
        float damage = 10f)
    {
        this.arenaTilemap = arenaTilemap;
        this.arenaTile = arenaTile;
        this.lightningPrefab = lightningPrefab;

        this.strikeInterval = strikeInterval;
        this.damageRadius = damageRadius;
        this.damage = damage;
    }

    public void Execute()
    {
        if (isRunning)
            return;

        if (!ValidateReferences())
            return;

        CacheArenaPositions();

        if (validPositions.Count == 0)
        {
            Debug.LogWarning(
                "[LightningStrike] No arena tiles found."
            );
            return;
        }

        isRunning = true;

        lightningRoutine = CoroutineRunner.Instance.StartRoutine(
            LightningRoutine()
        );
    }

    public void Stop()
    {
        if (!isRunning)
            return;

        isRunning = false;

        CoroutineRunner.Instance.StopRoutine(lightningRoutine);
        lightningRoutine = null;
    }

    private bool ValidateReferences()
    {
        if (arenaTilemap == null || arenaTile == null)
        {
            Debug.LogWarning(
                "[LightningStrike] Missing Tilemap or Arena Tile."
            );

            return false;
        }

        return true;
    }

    private void CacheArenaPositions()
    {
        validPositions.Clear();

        BoundsInt bounds = arenaTilemap.cellBounds;

        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (arenaTilemap.GetTile(cell) == arenaTile)
            {
                Vector3 worldPosition =
                    arenaTilemap.GetCellCenterWorld(cell);

                validPositions.Add(worldPosition);
            }
        }
    }

    private IEnumerator LightningRoutine()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(strikeInterval);

            if (!isRunning)
                yield break;

            if (arenaTilemap == null)
            {
                Stop();
                yield break;
            }

            StrikeLightning();
        }
    }

    private void StrikeLightning()
    {
        if (validPositions.Count == 0)
            return;

        int randomIndex = Random.Range(0, validPositions.Count);

        Vector3 strikePosition = validPositions[randomIndex];

        // Efeito visual do raio.
        if (lightningPrefab != null)
        {
            Object.Instantiate(
                lightningPrefab,
                strikePosition,
                Quaternion.identity
            );
        }

        // Detecta todos os colliders próximos, independentemente da Layer.
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            strikePosition,
            damageRadius
        );

        // Evita atingir o mesmo monstro mais de uma vez.
        HashSet<EnemyBehaviour> damagedEnemies = new();

        foreach (Collider2D hit in hits)
        {
            EnemyBehaviour enemy = hit.GetComponentInParent<EnemyBehaviour>();

            // Ignora qualquer objeto que não seja um monstro.
            if (enemy == null)
                continue;

            if (!damagedEnemies.Add(enemy))
                continue;

            HealthBehaviour health = enemy.GetComponent<HealthBehaviour>();

            if (health == null || health.IsDead)
                continue;

            // Aplica o dano.
            health.TakeDamage(damage);

            // Reproduz o feedback de impacto, caso sobreviva.
            if (!health.IsDead)
            {
                Vector2 hitDirection = ((Vector2)enemy.transform.position - (Vector2)strikePosition).normalized;

                enemy.ReceiveHit(hitDirection);
            }

            Debug.Log($"[LightningStrike] {enemy.name} recebeu {damage} de dano!");
        }
    }
}
