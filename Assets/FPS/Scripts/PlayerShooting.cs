using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerShooting : MonoBehaviour
{
    public GameObject holder;
    public GameObject secondaryHolder;
    public GunController gun;
    public GunController secondaryGun;

    private Health playerHealth;
    private bool isShooting = false;
    private PlayerInput playerInput;
    public PlayerController playerController { get; private set; }
    private Camera playerCam;
    private float baseFOV;
    private List<Collider> zombies = new List<Collider>();
    private bool zoomed = false;

    public Volume globalVolume;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;

    void Start()
    {
        playerInput = new PlayerInput();
        playerController = GetComponent<PlayerController>();
        playerCam = GetComponentInChildren<Camera>();
        baseFOV = playerCam.fieldOfView;
        playerHealth = GetComponent<Health>();

        if (holder.transform.childCount > 0)
        {
            Transform gunTransform = holder.transform.GetChild(0);
            GameObject foundGun = gunTransform.gameObject;
            if (foundGun) gun = foundGun.GetComponent<GunController>();
        }

        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out chromaticAberration);
    }

    public void PickupGun(GameObject newGun)
    {
        GunController newGunController = newGun.GetComponent<GunController>();
        if (newGunController == null) {
            return;
        }

        if (gun != null)
        {
            if (secondaryGun != null) { 
                OnDrop();
            }
            else
            {
                secondaryGun = gun;
                secondaryGun.transform.SetParent(secondaryHolder.transform);
                secondaryGun.transform.localPosition = Vector3.zero;
                secondaryGun.transform.localRotation = Quaternion.identity;
                secondaryGun.enabled = false;
            }
        }

        gun = newGunController;
        newGun.transform.SetParent(holder.transform);
        newGun.transform.localPosition = Vector3.zero;
        newGun.transform.localRotation = Quaternion.identity;
    }

    void OnScroll(InputValue inputValue)
    {
        Vector2 delta = inputValue.Get<Vector2>();
        if (Time.timeScale == 0) return;
        if (gun == null) return;
        if (delta.magnitude == 0) return;

        if (secondaryGun != null) {
            GunController temp = gun;
            gun = secondaryGun;
            secondaryGun = temp;
            
            gun.transform.SetParent(holder.transform);
            gun.transform.localPosition = Vector3.zero;
            gun.transform.localRotation = Quaternion.identity;
            gun.enabled = true;

            secondaryGun.transform.SetParent(secondaryHolder.transform);
            secondaryGun.transform.localPosition = Vector3.zero;
            secondaryGun.transform.localRotation = Quaternion.identity;
            secondaryGun.enabled = false;
            playerController.PlayPickupSound();

            if (zoomed) OnZoom();
        }
    }

    void OnShoot()
    {
        if (Time.timeScale == 0) return;
        if (gun == null) return;
        isShooting = true;
    }

    void OnShootEnd()
    {
        isShooting = false;
    }

    void OnReload()
    {
        if (Time.timeScale == 0) return;
        if (gun == null) return;

        gun.TryReload();
        OnZoomEnd();
    }

    void OnZoom()
    {
        if (Time.timeScale == 0) return;
        if (gun == null) return;
        zoomed = true;
        playerController.AimDownSights(gun.aimDownSightsFOV / 100);
        playerCam.fieldOfView = gun.aimDownSightsFOV;

        if (gun.aimDownSightsFOV < 30)
        {
            StartCoroutine(HandleVignette(0.8f, 0.18f));
        }
    }

    public IEnumerator HandleVignette(float targetIntensity, float duration)
    {
        if (targetIntensity > 0) vignette.active = true;
        bool wasZoomed = zoomed;

        float initialIntensity = vignette.intensity.value;
        float t = 0f;
        while (t < duration)
        {
            if (wasZoomed != zoomed) yield break;
            t += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(initialIntensity, targetIntensity, t / duration);
            yield return null;
        }

        if (targetIntensity == 0) vignette.active = false;
    }

    void OnZoomEnd()
    {
        playerController.AimDownSights();
        playerCam.fieldOfView = baseFOV;
        zoomed = false;
        StartCoroutine(HandleVignette(0f, 0.18f));
    }

    public void OnDrop()
    {
        if (Time.timeScale == 0) return;
        if (gun == null) return;

        gun.Drop();
        gun = null;
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0) return;
        if (gun != null && isShooting && gun.CheckCooldown()) { 
            if (gun.gunState.currentMag <= 0)
            {
                gun.TryReload();
                OnZoomEnd();
                isShooting = false;
            }
            else
            {
                gun.Shoot();

                foreach (Collider zombie in zombies)
                {
                    if (zombie == null) continue;
                    NormalZombieController zombieController = zombie.GetComponent<NormalZombieController>();
                    if (zombieController == null) continue;
                    zombieController.OnAlerted(gameObject);
                }
            }
        }

        chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, playerHealth.GetHealth() < 35 ? 0.8f : 0f, Time.deltaTime * 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Zombie"))
        {
            zombies.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Zombie"))
        {
            zombies.Remove(other);
        }
    }
}
