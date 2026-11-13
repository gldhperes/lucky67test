
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const string INTRODUCTION_KEY = "Lucky67_IntroductionCompleted";

    [Header("References")]
    [SerializeField] private EventManager eventManager;
    [SerializeField] private PulseEffect pulseEffect;
    [SerializeField] private DialogueController dialogueController;

    [Header("Introduction Settings")]
    [SerializeField] private bool playIntroductionOnce = true;

    public bool IsIntroductionPlaying { get; private set; }
    public bool GameplayStarted { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 67f;
    [SerializeField] private float eventInterval = 6.7f;

    [Header("Game State")]
    [SerializeField] private float elapsedTime = 0f;

    public float ElapsedTime => elapsedTime;

    private float eventTimer = 0f;

    public bool GameEnded { get; private set; }
    public bool PlayerWon { get; private set; }



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Garante que a cena não herde um timeScale congelado.
        Time.timeScale = 1f;

        bool introductionCompleted = PlayerPrefs.GetInt(INTRODUCTION_KEY, 0) == 1;

        bool shouldPlayIntroduction = playIntroductionOnce && !introductionCompleted;

        if (shouldPlayIntroduction)
        {
            if (dialogueController == null)
            {
                Debug.LogError("[GameManager] DialogueController não foi atribuído!");

                GameplayStarted = true;
                return;
            }

            IsIntroductionPlaying = true;
            GameplayStarted = false;

            Time.timeScale = 0f;

            dialogueController.PlayIntroduction();
        }
        else
        {
            GameplayStarted = true;
            Debug.Log("[GameManager] Introdução ignorada. Gameplay iniciado.");

            AudioManager.Instance.PlayGameAudio();
        }
    }

    private void Update()
    {
        if (!GameplayStarted || GameEnded)
            return;

        elapsedTime += Time.deltaTime;
        eventTimer += Time.deltaTime;

        if (elapsedTime >= gameDuration)
        {
            WinGame();
            return;
        }

        if (eventTimer >= eventInterval)
        {
            eventTimer -= eventInterval;
            TriggerRandomEvent();
        }
    }

    public void CompleteIntroduction()
    {
        if (!IsIntroductionPlaying)
            return;

        IsIntroductionPlaying = false;
        GameplayStarted = true;

        if (playIntroductionOnce)
        {
            PlayerPrefs.SetInt(INTRODUCTION_KEY, 1);
            PlayerPrefs.Save();
        }

        Time.timeScale = 1f;

        Debug.Log("[GameManager] Introdução concluída. Teste 67 iniciado!");
        AudioManager.Instance.PlayGameAudio();
    }

    private void TriggerRandomEvent()
    {
        Debug.Log("Passaram 6.7 segundos!");

        if (eventManager != null)
            eventManager.ExecuteRandomEvent();

        if (pulseEffect != null)
            pulseEffect.PlayPulse();
    }

    public void PlayerDied()
    {
        if (GameEnded)
            return;

        LoseGame();
    }

    private void WinGame()
    {
        GameEnded = true;
        PlayerWon = true;

        Debug.Log("PLAYER VENCEU!");

        GameResultData.SetResult(GameResultType.Win);

        LoadGameResultScene();
    }

    private void LoseGame()
    {
        GameEnded = true;
        PlayerWon = false;

        Debug.Log("PLAYER PERDEU!");

        GameResultData.SetResult(GameResultType.Lose);

        LoadGameResultScene();
    }

    private void LoadGameResultScene()
    {
        SceneTransitionManager.Instance.LoadScene("GameResult");
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0f, gameDuration - elapsedTime);
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    [ContextMenu("Reset Introduction Progress")]
    private void ResetIntroductionProgress()
    {
        PlayerPrefs.DeleteKey(INTRODUCTION_KEY);
        PlayerPrefs.Save();

        Debug.Log("[GameManager] Progresso da introdução resetado.");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            if (IsIntroductionPlaying)
                Time.timeScale = 1f;

            Instance = null;
        }
    }
}
