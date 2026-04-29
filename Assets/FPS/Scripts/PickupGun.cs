using UnityEngine;

public class PickupGun : InteractAction
{
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
        GameObject newGun = Instantiate(gunState.weaponPrefab, player.holder.transform);
        newGun.transform.localPosition = Vector3.zero;
        newGun.transform.localRotation = Quaternion.identity;
        playerController.PlayPickupSound();

        GunState otherGun = newGun.GetComponent<GunState>();
        if (otherGun != null)
        {
            otherGun.transferredAmmo = true;
            otherGun.currentAmmo = gunState.currentAmmo;
            otherGun.currentMag = gunState.currentMag;
            otherGun.magazineSize = gunState.magazineSize;
            otherGun.maxAmmo = gunState.maxAmmo;
            otherGun.weaponPrefab = gunState.weaponPrefab;
        }
        player.PickupGun(newGun);
        Destroy(gameObject);
        return true;
    }
}
