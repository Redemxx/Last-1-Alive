using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]

public class NormalZombieController : MonoBehaviour
{
    [SerializeField] private AudioClip[] ambientSounds;
    [SerializeField] private AudioClip zombieScream;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDistance = 1.0f;

    private Health health;
    private NavMeshAgent nav;
    private Animator animator;
    private AudioSource audioSource;
    private GameObject target;
    private Vector3 targetPosition = Vector3.zero;
    private int screamHash, chaseHash, attackHash, deathHash, pausedHash;
    private List<Collider> colliders = new List<Collider>();
    private float nextAttackTime = 0f;
    private float nextSoundTime = 0f;
    private bool isDead = false;

    void Start()
    {
        health = GetComponent<Health>();
        health.onDeath += Die;
        health.onDamaged += OnAlerted;

        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Transform tr = GetComponent<Transform>();
        tr.rotation= Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        audioSource = GetComponent<AudioSource>();

        animator.Play("Idle");

        screamHash = Animator.StringToHash("Scream");
        chaseHash = Animator.StringToHash("Chase");
        attackHash = Animator.StringToHash("Attack");
        deathHash = Animator.StringToHash("Death");
        pausedHash = Animator.StringToHash("Paused");

        nextSoundTime = Time.time + UnityEngine.Random.Range(0f, 60f);
    }

    void Update()
    {
        if (isDead) return;

        if (Time.time >= nextSoundTime)
        {
            audioSource.PlayOneShot(ambientSounds[Random.Range(0, ambientSounds.Length)]);
            nextSoundTime = Time.time + UnityEngine.Random.Range(10f, 60f);
        }

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget <= attackDistance && nav.velocity.magnitude <= 0.1 && Time.time >= nextAttackTime)
            {
                Attack();
            }
            
            int currentStateHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            if (currentStateHash == chaseHash || (currentStateHash == pausedHash && Time.time > nextAttackTime)) { 
                animator.SetBool("Moving", true);
                nav.SetDestination(target.transform.position);
            }
        }
    }

    private void Attack()
    {
        if (isDead) return;

        nextAttackTime = Time.time + attackCooldown;
        animator.SetTrigger("Attack");
        animator.SetBool("Moving", false);

        StartCoroutine(ThrowAttack());
    }

    public IEnumerator ThrowAttack()
    {
        yield return new WaitForSeconds(0.8f);
        if (target == null) yield break;
        if (isDead) yield break;

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget > attackDistance) yield break;

        Health player = target.GetComponentInParent<Health>();
        if (player != null)
        {
            player.TakeDamage(gameObject, attackDamage);
        }
        else
        {
            Debug.LogWarning("Target does not have a Health component!");
        }

        audioSource.PlayOneShot(hitSound);
    }
    public IEnumerator Engage(GameObject obj)
    {
        if (isDead) yield break;
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.5f));
        target = obj;
        animator.SetBool("Moving", true);
    }

    private IEnumerator TurnToward(GameObject obj)
    {
        if (isDead) yield break;

        Vector3 direction = (obj.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float time = 0;
        while (time < 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        colliders.Add(other);

        if (target != null) return;

        if (other.CompareTag("Player"))
        {
            target = other.gameObject;
            audioSource.PlayOneShot(zombieScream);
            animator.SetBool("Engaged", true);

            // Alert nearby zombies
            foreach (var collider in colliders)
            {
                if (collider != null &&collider.CompareTag("Zombie"))
                {
                    var zombieController = collider.gameObject.GetComponent<NormalZombieController>();
                    if (zombieController != null)
                    {
                        StartCoroutine(zombieController.Engage(target));
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isDead) return;

        colliders.Remove(other);
    }

    public void Die(GameObject source)
    {
        animator.SetTrigger("Die");
        nav.isStopped = true;
        GetComponent<Collider>().enabled = false;
        colliders.Clear();
        isDead = true;
        Destroy(gameObject, 60f);
    }

    public void OnAlerted(GameObject source)
    {
        target = source;
        nav.SetDestination(source.transform.position);
        animator.SetBool("Moving", true);
        health.onDamaged -= OnAlerted;
    }
}
