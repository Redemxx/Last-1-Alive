using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    private PlayerInput playerInput;
    public GunController gun;
    private bool isShooting = false;

    void Start()
    {
        playerInput = new PlayerInput();
    }

    void OnShoot()
    {
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
    }

    void Update()
    {
        if (gun != null && isShooting) { 
            gun.Shoot();
        }
    }
}
