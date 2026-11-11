
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
    private readonly LayerMask monsterLayer;

    private readonly float strikeInterval;
    private readonly float damageRadius;
    private readonly float damage;

    private readonly List<Vector3> validPositions = new List<Vector3>();

    private Coroutine lightningRoutine;
    private bool isRunning;

    public LightningStrikeEvent(
        Tilemap arenaTilemap,
        TileBase arenaTile,
        GameObject lightningPrefab,
        LayerMask monsterLayer,
        float strikeInterval = 3f,
        float damageRadius = 1.5f,
        float damage = 1f)
    {
        this.arenaTilemap = arenaTilemap;
        this.arenaTile = arenaTile;
        this.lightningPrefab = lightningPrefab;
        this.monsterLayer = monsterLayer;

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
            Debug.LogWarning("Lightning Strike: No arena tiles found.");
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
            Debug.LogWarning("Lightning Strike: Missing Tilemap or Arena Tile.");
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

        // Efeito visual
        if (lightningPrefab != null)
        {
            Object.Instantiate(
                lightningPrefab,
                strikePosition,
                Quaternion.identity
            );
        }

        // Detecção de monstros
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            strikePosition,
            damageRadius,
            monsterLayer
        );

        HashSet<IDamageable> damagedMonsters = new HashSet<IDamageable>();

        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable == null)
                continue;

            // Evita causar dano duplicado em monstros com vários colliders.
            if (!damagedMonsters.Add(damageable))
                continue;

            damageable.TakeDamage(damage);
        }
    }
}
