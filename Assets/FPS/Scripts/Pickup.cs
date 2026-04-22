using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Pickup : MonoBehaviour
{
    public Material highlightMaterial;
    public GameObject weaponPrefab;
    public float lookRange = 3f;

    private Material[] originalMaterials;
    private MeshRenderer[] meshRenderers;
    private bool isLookedAt = false;
    private Camera playerCam;
    private PlayerShooting player;

    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++) {
            originalMaterials[i] = meshRenderers[i].material;
        }

        player = Object.FindFirstObjectByType<PlayerShooting>();
        playerCam = player.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, lookRange) && hit.collider.GetComponentInParent<Pickup>() == this)
        {
            if (!isLookedAt) SetLookedAt(true);
            return;
        }
        
        if (isLookedAt) SetLookedAt(false);
    }

    void SetLookedAt(bool lookedAt)
    {
        isLookedAt = lookedAt;
        for (int i = 0; i < meshRenderers.Length; i++) {
            meshRenderers[i].material = lookedAt ? highlightMaterial : originalMaterials[i];
        }
    }

    public void OnPickup()
    {
        UnityEngine.Debug.Log("Pickup attempted");
        if (!isLookedAt) return;

        if (player.gun != null)
            Destroy(player.gun.gameObject);

        UnityEngine.Debug.Log("Gun picked up");
        GameObject newGun = Instantiate(weaponPrefab, player.holder.transform);
        newGun.transform.localPosition = Vector3.zero;
        newGun.transform.localRotation = Quaternion.identity;
        player.gun = newGun.GetComponent<GunController>();

        Destroy(gameObject);
    }
}
