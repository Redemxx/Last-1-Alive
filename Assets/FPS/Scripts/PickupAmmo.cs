using System.Collections;
using UnityEngine;

public class PickupAmmo : InteractAction
{
    [SerializeField, Range(0.1f, 1.0f)] private float reloadWeight = 1f;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip noAction;

    public bool hasAmmo = true;
    private Transform hinge;
    private AudioSource audioSource;
    private bool looted = false;

    void Start()
    {
        hinge = transform.Find("Hinge");
        audioSource = GetComponent<AudioSource>();
    }

    public override bool InvokeAction()
    {
        if (looted) return true;

        PlayerShooting player = Object.FindFirstObjectByType<PlayerShooting>();
    
        if (player.gun == null) {
            audioSource.PlayOneShot(noAction);
            return false;
        };

        GunController gunController = player.gun.GetComponent<GunController>();

        if (!gunController.FullReload(reloadWeight)) return false;
        looted = true;
        StartCoroutine(OpenCase());
        audioSource.PlayOneShot(openSound);
        return true;
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
}
