
using TMPro;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        float elapsedTime = GameManager.Instance.ElapsedTime;

        int seconds = Mathf.FloorToInt(elapsedTime);
        int centiseconds = Mathf.FloorToInt((elapsedTime % 1f) * 100f);

        timerText.text = $"{seconds:00}:{centiseconds:00}";
    }
}
