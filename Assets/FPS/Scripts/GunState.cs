using UnityEngine;

public class GunState : MonoBehaviour
{

    public int magazineSize = 10;
    public int maxAmmo = 60;
    [HideInInspector] public int currentMag;
    [HideInInspector] public int currentAmmo;
    public bool transferredAmmo = false;

    void Start()
    {
        if (transferredAmmo) return;

        maxAmmo = Mathf.CeilToInt(maxAmmo * (1f - GameState.Instance.GameModifier()));

        currentMag = magazineSize;
        currentAmmo = maxAmmo;
    }

    public bool Reload()
    {
        int neededAmmo = magazineSize - currentMag;
        if (currentAmmo <= 0 || neededAmmo <= 0) return false;
        int ammoToLoad = Mathf.Min(neededAmmo, currentAmmo);
        currentMag += ammoToLoad;
        currentAmmo -= ammoToLoad;
        return true;
    }
}
