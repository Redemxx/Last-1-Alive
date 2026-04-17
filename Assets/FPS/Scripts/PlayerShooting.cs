using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject holder;

    private GunController gun;
    private bool isShooting = false;
    private PlayerInput playerInput;

    void Start()
    {
        playerInput = new PlayerInput();
        
        GameObject foundGun = holder.transform.GetChild(0).gameObject;
        if (foundGun) gun = foundGun.GetComponent<GunController>();
    }

    void OnShoot()
    {
        if (gun == null) Debug.Log("Gun not found in holder!");
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
