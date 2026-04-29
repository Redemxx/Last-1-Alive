using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject holder;
    public GunController gun;

    private bool isShooting = false;
    private PlayerInput playerInput;
    public PlayerController playerController { get; private set; }
    private Camera playerCam;
    private float baseFOV;
    private List<Collider> zombies = new List<Collider>();

    void Start()
    {
        playerInput = new PlayerInput();
        playerController = GetComponent<PlayerController>();
        playerCam = GetComponentInChildren<Camera>();
        baseFOV = playerCam.fieldOfView;

        if (holder.transform.childCount > 0)
        {
            Transform gunTransform = holder.transform.GetChild(0);
            GameObject foundGun = gunTransform.gameObject;
            if (foundGun) gun = foundGun.GetComponent<GunController>();
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
        playerController.AimDownSights(gun.aimDownSightsFOV / 100);
        playerCam.fieldOfView = gun.aimDownSightsFOV;
    }

    void OnZoomEnd()
    {
        playerController.AimDownSights();
        playerCam.fieldOfView = baseFOV;
    }

    public void OnDrop()
    {
        if (Time.timeScale == 0) return;
        if (gun == null) return;

        gun.Drop();
        gun = null;
    }

    void Update()
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
