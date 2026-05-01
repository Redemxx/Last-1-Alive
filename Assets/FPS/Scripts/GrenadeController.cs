using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public float fuseTime = 10f;
    public float throwForce = 10f;
    public float throwChargeTime = 2f;
    public float pullBackDistance = 0.4f;
    public AudioClip pinPullSound;
    public AudioClip explosionSound;
    public LayerMask damageMask;

    private float pinPulledTime;
    private float explosionTime;
    private Rigidbody rb;
    private bool thrown = false;
    private bool exploded = false;

    private Transform remains;
    private AudioSource audioSource;
    private ParticleSystem explosionEffect;
    private PlayerShooting shooting;
    public GameObject explosionPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        remains = transform.GetChild(0);
        audioSource = remains.GetComponent<AudioSource>();
        shooting = GetComponentInParent<PlayerShooting>();
        explosionEffect = remains.GetComponentInChildren<ParticleSystem>();
    }

    void FixedUpdate()
    {
        if (!exploded && pinPulledTime != 0 && Time.time >= explosionTime)
        {
            exploded = true;
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
            foreach (Collider nearby in colliders)
            {
                Rigidbody nearbyRb = nearby.GetComponent<Rigidbody>();
                Health health = nearby.GetComponent<Health>();

                if (nearbyRb != null)
                {
                    nearbyRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }

                if (health != null)
                {
                    float distance = Vector3.Distance(transform.position, nearby.transform.position);
                    float damage = Mathf.Lerp(explosionForce, 0, distance / explosionRadius);
                    health.TakeDamage(shooting.gameObject, damage);
                    Debug.Log("Delt exlosive damage of " + damage + " to " + nearby.name);
                }
            }

            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosionEffect.Play();
            audioSource.PlayOneShot(explosionSound);
            remains.SetParent(null, true);
            Destroy(remains.gameObject, 1f);
            Destroy(gameObject);
        }
    }

    public void PullPin()
    {
        pinPulledTime = Time.time;
        explosionTime = Time.time + fuseTime;
        StartCoroutine(PullBack());
        audioSource.PlayOneShot(pinPullSound);
    }

    public void ThrowGrenade()
    {
        transform.SetParent(null, true);
        thrown = true;
        float charge = Mathf.Min((Time.time - pinPulledTime) / throwChargeTime, 1);
        float throwStrength = throwForce * charge;

        Debug.Log("Throwing grenade with strength: " + throwStrength + " charge: " + charge);
        rb.isKinematic = false;
        rb.AddForce(transform.forward * -throwStrength, ForceMode.VelocityChange);
        rb.angularVelocity = new Vector3(0, 0, charge * 5f);
    }

    public IEnumerator PullBack()
    {
        Vector3 initialPosition = transform.localPosition;
        Vector3 recoilTarget = initialPosition + new Vector3(0, 0, pullBackDistance);
        float t = 0f;

        while (t < throwChargeTime)
        {
            if (thrown) yield break;

            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(initialPosition, recoilTarget, t / throwChargeTime);
            yield return null;
        }
        yield break;
    }
}
