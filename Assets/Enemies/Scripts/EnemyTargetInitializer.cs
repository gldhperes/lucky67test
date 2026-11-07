
using UnityEngine;

public class EnemyTargetInitializer : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (player == null)
        {
            Debug.LogWarning("Player não encontrado!");
            return;
        }

        EnemyBehaviour enemy = GetComponent<EnemyBehaviour>();

        if (enemy != null)
        {
            enemy.SetTarget(player.transform);
        }
    }
}
