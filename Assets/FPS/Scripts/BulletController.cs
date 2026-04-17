using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public int damage = 1;
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = -transform.forward * speed;
        rb.mass = damage / 10f;
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        Debug.Log(collision.gameObject.name);
        Destroy(gameObject);
    }
}
