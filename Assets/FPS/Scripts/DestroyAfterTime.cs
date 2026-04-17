using Unity.VisualScripting;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float timeToDestroy = 2f;
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
