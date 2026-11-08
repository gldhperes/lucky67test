
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    [Header("References")]
    [SerializeField] private EventManager eventManager;

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 67f;
    [SerializeField] private float eventInterval = 6.7f;

    [Header("Game State")]
    [SerializeField] private float elapsedTime = 0f;
    public float ElapsedTime => elapsedTime;

    private float eventTimer = 0f;

    public bool GameEnded { get; private set; }
    public bool PlayerWon { get; private set; }

    // Eventos para outros sistemas escutarem
    public event Action OnGameWon;
    public event Action OnGameLost;
    public event Action OnEventInterval;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (GameEnded)
            return;

        elapsedTime += Time.deltaTime;
        eventTimer += Time.deltaTime;

        // Verifica se passaram 67 segundos
        if (elapsedTime >= gameDuration)
        {
            WinGame();
            return;
        }

        // Verifica se passaram 6.7 segundos
        if (eventTimer >= eventInterval)
        {
            eventTimer -= eventInterval;

            TriggerRandomEvent();
        }
    }

    private void TriggerRandomEvent()
    {
        Debug.Log("Passaram 6.7 segundos!");

        eventManager.ExecuteRandomEvent();

        OnEventInterval?.Invoke();
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

        OnGameWon?.Invoke();
    }

    private void LoseGame()
    {
        GameEnded = true;
        PlayerWon = false;

        Debug.Log("PLAYER PERDEU!");

        OnGameLost?.Invoke();
    }

    // Retorna o tempo restante da partida
    public float GetRemainingTime()
    {
        return Mathf.Max(0f, gameDuration - elapsedTime);
    }

    // Retorna o tempo decorrido
    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
