using UnityEngine;

public class PickupGun : InteractAction
{
    public GameObject weaponPrefab;
    private PlayerShooting player;
    private PlayerController playerController;
    private GunState gunState;

    void Start()
    {
        player = Object.FindFirstObjectByType<PlayerShooting>();
        playerController = Object.FindFirstObjectByType<PlayerController>();
        gunState = GetComponent<GunState>();
    }

    public override bool InvokeAction()
    {
        player.OnDrop();

        GameObject newGun = Instantiate(weaponPrefab, player.holder.transform);
        newGun.transform.localPosition = Vector3.zero;
        newGun.transform.localRotation = Quaternion.identity;
        player.gun = newGun.GetComponent<GunController>();

        GunState otherGun = newGun.GetComponent<GunState>();
        if (otherGun != null)
        {
            otherGun.transferredAmmo = true;
            otherGun.currentAmmo = gunState.currentAmmo;
            otherGun.currentMag = gunState.currentMag;
        }
        playerController.PlayPickupSound();
        Destroy(gameObject);
        return true;
    }
}
