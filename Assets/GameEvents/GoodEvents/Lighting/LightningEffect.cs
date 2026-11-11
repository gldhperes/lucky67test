using UnityEngine;

public class LightningEffect : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.9f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}