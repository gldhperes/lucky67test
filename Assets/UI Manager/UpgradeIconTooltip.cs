
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeIconTooltip : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Tooltip References")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;

    private PausePanel pausePanel;
    private bool isPointerOver;

    private void Awake()
    {
        HideTooltip();
    }

    public void Initialize(string upgradeName, Color textColor, PausePanel pause)
    {
        pausePanel = pause;

        if (tooltipText != null)
        {
            tooltipText.text = upgradeName;
            tooltipText.color = textColor;
        }

        RefreshTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        RefreshTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        HideTooltip();
    }

    private void Update()
    {
        // Atualiza também quando o Pause é aberto
        // ou fechado enquanto o mouse está sobre o ícone.
        if (isPointerOver)
            RefreshTooltip();
    }

    private void RefreshTooltip()
    {
        if (tooltipPanel == null)
            return;

        bool shouldShow =
            isPointerOver &&
            pausePanel != null &&
            pausePanel.IsPaused;

        tooltipPanel.SetActive(shouldShow);
    }

    private void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}
