using Unity.VectorGraphics.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public GameObject holder;
    public GunController gun;

    private bool isShooting = false;
    private PlayerInput playerInput;
    private PlayerController playerController;
    private Camera playerCam;

    void Start()
    {
        playerInput = new PlayerInput();
        playerController = GetComponentInChildren<PlayerController>();
        playerCam = GetComponentInChildren<Camera>();

        Transform gunTransform = holder.transform.GetChild(0);
        GameObject foundGun = gunTransform.gameObject;
        if (foundGun) gun = foundGun.GetComponent<GunController>();
    }

    void OnShoot()
    {
        if (gun == null) return;
        isShooting = true;
    }

    void OnShootEnd()
    {
        isShooting = false;
    }

    void OnReload()
    {
        if (gun == null) return;

        gun.TryReload();
        OnZoomEnd();
    }

    void OnZoom()
    {
        if (gun == null) return;
        playerController.AimDownSights(gun.aimDownSightsFOV / 100);
        playerCam.fieldOfView = gun.aimDownSightsFOV;
    }

    void OnZoomEnd()
    {
        playerController.AimDownSights();
        playerCam.fieldOfView = 100f;
    }

    public void OnDrop()
    {
        if (gun == null) return;

        gun.Drop();
        gun = null;
    }

    void Update()
    {
        Debug.Log("Update called, isShooting: " + isShooting);
        Debug.Log(gun != null ? "Gun is not null" : "Gun is null");
        Debug.Log("Checking cooldown: " + (gun != null ? gun.CheckCooldown().ToString() : "No gun to check cooldown"));
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
            }
        }
    }
}
