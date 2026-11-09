
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class PausePanel : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject howToPlayPanel;


    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.4f;

    private CanvasGroup howToPlayCanvasGroup;
    private RectTransform howToPlayRect;

    private Vector2 howToPlayInitialPosition;
    private Coroutine currentAnimation;

    private void Start()
    {
        menuPanel.SetActive(true);
        howToPlayPanel.SetActive(false);

        howToPlayCanvasGroup = howToPlayPanel.GetComponent<CanvasGroup>();

        if (howToPlayCanvasGroup == null)
            howToPlayCanvasGroup = howToPlayPanel.AddComponent<CanvasGroup>();

        howToPlayRect = howToPlayPanel.GetComponent<RectTransform>();

        howToPlayInitialPosition = howToPlayRect.anchoredPosition;

        howToPlayCanvasGroup.alpha = 0f;
        howToPlayCanvasGroup.interactable = false;
        howToPlayCanvasGroup.blocksRaycasts = false;


        // Botoes Config 
        howToPlayButton.onClick.AddListener(() => OpenHowToPlay());
        backButton.onClick.AddListener(() => Back());

        CheckContext(OnMainMenu, OnGame);
    }

    private void OnMainMenu()
    {
        playButton.onClick.AddListener(() =>
        {
            // Carrega a cena de play
        });

        quitButton.onClick.AddListener(() =>
        {
            // Carrega a saida do jogo
        });
    }

    private void OnGame()
    {
        playButton.GetComponentInChildren<TMP_Text>().text = "Continue";
        playButton.onClick.AddListener(() => ClosePausePanel());

        // botao de quit voltar para MainMenu
        quitButton.onClick.AddListener(() =>
        {
            // Carrega a cena de MainMenu
        });
    }

    public void ClosePausePanel() { }

    public void CheckContext(Action OnMainMenu, Action OnGame)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        switch (currentScene)
        {
            case "MainMenu":
                OnMainMenu?.Invoke();
                break;

            case "Game":
                OnGame?.Invoke();
                break;
        }
    }


    // HOW TO PLAY
    public void OpenHowToPlay()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(ShowHowToPlay());
    }

    private IEnumerator ShowHowToPlay()
    {
        menuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);

        howToPlayCanvasGroup.alpha = 0f;
        howToPlayCanvasGroup.interactable = false;
        howToPlayCanvasGroup.blocksRaycasts = false;

        Vector2 startPosition = howToPlayInitialPosition +
                                Vector2.right * howToPlayRect.rect.width;

        howToPlayRect.anchoredPosition = startPosition;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsedTime / animationDuration);
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            howToPlayRect.anchoredPosition = Vector2.Lerp(
                startPosition,
                howToPlayInitialPosition,
                smoothProgress
            );

            howToPlayCanvasGroup.alpha = smoothProgress;

            yield return null;
        }

        howToPlayRect.anchoredPosition = howToPlayInitialPosition;
        howToPlayCanvasGroup.alpha = 1f;

        howToPlayCanvasGroup.interactable = true;
        howToPlayCanvasGroup.blocksRaycasts = true;

        currentAnimation = null;
    }

    // BACK
    public void Back()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(HideHowToPlay());
    }

    private IEnumerator HideHowToPlay()
    {
        howToPlayCanvasGroup.interactable = false;
        howToPlayCanvasGroup.blocksRaycasts = false;

        Vector2 startPosition = howToPlayRect.anchoredPosition;

        Vector2 targetPosition = howToPlayInitialPosition +
                                 Vector2.right * howToPlayRect.rect.width;

        float startAlpha = howToPlayCanvasGroup.alpha;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsedTime / animationDuration);
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            howToPlayRect.anchoredPosition = Vector2.Lerp(
                startPosition,
                targetPosition,
                smoothProgress
            );

            howToPlayCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                0f,
                smoothProgress
            );

            yield return null;
        }

        howToPlayCanvasGroup.alpha = 0f;

        howToPlayPanel.SetActive(false);
        menuPanel.SetActive(true);

        howToPlayRect.anchoredPosition = howToPlayInitialPosition;

        currentAnimation = null;
    }

    // QUIT
    public void Quit()
    {
        Application.Quit();

        // Útil durante os testes no Editor:
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
