using UnityEngine;
using System.Collections;
using System;

public class GunController : MonoBehaviour
{
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private Vector3 reloadRotationOffset = new Vector3(66, 50, 50);
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private int magazineSize = 10;
    [SerializeField] private float magPullDistance = 0.3f;
    [SerializeField] private Vector3 magPullDirection = new Vector3(0, -1, 0);
    [SerializeField] private int maxAmmo = 60;
    [SerializeField] private int baseDamage = 1;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float recoilDistance = 0.1f;
    [SerializeField] private float recoilReturnSpeed = 0.12f;
    [SerializeField] private GameObject weaponFlash;

    private Transform bulletSpawnPoint;
    private Transform flashSpawnPoint;
    private GameObject mag;
    private Camera playerCam;

    private int currentMag;
    private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private Vector3 initialMagPosition;
    

    void Start()
    {
        playerCam = GetComponentInParent<Camera>();
        currentMag = magazineSize;
        currentAmmo = maxAmmo;
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        bulletSpawnPoint = transform.Find("BulletSpawnPoint");
        flashSpawnPoint = transform.Find("FlashSpawnPoint");
        mag = transform.Find("Mag").gameObject;
        initialMagPosition = mag.transform.localPosition;
    }

    public void Shoot()
    {
        if (isReloading) return;
        if (Time.time < nextTimeToFire) return;

        if (currentMag <= 0)
        {
            TryReload();
            return;
        }

        nextTimeToFire = Time.time + 1f / fireRate;
        currentMag--;

        Debug.Log("Bang! Bullets left in mag: " + currentMag);
        Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        GameObject flash = Instantiate(weaponFlash, flashSpawnPoint.position, flashSpawnPoint.rotation);
        flash.transform.SetParent(flashSpawnPoint.transform);

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Health target = hit.collider.GetComponent<Health>();
            if (target != null)
            {
                target.TakeDamage(gameObject, baseDamage);
            }
        }

        StopCoroutine(nameof(Recoil));
        StartCoroutine(nameof(Recoil));
    }

    IEnumerator Reload()
    {
        if (isReloading) yield break;
        if (currentMag == magazineSize) yield break;
        if (currentAmmo == 0) yield break;

        int ammoToLoad = Math.Min(magazineSize - currentMag, currentAmmo);
        if (ammoToLoad == 0) yield break;

        isReloading = true;

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

        currentMag += ammoToLoad;
        currentAmmo -= ammoToLoad;
        isReloading = false;
    }

    public void TryReload()
    {
        if (isReloading) return;
        if (currentMag == magazineSize) return;
        StartCoroutine(Reload());
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
}
