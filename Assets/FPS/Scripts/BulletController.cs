using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public int damage = 1;
    public Vector3 sourcePoint = Vector3.zero;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = -transform.forward * speed;
        rb.mass = damage / 10f;
        sourcePoint = rb.position;
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        Debug.Log(collision.gameObject.name);
        Destroy(gameObject);
    }
}
