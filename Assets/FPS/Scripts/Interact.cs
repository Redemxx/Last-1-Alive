using UnityEngine;

public class Interact : MonoBehaviour
{
    public Material highlightMaterial;
    public float lookRange = 3f;

    private InteractAction action;
    private Material[] originalMaterials;
    private MeshRenderer[] meshRenderers;
    private bool isLookedAt = false;
    private bool interacted = false;
    private Camera playerCam;

    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++) {
            originalMaterials[i] = meshRenderers[i].material;
        }

        playerCam = Object.FindFirstObjectByType<PlayerShooting>().GetComponentInChildren<Camera>();
        action = GetComponent<InteractAction>();
    }

    void Update()
    {
        if (interacted) return;

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, lookRange) && hit.collider.GetComponentInParent<Interact>() == this)
        {
            if (!isLookedAt) SetLookedAt(true);
        }
        else if (isLookedAt) SetLookedAt(false);

    }

    void SetLookedAt(bool lookedAt)
    {
        isLookedAt = lookedAt;
        for (int i = 0; i < meshRenderers.Length; i++) {
            meshRenderers[i].material = lookedAt ? highlightMaterial : originalMaterials[i];
        }
    }

    public void OnInteract()
    {
        if (!isLookedAt || interacted) return;
        interacted = action.InvokeAction();

        if (interacted) SetLookedAt(false);
    }
}
