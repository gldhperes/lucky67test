
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    private bool isLoading;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        AudioManager.Instance.StopMusic();

        if (isLoading)
            return;

        if (SceneManager.GetActiveScene().name == sceneName)
            return;

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoading = true;

        // Garante que o jogo não esteja pausado
        Time.timeScale = 1f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        if (operation == null)
        {
            Debug.LogError($"Não foi possível carregar a cena: {sceneName}");
            isLoading = false;
            yield break;
        }

        // Impede a ativação automática da cena
        operation.allowSceneActivation = false;

        // Aguarda o carregamento completo (90%)
        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        // A cena está pronta para ser ativada
        operation.allowSceneActivation = true;

        // Aguarda a ativação terminar
        while (!operation.isDone)
        {
            yield return null;
        }

        isLoading = false;
    }
}
