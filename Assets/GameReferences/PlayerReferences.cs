
using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    [field: SerializeField] public PlayerBehaviour Player { get; private set; }
    [field: SerializeField] public ProjectileBehaviour ProjectileBehaviour { get; private set; }

    private void Awake()
    {
        Player = FindFirstObjectByType<PlayerBehaviour>();
        ProjectileBehaviour = Player.projectilePrefab.GetComponent<ProjectileBehaviour>();
    }
}
