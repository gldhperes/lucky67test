
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeIconController : MonoBehaviour
{
    [Serializable]
    private class EventIconEntry
    {
        public string eventName;
        public bool isPositive;
        public Sprite icon;
    }

    [Header("References")]
    [SerializeField] private Transform iconContainer;
    [SerializeField] private GameObject iconPrefab;

    [Header("Pause Reference")]
    [SerializeField] private PausePanel pausePanel;

    [Header("Registered Event Icons")]
    [SerializeField] private List<EventIconEntry> eventIcons = new();

    public void AddEventIcon(string eventName, bool isPositive)
    {
        EventIconEntry entry = eventIcons.Find(x => x.eventName == eventName && x.isPositive == isPositive);

        if (entry == null)
        {
            Debug.LogWarning(
                $"[UpgradeIconController] No icon registered for: " +
                $"{eventName} | Positive: {isPositive}"
            );
            return;
        }

        if (entry.icon == null)
        {
            Debug.LogWarning(
                $"[UpgradeIconController] Sprite missing: {eventName}"
            );
            return;
        }

        if (iconContainer == null || iconPrefab == null)
        {
            Debug.LogError(
                "[UpgradeIconController] Container or Prefab missing!"
            );
            return;
        }

        // Cria uma cópia do prefab dentro do Content.
        GameObject newIcon = Instantiate(iconPrefab, iconContainer);

        // Obtém o componente Image do prefab.
        Image image = newIcon.GetComponent<Image>();

        if (image == null)
        {
            Debug.LogError("[UpgradeIconController] Prefab has no Image component!");

            Destroy(newIcon);
            return;
        }

        // Aplica o Sprite cadastrado.
        image.sprite = entry.icon;
        image.preserveAspect = true;

        // Coloca o novo ícone no início da lista.
        newIcon.transform.SetAsFirstSibling();

        Debug.Log(
            $"[UpgradeIconController] Added icon: {eventName}"
        );

        // Configura o tooltip do ícone.
        UpgradeIconTooltip tooltip =
            newIcon.GetComponent<UpgradeIconTooltip>();

        if (tooltip != null)
        {
            string suffix = isPositive ? " Up" : " Down";

            string displayName = eventName + suffix;

            Color textColor = isPositive ? Color.green : Color.red;

            tooltip.Initialize(displayName, textColor, pausePanel);
        }

    }

    public void ClearIcons()
    {
        if (iconContainer == null)
            return;

        foreach (Transform child in iconContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
