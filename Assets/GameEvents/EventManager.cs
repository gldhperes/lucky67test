
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private UpgradeIconController upgradeIconController;

    [Header("General Settings")]
    [SerializeField] private Tilemap arenaTilemap;
    [SerializeField] private TileBase arenaTile;

    [Header("Lightning Strike Settings")]
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private LayerMask monsterLayer;

    [Header("Lightning Parameters")]
    [SerializeField] private float strikeInterval = 3f;
    [SerializeField] private float damageRadius = 1.5f;
    [SerializeField] private float lightningDamage = 10f;

    [Header("Spike Field Settings")]
    [SerializeField] private Tilemap spikeTilemap;
    [SerializeField] private TileBase warningTile;
    [SerializeField] private TileBase spikeTile;

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private float spikeInterval = 5f;
    [SerializeField] private float warningDuration = 1f;
    [SerializeField] private float spikeDuration = 4f;
    [SerializeField] private float spikeDamage = 5f;
    [SerializeField] private float damageCooldown = 0.5f;
    private SpikeFieldEvent spikeFieldEvent;





    private List<IGameEvent> goodEvents = new();
    private List<IGameEvent> badEvents = new();

    private void Awake()
    {
        InitializeEvents();
    }

    private void InitializeEvents()
    {
        // Eventos bons
        goodEvents.Add(new ReducePlayerFireRateEvent(-0.1f));
        goodEvents.Add(new ReducePlayerHealthEvent(-10));
        goodEvents.Add(new ReducePlayerSpeedEvent(-.12f));

        goodEvents.Add(new LightningStrikeEvent(arenaTilemap, arenaTile, lightningPrefab, strikeInterval, damageRadius, lightningDamage));
        goodEvents.Add(new RicochetBulletEvent());
        goodEvents.Add(new IncreaseBulletDamageEvent());
        goodEvents.Add(new MultishotEvent());
        // Eventos ruins
        badEvents.Add(new ReducePlayerFireRateEvent(0.1f));
        badEvents.Add(new ReducePlayerHealthEvent(10));
        badEvents.Add(new ReducePlayerSpeedEvent(.12f));

        spikeFieldEvent = new SpikeFieldEvent(arenaTilemap, arenaTile, spikeTilemap, warningTile, spikeTile, playerLayer, spikeInterval, warningDuration, spikeDuration, spikeDamage, damageCooldown);
        badEvents.Add(spikeFieldEvent);
    }



    public void ExecuteRandomEvent()
    {
        // Sorteia entre eventos positivos e negativos.
        bool isGood = Random.value < 0.5f;
        // bool isGood = true;
        // bool isGood = false;

        List<IGameEvent> selectedList = isGood ? goodEvents : badEvents;
        if (selectedList.Count == 0)
            return;

        int index = Random.Range(0, selectedList.Count);

        IGameEvent selectedEvent = selectedList[index];

        Debug.Log($"Evento disparado: {selectedEvent.EventName}");

        // Executa o evento.
        selectedEvent.Execute();

        // Adiciona o ícone correspondente na HUD.
        if (upgradeIconController != null)
        {
            upgradeIconController.AddEventIcon(
                selectedEvent.EventName,
                isGood
            );
        }
        else
        {
            Debug.LogError(
                "[EventManager] UpgradeIconController não atribuído!"
            );
        }

        // Remove o evento para não ser sorteado novamente.
        selectedList.RemoveAt(index);
    }



    private void OnDestroy()
    {
        spikeFieldEvent?.Stop();

        foreach (IGameEvent gameEvent in goodEvents)
        {
            if (gameEvent is LightningStrikeEvent lightningEvent)
                lightningEvent.Stop();
        }
    }
}
