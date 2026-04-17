using UnityEngine;
using System.Collections;
using System;

public class GunController : MonoBehaviour
{
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private int magazineSize = 10;
    [SerializeField] private int maxAmmo = 60;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float recoilDistance = 0.1f;
    [SerializeField] private float recoilReturnSpeed = 0.12f;
    [SerializeField] private GameObject weaponFlash;

    private int currentMag;
    private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private Vector3 reloadRotationOffset = new Vector3(66, 50, 50);

    void Start()
    {
        currentMag = magazineSize;
        currentAmmo = maxAmmo;
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
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
        GameObject flash = Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        flash.transform.SetParent(bulletSpawnPoint.transform);

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
        float halfReload = reloadTime / 2f;
        float t = 0f;

        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, t / halfReload);
            yield return null;
        }

        t = 0f;

        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(targetRotation, initialRotation, t / halfReload);
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

        while (t < recoilReturnSpeed)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(initialPosition, recoilTarget, t);
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
