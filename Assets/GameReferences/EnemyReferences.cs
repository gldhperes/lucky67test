
using UnityEngine;

public class EnemyReferences : MonoBehaviour
{
    [field: SerializeField] public EnemyBehaviour enemyBehaviour { get; private set; }

    private void Awake()
    {
        enemyBehaviour = FindFirstObjectByType<EnemyBehaviour>();
    }
}
