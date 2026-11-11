
using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject runner = new GameObject("CoroutineRunner");
                instance = runner.AddComponent<CoroutineRunner>();

                DontDestroyOnLoad(runner);
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Coroutine StartRoutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void StopRoutine(Coroutine routine)
    {
        if (routine != null)
            StopCoroutine(routine);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
