
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private TMP_Text timerText;

    [Header("Health Bar ")]
    [SerializeField] private HealthBehaviour playerHealth;
    [SerializeField] private Slider healthSlider;

    private void Update()
    {
        UpdateTimer();
        UpdateHealthBar();
    }

    private void UpdateTimer()
    {
        float elapsedTime = GameManager.Instance.ElapsedTime;

        int seconds = Mathf.FloorToInt(elapsedTime);
        int centiseconds = Mathf.FloorToInt((elapsedTime % 1f) * 100f);

        timerText.text = $"{seconds:00}:{centiseconds:00}";
    }
    private void UpdateHealthBar()
    {
        float healthPercentage = playerHealth.CurrentHealth / playerHealth.MaxHealth;

        healthSlider.value = healthPercentage;
    }
}
