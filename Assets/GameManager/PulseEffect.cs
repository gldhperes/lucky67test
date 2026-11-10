
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PulseEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase targetTile;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseDuration = 0.3f;

    private readonly List<Vector3Int> tilePositions = new();
    private readonly List<Matrix4x4> originalMatrices = new();

    private Coroutine pulseCoroutine;

    private void Awake()
    {
        CacheTargetTiles();
    }

    private void CacheTargetTiles()
    {
        tilePositions.Clear();
        originalMatrices.Clear();

        if (tilemap == null || targetTile == null)
        {
            Debug.LogWarning("TilemapPulseEffect: Referências não configuradas!");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            if (tilemap.GetTile(position) != targetTile)
                continue;

            tilePositions.Add(position);
            originalMatrices.Add(tilemap.GetTransformMatrix(position));
        }

        Debug.Log($"TilemapPulseEffect: {tilePositions.Count} tiles encontrados.");
    }

    public void PlayPulse()
    {
        if (tilePositions.Count == 0)
            return;

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            RestoreOriginalTransforms();
        }

        pulseCoroutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        float halfDuration = pulseDuration / 2f;

        // Expansão
        yield return AnimateScale(1f, pulseScale, halfDuration);

        // Retorno ao tamanho original
        yield return AnimateScale(pulseScale, 1f, halfDuration);

        RestoreOriginalTransforms();

        pulseCoroutine = null;
    }

    private IEnumerator AnimateScale(float start, float end, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            float currentScale = Mathf.Lerp(start, end, t);

            ApplyScale(currentScale);

            yield return null;
        }

        ApplyScale(end);
    }

    private void ApplyScale(float scale)
    {
        if (tilemap == null)
            return;

        Matrix4x4 scaleMatrix = Matrix4x4.Scale(Vector3.one * scale);

        for (int i = 0; i < tilePositions.Count; i++)
        {
            tilemap.SetTransformMatrix(tilePositions[i], originalMatrices[i] * scaleMatrix);
        }
    }

    private void RestoreOriginalTransforms()
    {
        if (tilemap == null)
            return;

        for (int i = 0; i < tilePositions.Count; i++)
        {
            tilemap.SetTransformMatrix(tilePositions[i], originalMatrices[i]);
        }
    }

    private void OnDisable()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        RestoreOriginalTransforms();
    }
}
