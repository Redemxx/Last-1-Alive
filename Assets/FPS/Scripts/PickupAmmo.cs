using System.Collections;
using UnityEngine;

public class PickupAmmo : InteractAction
{
    [SerializeField] private int ammoCount = 300;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip noAction;

    public bool hasAmmo = true;
    private Transform hinge;
    private AudioSource audioSource;
    private bool looted = false;
    private GameObject flare;

    void Start()
    {
        hinge = transform.Find("Hinge");
        audioSource = GetComponent<AudioSource>();
        flare = hinge.Find("Top").Find("Flare").gameObject;
    }

    public override bool InvokeAction()
    {
        if (ammoCount <= 0) return false;

        PlayerShooting player = Object.FindFirstObjectByType<PlayerShooting>();
    
        if (player.gun == null) {
            audioSource.PlayOneShot(noAction);
            return false;
        };

        GunController gunController = player.gun.GetComponent<GunController>();

        int loaded = gunController.ReloadAmmo(ammoCount);
        ammoCount -= loaded;

        if (loaded == 0) return false;

        if (ammoCount <= 0) { 
            Destroy(flare);
            StartCoroutine(CloseCase());
            return true;
        }

        if (!looted)
        {
            looted = true;
            StartCoroutine(OpenCase());
            audioSource.PlayOneShot(openSound);
        }
        return false;
    }

    private IEnumerator OpenCase()
    {
        Quaternion initialRotation = hinge.localRotation;
        Vector3 rotationOffset = new Vector3(-140, 0, 0);
        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles + rotationOffset);

        float t = 0f;
        float openTime = 0.6f;
        while (t < openTime)
        {
            t += Time.deltaTime;
            hinge.localRotation = Quaternion.Slerp(initialRotation, targetRotation, t / openTime);
            yield return null;
        }
    }

    private IEnumerator CloseCase()
    {
        Quaternion initialRotation = hinge.localRotation;
        Vector3 rotationOffset = new Vector3(-140, 0, 0);
        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles + rotationOffset);

        float t = 0f;
        float openTime = 0.6f;
        while (t < openTime)
        {
            t += Time.deltaTime;
            hinge.localRotation = Quaternion.Slerp(initialRotation, targetRotation, t / openTime);
            yield return null;
        }
    }
}
