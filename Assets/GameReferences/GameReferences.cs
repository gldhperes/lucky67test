
using UnityEngine;

public class GameReferences : MonoBehaviour
{
    public static GameReferences Instance { get; private set; }

    [field: SerializeField] public PlayerReferences Player { get; private set; }
    [field: SerializeField] public EnemyReferences Enemy { get; private set; }
    [field: SerializeField] public UIReferences UI { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Player = GetComponent<PlayerReferences>();
        Enemy = GetComponent<EnemyReferences>();
        UI = GetComponent<UIReferences>();
    }
}
