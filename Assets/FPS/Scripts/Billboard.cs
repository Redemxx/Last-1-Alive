using UnityEngine;

public class Billboard : MonoBehaviour
{
    public float maxDistance = 20f;

    private Transform camTransform;
    private SpriteRenderer spriteRenderer;
    private Light playerLight;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        camTransform = player.GetComponentInChildren<Camera>().transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerLight = player.GetComponentInChildren<Light>();
    }

    void LateUpdate()
    {
        float distance = Vector3.Distance(transform.position, camTransform.position);
        if (distance > maxDistance || !playerLight.enabled) { 
            spriteRenderer.enabled = false;
            return;
        }

        spriteRenderer.enabled = true;
        Color color = spriteRenderer.color;
        color.a = Mathf.Clamp01(1f - (distance / maxDistance));
        spriteRenderer.color = color;
        transform.rotation = camTransform.rotation;
        transform.position = transform.parent.position + Vector3.up * 0.1f;
    }
}
