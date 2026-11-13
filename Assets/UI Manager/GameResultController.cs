
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameResultController : MonoBehaviour
{
    [Header("Backgrounds")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite winBackground;
    [SerializeField] private Sprite loseBackground;

    [Header("Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        ApplyResult();

        playAgainButton.onClick.AddListener(PlayAgain);
        mainMenuButton.onClick.AddListener(MainMenu);
    }

    private void ApplyResult()
    {
        switch (GameResultData.Result)
        {
            case GameResultType.Win:
                backgroundImage.sprite = winBackground;
                break;

            case GameResultType.Lose:
                backgroundImage.sprite = loseBackground;
                break;

            default:
                Debug.LogWarning(
                    "[GameResultController] Nenhum resultado definido!"
                );
                break;
        }
    }

    private void PlayAgain()
    {
        Time.timeScale = 1f;

        GameResultData.Reset();

        SceneTransitionManager.Instance.LoadScene("Game");
    }

    private void MainMenu()
    {
        Time.timeScale = 1f;

        GameResultData.Reset();

        SceneTransitionManager.Instance.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        if (playAgainButton != null)
            playAgainButton.onClick.RemoveListener(PlayAgain);

        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(MainMenu);
    }
}
