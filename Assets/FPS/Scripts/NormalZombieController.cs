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
    [SerializeField] private float chasingSpeed = 5f;
    [SerializeField] private float wanderingSpeed = 1.5f;
    [SerializeField] private float wanderChance = 0.1f/100f;

    [SerializeField] private bool idleAnimationIsFloat = true;
    [SerializeField] private bool moveAnimationIsFloat = true;
    [SerializeField] private bool deathAnimationIsFloat = true;
    [SerializeField] private float idleAnimationsRandge = 1;
    [SerializeField] private float moveAnimationsRange = 1;
    [SerializeField] private float deathAnimationsRange = 1;


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
    private int wanderOffset;

    void Start()
    {
        attackDamage = Mathf.CeilToInt(attackDamage * (1f + GameState.Instance.GameModifier()));
        health = GetComponent<Health>();
        health.onDeath += Die;
        health.onDamaged += OnAlerted;
        health.ApplyModifier(GameState.Instance.GameModifier());

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

        float idleType = idleAnimationIsFloat ? Random.Range(0f, idleAnimationsRandge) : Random.Range(0, (int)idleAnimationsRandge + 1);
        animator.SetFloat("IdleType", idleType);
        Debug.Log("idle type: " + animator.GetFloat("IdleType"));
        float walkType = moveAnimationIsFloat ? Random.Range(0f, moveAnimationsRange) : Random.Range(0, (int)moveAnimationsRange + 1);
        animator.SetFloat("WalkType", walkType);
        Debug.Log("walk type: " + animator.GetFloat("WalkType"));
        float deathType = deathAnimationIsFloat ? Random.Range(0f, deathAnimationsRange) : Random.Range(0, (int)deathAnimationsRange + 1);
        Debug.Log(deathType);
        animator.SetFloat("DeathType", deathType);
        Debug.Log("death type: " + animator.GetFloat("DeathType"));

        nextSoundTime = Time.time + UnityEngine.Random.Range(0f, 60f);
        wanderOffset = Random.Range(0, 60);
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

        if (Time.frameCount%(60) == wanderOffset && Random.Range(0f, 1f) > wanderChance) { 
            // Set random dest
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
        nav.speed = chasingSpeed;
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

        if (other.CompareTag("Zombie"))
        {
            var zombieController = other.gameObject.GetComponent<NormalZombieController>();
            if (zombieController != null && zombieController.target != null)
            {
                StartCoroutine(Engage(zombieController.target));
                return;
            }
        }

        if (target != null) return;

        if (other.CompareTag("Player"))
        {
            AlertZombies(other.gameObject);
            target = other.gameObject;
            nav.speed = chasingSpeed;
        }
    }

    private void AlertZombies(GameObject source)
    {
        target = source;
        nav.speed = chasingSpeed;
        audioSource.PlayOneShot(zombieScream);
        animator.SetBool("Engaged", true);

        StartCoroutine(AlertZombiesCont());
    }

    public IEnumerator AlertZombiesCont() {
        yield return new WaitForSeconds(2f);

        if (isDead) yield break;

        foreach (var collider in colliders)
        {
            if (collider != null && collider.CompareTag("Zombie"))
            {
                var zombieController = collider.gameObject.GetComponent<NormalZombieController>();
                if (zombieController != null)
                {
                    StartCoroutine(zombieController.Engage(target));
                }
            }
        }
        yield break;
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
        if (target != null) return;
        health.onDamaged -= OnAlerted;
        AlertZombies(source);
    }
}
