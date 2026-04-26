using UnityEngine;

[RequireComponent(typeof(Health))]
public class Breakable : MonoBehaviour
{
    [SerializeField] private AudioClip breakSound;

    void Start()
    {
        Health health = GetComponent<Health>();
        health.onDeath += Break;
    }

    public void Break(GameObject source)
    {
        if (source.CompareTag("Player"))
        {
            PlayerController playerController = source.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.PlaySound(breakSound);
            }
        }
        Destroy(gameObject);
    }
}
