using UnityEngine;

public class SmashLight : MonoBehaviour
{
    Health health;
    AudioSource audioSource;
    GameObject lightCover;
    void Start()
    {
        health = GetComponentInChildren<Health>();
        health.onDeath += Smash;

        audioSource = GetComponentInChildren<AudioSource>();
        lightCover = transform.Find("Cover").gameObject;
        lightCover.SetActive(false);
    }

    public void Smash(GameObject obj)
    {
        Debug.Log("Smash light!");
        foreach (Light light in GetComponentsInChildren<Light>())
        {
            light.enabled = false;
        }

        audioSource.Play();
        lightCover.SetActive(true);
    }
}
