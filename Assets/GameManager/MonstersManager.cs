using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonstersManager : MonoBehaviour
{
    [Header("Spawn References")]
    [SerializeField] private Transform player;
    [SerializeField] private Tilemap spawnTilemap;
    [SerializeField] private TileBase validSpawnTile;

    [Header("Monster Settings")]
    [SerializeField] private GameObject[] monsterPrefabs;

    [Header("Monster Limits")]
    [SerializeField] private int maxMonsters = 6;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Spawn Restrictions")]
    [SerializeField] private float minPlayerDistance = 5f;
    [SerializeField] private float spawnCheckRadius = 0.4f;
    [SerializeField] private LayerMask blockingLayers;
    [SerializeField] private int maxSpawnAttempts = 30;

    // Cells where spawning is allowed
    private readonly List<Vector3Int> validSpawnCells = new();

    // Monsters spawned and managed by this MS
    private readonly HashSet<GameObject> activeMonsters = new();

    private float spawnTimer;

    private void Start()
    {
        CacheSpawnCells();

        spawnTimer = spawnInterval;
    }

    private void Update()
    {
        // Remove destroyed monsters from tracking
        activeMonsters.RemoveWhere(monster => monster == null);

        // Respect maximum monster limit
        if (activeMonsters.Count >= maxMonsters)
            return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            TrySpawnMonster();
            spawnTimer = spawnInterval;
        }
    }

    private void CacheSpawnCells()
    {
        validSpawnCells.Clear();

        if (spawnTilemap == null || validSpawnTile == null)
        {
            Debug.LogError("[MS] Spawn Tilemap or Spawn Tile is missing!");
            return;
        }

        BoundsInt bounds = spawnTilemap.cellBounds;

        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (spawnTilemap.GetTile(cell) == validSpawnTile)
            {
                validSpawnCells.Add(cell);
            }
        }

        Debug.Log($"[MS] Valid spawn cells found: {validSpawnCells.Count}");
    }

    private void TrySpawnMonster()
    {
        if (player == null || monsterPrefabs == null || monsterPrefabs.Length == 0)
            return;

        if (validSpawnCells.Count == 0)
            return;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            // Select a random valid cell
            int index = Random.Range(0, validSpawnCells.Count);

            Vector3 spawnPosition =
                spawnTilemap.GetCellCenterWorld(validSpawnCells[index]);

            // Check minimum distance from player
            if (Vector2.Distance(player.position, spawnPosition) < minPlayerDistance)
                continue;

            // Check for obstacles or other colliders
            if (Physics2D.OverlapCircle(
                spawnPosition,
                spawnCheckRadius,
                blockingLayers) != null)
            {
                continue;
            }

            // Select a random monster prefab
            GameObject prefab = monsterPrefabs[
                Random.Range(0, monsterPrefabs.Length)
            ];

            if (prefab == null)
                continue;

            // Spawn monster
            GameObject monster = Instantiate(
                prefab,
                spawnPosition,
                Quaternion.identity
            );

            activeMonsters.Add(monster);

            return;
        }

        // No valid position found after maximum attempts
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, minPlayerDistance);
    }
}
