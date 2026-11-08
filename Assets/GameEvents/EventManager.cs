using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    private List<IGameEvent> goodEvents = new();
    private List<IGameEvent> badEvents = new();

    private void Awake()
    {
        InitializeEvents();
    }

    private void InitializeEvents()
    {
        // Eventos bons
        goodEvents.Add(new RicochetBulletEvent());

        // Eventos ruins
        // badEvents.Add(new ReducePlayerFireRateEvent(0.5f));
        // badEvents.Add(new ReducePlayerHealthEvent(10));
        // badEvents.Add(new ReducePlayerSpeedEvent(.12f));
    }

    public void ExecuteRandomEvent()
    {
        bool isGood = Random.value < 0.5f;

        // TODO >>> REMOVER DEPOIS DO TESTE
        // bool isGood = false;

        List<IGameEvent> selectedList = isGood ? goodEvents : badEvents;

        if (selectedList.Count == 0)
            return;

        int index = Random.Range(0, selectedList.Count);

        IGameEvent selectedEvent = selectedList[index];

        Debug.Log($"Evento disparado: {selectedEvent.EventName}");
        selectedEvent.Execute();

        selectedList.RemoveAt(index);
    }
}