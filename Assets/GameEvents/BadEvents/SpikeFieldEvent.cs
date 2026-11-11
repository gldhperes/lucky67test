
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpikeFieldEvent : IGameEvent
{
    public string EventName { get; } = "Spike Field";

    private readonly Tilemap arenaTilemap;
    private readonly TileBase arenaTile;

    private readonly Tilemap spikeTilemap;
    private readonly TileBase warningTile;
    private readonly TileBase spikeTile;

    private readonly LayerMask playerLayer;

    private readonly float spawnInterval;
    private readonly float warningDuration;
    private readonly float spikeDuration;
    private readonly float damage;
    private readonly float damageCooldown;

    private readonly List<Vector3Int> validPositions = new();

    private Coroutine spikeRoutine;
    private bool isRunning;

    public SpikeFieldEvent(Tilemap arenaTilemap, TileBase arenaTile, Tilemap spikeTilemap, TileBase warningTile, TileBase spikeTile, LayerMask playerLayer, float spawnInterval = 5f, float warningDuration = 1f, float spikeDuration = 4f, float damage = 1f, float damageCooldown = 0.5f)
    {
        this.arenaTilemap = arenaTilemap;
        this.arenaTile = arenaTile;

        this.spikeTilemap = spikeTilemap;
        this.warningTile = warningTile;
        this.spikeTile = spikeTile;

        this.playerLayer = playerLayer;

        this.spawnInterval = spawnInterval;
        this.warningDuration = warningDuration;
        this.spikeDuration = spikeDuration;
        this.damage = damage;
        this.damageCooldown = damageCooldown;
    }

    public void Execute()
    {
        if (isRunning)
            return;

        if (!ValidateReferences())
            return;

        CacheValidPositions();

        if (validPositions.Count == 0)
        {
            Debug.LogWarning("Spike Field: No valid 3x3 positions found.");
            return;
        }

        isRunning = true;

        spikeRoutine = CoroutineRunner.Instance.StartRoutine(
            SpikeRoutine()
        );
    }

    public void Stop()
    {
        if (!isRunning)
            return;

        isRunning = false;

        CoroutineRunner.Instance.StopRoutine(spikeRoutine);
        spikeRoutine = null;

        spikeTilemap.ClearAllTiles();
    }

    private bool ValidateReferences()
    {
        if (arenaTilemap == null ||
            arenaTile == null ||
            spikeTilemap == null ||
            warningTile == null ||
            spikeTile == null)
        {
            Debug.LogWarning("Spike Field: Missing references.");
            return false;
        }

        if (arenaTilemap == spikeTilemap)
        {
            Debug.LogWarning(
                "Spike Field: Arena and Spike Tilemaps must be different."
            );
            return false;
        }

        return true;
    }

    private void CacheValidPositions()
    {
        validPositions.Clear();

        BoundsInt bounds = arenaTilemap.cellBounds;

        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (IsValid3x3Area(cell))
                validPositions.Add(cell);
        }
    }

    private bool IsValid3x3Area(Vector3Int origin)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Vector3Int cell = origin + new Vector3Int(x, y, 0);

                if (arenaTilemap.GetTile(cell) != arenaTile)
                    return false;
            }
        }

        return true;
    }

    private IEnumerator SpikeRoutine()
    {
        while (isRunning)
        {
            Vector3Int origin = validPositions[
                Random.Range(0, validPositions.Count)
            ];

            // 1. Mostra o aviso.
            SetFieldTile(origin, warningTile);

            yield return new WaitForSeconds(warningDuration);

            if (!isRunning)
                yield break;

            // 2. Substitui pelo espinho.
            SetFieldTile(origin, spikeTile);

            float elapsed = 0f;
            float nextDamageTime = 0f;

            // 3. Mantém os espinhos ativos durante 4 segundos.
            while (elapsed < spikeDuration)
            {
                if (Time.time >= nextDamageTime)
                {
                    DealPlayerDamage(origin);
                    nextDamageTime = Time.time + damageCooldown;
                }

                yield return null;
                elapsed += Time.deltaTime;
            }

            if (!isRunning)
                yield break;

            // 4. Remove o campo.
            spikeTilemap.ClearAllTiles();

            // 5. Aguarda até o próximo ciclo.
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SetFieldTile(Vector3Int origin, TileBase tile)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Vector3Int cell = origin + new Vector3Int(x, y, 0);
                spikeTilemap.SetTile(cell, tile);
            }
        }
    }

    private void DealPlayerDamage(Vector3Int origin)
    {
        Vector3 center = arenaTilemap.GetCellCenterWorld(origin + new Vector3Int(1, 1, 0));

        Vector3 size = Vector3.Scale(spikeTilemap.cellSize, new Vector3(3f, 3f, 1f));

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, playerLayer);

        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                break;
            }
        }
    }
}
