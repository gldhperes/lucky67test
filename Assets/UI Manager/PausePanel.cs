
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.InputSystem;

public class PausePanel : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject backgorundPanel;


    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.4f;

    [Header("Variables")]
#if UNITY_EDITOR
    [ReadOnly]
#endif
    [SerializeField] private bool isPaused;


    private CanvasGroup howToPlayCanvasGroup;
    private RectTransform howToPlayRect;

    private Vector2 howToPlayInitialPosition;
    private Coroutine currentAnimation;

    private void Start()
    {
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

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Game")
            return;

        if (Keyboard.current.pKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isPaused)
            ClosePausePanel();
        else
            OpenPausePanel();
    }

    private void OpenPausePanel()
    {
        isPaused = true;

        menuPanel.SetActive(true);
        backgorundPanel.SetActive(true);
        howToPlayPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    public void ClosePausePanel()
    {
        isPaused = false;

        menuPanel.SetActive(false);
        backgorundPanel.SetActive(false);
        howToPlayPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void OnMainMenu()
    {
        //TODO >>> Fazer script para animar o titulo e a chegada (fadeIN) desse objeto

        menuPanel.SetActive(true);
        howToPlayPanel.SetActive(false);

        playButton.onClick.AddListener(() =>
        {
            // Carrega a cena de play
            SceneTransitionManager.Instance.LoadScene("Game");
        });

        quitButton.onClick.AddListener(() =>
        {
            // Carrega a saida do jogo
            quitButton.onClick.AddListener(Quit);
        });
    }

    private void OnGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        menuPanel.SetActive(false);
        backgorundPanel.SetActive(false);
        howToPlayPanel.SetActive(false);

        backgorundPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.98f);

        playButton.GetComponentInChildren<TMP_Text>().text = "Continue".ToUpper();
        playButton.onClick.AddListener(() => ClosePausePanel());

        // botao de quit voltar para MainMenu
        quitButton.GetComponentInChildren<TMP_Text>().text = "Back to Menu".ToUpper();
        quitButton.onClick.AddListener(() =>
        {
            // Carrega a cena de MainMenu
            SceneTransitionManager.Instance.LoadScene("MainMenu");
        });
    }

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

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
