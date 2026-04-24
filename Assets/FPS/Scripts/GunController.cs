using UnityEngine;
using System.Collections;
using System;

public class GunController : MonoBehaviour
{
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private Vector3 reloadRotationOffset = new Vector3(66, 50, 50);
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float magPullDistance = 0.3f;
    [SerializeField] private Vector3 magPullDirection = new Vector3(0, -1, 0);
    [SerializeField] private int baseDamage = 1;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float recoilDistance = 0.1f;
    [SerializeField] private float recoilReturnSpeed = 0.12f;
    [SerializeField] private GameObject droppedVariant;
    [SerializeField] private GameObject weaponFlash;
    [SerializeField] private AudioClip gunSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip magEmpty;
    [Range(10f, 100f)] public float aimDownSightsFOV = 40f;

    private Transform bulletSpawnPoint;
    private GameObject mag;
    private Camera playerCam;
    private AudioSource internalSound;
    public GunState gunState;

    private bool isReloading = false;
    private float nextTimeToFire = 0f;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private Vector3 initialMagPosition;
    private LayerMask hitMask;


    void Start()
    {
        playerCam = GetComponentInParent<Camera>();
        internalSound = GetComponentInParent<AudioSource>();
        gunState = GetComponent<GunState>();

        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        bulletSpawnPoint = transform.Find("BulletSpawnPoint");
        mag = transform.Find("Mag").gameObject;
        initialMagPosition = mag.transform.localPosition;

        hitMask = LayerMask.GetMask("Zombies", "Breakables");
    }

    public bool CheckCooldown()
    {
        return Time.time >= nextTimeToFire;
    }

    public void Shoot()
    {
        if (isReloading) return;
        if (Time.time < nextTimeToFire) return;

        if (gunState.currentMag <= 0) return;

        nextTimeToFire = Time.time + 1f / fireRate;
        gunState.currentMag--;

        internalSound.PlayOneShot(gunSound);

        Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        GameObject flash = Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        flash.transform.SetParent(bulletSpawnPoint.transform);

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hitMask))
        {
            Health target = hit.collider.GetComponent<Health>();
            if (target != null)
            {
                target.TakeDamage(gameObject, baseDamage);
                internalSound.PlayOneShot(hitSound);
            }
        }


        StopCoroutine(nameof(Recoil));
        StartCoroutine(nameof(Recoil));
    }

    IEnumerator Reload()
    {
        if (isReloading) yield break;
        if (gunState.currentMag == gunState.magazineSize) yield break;
        if (gunState.currentAmmo == 0) {
            internalSound.PlayOneShot(magEmpty);
            yield break;
        };

        if (!gunState.Reload()) yield break;

        isReloading = true;
        internalSound.PlayOneShot(reloadSound);

        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles + reloadRotationOffset);
        Vector3 magTarget = mag.transform.localPosition + magPullDirection * magPullDistance;
        float halfReload = reloadTime / 2f;
        float t = 0f;

        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, t / halfReload);
            mag.transform.localPosition = Vector3.Lerp(initialMagPosition, magTarget, t / halfReload);
            yield return null;
        }

        t = 0f;

        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(targetRotation, initialRotation, t / halfReload);
            mag.transform.localPosition = Vector3.Lerp(magTarget, initialMagPosition, t / halfReload);
            yield return null;
        }

        isReloading = false;
    }

    public bool TryReload()
    {
        if (isReloading) return false;
        if (gunState.currentMag == gunState.magazineSize) return false;
        StartCoroutine(Reload());
        return true;
    }

    public bool FullReload(float reloadWeight)
    {
        if (gunState.currentAmmo == gunState.maxAmmo && gunState.currentMag == gunState.magazineSize) return false;

        int needInMag = gunState.magazineSize - gunState.currentMag;
        gunState.currentAmmo = Mathf.RoundToInt(gunState.maxAmmo * reloadWeight) + needInMag;
    
        if (needInMag > 0)
            StartCoroutine(Reload());
        return true;
    }

    private IEnumerator Recoil()
    {
        Vector3 recoilTarget = initialPosition + new Vector3(0, 0, recoilDistance);
        float t = 0f;

        float kick = 0.1f;
        while (t < kick)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(initialPosition, recoilTarget, 1 / kick);
            yield return null;
        }

        t = 0f;

        while (t < recoilReturnSpeed)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(recoilTarget, initialPosition, t / recoilReturnSpeed);
            yield return null;
        }

        transform.localPosition = initialPosition;
    }

    public void Drop()
    {
        GameObject dropped = Instantiate(droppedVariant, transform.position, transform.rotation);
        GunState droppedGun = dropped.GetComponent<GunState>();
        if (droppedGun != null)
        {
            droppedGun.currentAmmo = gunState.currentAmmo;
            droppedGun.currentMag = gunState.currentMag;
            droppedGun.transferredAmmo = true;
        }
        Destroy(gameObject);
    }
}
