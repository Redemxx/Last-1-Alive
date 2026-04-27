using UnityEngine;

public class SmashLight : MonoBehaviour
{
    private Health health;
    private AudioSource audioSource;
    private GameObject lightCover;
    private bool isSmashed = false;

    void Start()
    {
        health = GetComponent<Health>();
        health.onDeath += Smash;

        audioSource = GetComponent<AudioSource>();
        lightCover = transform.Find("Cover").gameObject;
        lightCover.SetActive(false);
    }

    public void Smash(GameObject obj)
    {
        if (isSmashed) return;

        isSmashed = true;
        foreach (Light light in GetComponentsInChildren<Light>())
        {
            light.enabled = false;
        }

        audioSource.Play();
        lightCover.SetActive(true);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSmashed)
        {
            health.TakeDamage(other.gameObject, 9999);
        }
    }
}
