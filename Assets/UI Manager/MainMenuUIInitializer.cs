
using System.Collections;
using UnityEngine;
using TMPro;

public class MainMenuUIInitializer : MonoBehaviour
{
    [Header("Title Animation")]
    [SerializeField] private RectTransform titleText;

    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 targetPosition;

    [SerializeField] private float fallDuration = 0.35f;
    [SerializeField] private float overshootDistance = 45f;
    [SerializeField] private float bounceDuration = 0.2f;
    [SerializeField] private int bounceCount = 3;

    [Header("Panel Fade In")]
    [SerializeField] private GameObject mainMenuPanel;

    [SerializeField] private float delayBeforeFade = 0.5f;
    [SerializeField] private float fadeDuration = 0.8f;

    private CanvasGroup panelCanvasGroup;

    private void Awake()
    {
        if (mainMenuPanel != null)
        {
            // Obtém ou adiciona automaticamente o CanvasGroup.
            panelCanvasGroup = mainMenuPanel.GetComponent<CanvasGroup>();

            if (panelCanvasGroup == null)
                panelCanvasGroup = mainMenuPanel.AddComponent<CanvasGroup>();

            // Inicializa o painel invisível.
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;

            mainMenuPanel.SetActive(false);
        }
    }

    private void Start()
    {
        StartCoroutine(PlayInitialization());
    }

    private IEnumerator PlayInitialization()
    {
        // Animação do título.
        if (titleText != null)
        {
            titleText.anchoredPosition = startPosition;

            yield return StartCoroutine(AnimateTitle());
        }

        // Aguarda antes de mostrar o painel.
        yield return new WaitForSecondsRealtime(delayBeforeFade);

        // Executa o Fade In.
        if (mainMenuPanel != null && panelCanvasGroup != null)
        {
            mainMenuPanel.SetActive(true);

            yield return StartCoroutine(FadeInPanel());
        }
    }

    private IEnumerator AnimateTitle()
    {
        Vector2 overshootPosition =
            targetPosition + Vector2.down * overshootDistance;

        // Queda rápida.
        float elapsed = 0f;

        while (elapsed < fallDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / fallDuration);
            float easedProgress = progress * progress * progress;

            titleText.anchoredPosition = Vector2.Lerp(
                startPosition,
                overshootPosition,
                easedProgress
            );

            yield return null;
        }

        titleText.anchoredPosition = overshootPosition;

        // Bounces progressivamente menores.
        Vector2 previousPosition = overshootPosition;

        for (int i = 0; i < bounceCount; i++)
        {
            float amplitude = overshootDistance *
                Mathf.Pow(0.45f, i + 1);

            Vector2 bouncePosition =
                targetPosition + Vector2.up * amplitude;

            elapsed = 0f;

            while (elapsed < bounceDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float progress = Mathf.Clamp01(elapsed / bounceDuration);

                float easedProgress =
                    1f - Mathf.Pow(1f - progress, 3f);

                titleText.anchoredPosition = Vector2.Lerp(
                    previousPosition,
                    bouncePosition,
                    easedProgress
                );

                yield return null;
            }

            titleText.anchoredPosition = bouncePosition;
            previousPosition = bouncePosition;
        }

        // Estabiliza na posição final.
        elapsed = 0f;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / bounceDuration);

            float easedProgress =
                1f - Mathf.Pow(1f - progress, 3f);

            titleText.anchoredPosition = Vector2.Lerp(
                previousPosition,
                targetPosition,
                easedProgress
            );

            yield return null;
        }

        titleText.anchoredPosition = targetPosition;
    }

    private IEnumerator FadeInPanel()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / fadeDuration);

            panelCanvasGroup.alpha = progress;

            yield return null;
        }

        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;
    }
}
