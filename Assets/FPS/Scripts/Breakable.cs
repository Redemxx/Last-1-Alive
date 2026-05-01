using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(AudioSource))]
public class Breakable : MonoBehaviour
{
    [SerializeField] private AudioClip breakSound;
    private AudioSource breakSoundSource;

    void Start()
    {
        Health health = GetComponent<Health>();
        health.onDeath += Break;
        breakSoundSource = GetComponent<AudioSource>();
        breakSoundSource.spatialBlend = 1f;
        breakSoundSource.playOnAwake = false;
    }

    public void Break(GameObject source)
    {
        breakSoundSource.PlayOneShot(breakSound);
        Destroy(gameObject);
    }
}
