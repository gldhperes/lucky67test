
using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    [field: SerializeField] public PlayerBehaviour Player { get; private set; }

    private void Awake()
    {
        Player = FindFirstObjectByType<PlayerBehaviour>();
    }
}
