using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Lumin;
using static Unity.VisualScripting.Member;
using static UnityEditor.FilePathAttribute;

[RequireComponent(typeof(Health))]

public class NormalZombieController : MonoBehaviour
{
    enum State
    {
        STANDING,
        SCREAMING,
        CHASING,
        ATTACKING,
        DEAD
    };

    [SerializeField] private AudioClip zombieScream;

    private Health health;
    private NavMeshAgent nav;
    private Animator animator;
    private Rigidbody rb;
    private AudioSource audioSource;

    private GameObject target;
    private Vector3 targetPosition = Vector3.zero;
    private State state = State.STANDING;

    private int screamHash, chaseHash, attackHash, deathHash, pausedHash;

    private List<Collider> colliders = new List<Collider>();

    void Start()
    {
        health = GetComponent<Health>();
        health.onDeath += Die;
        health.onDamaged += OnAlerted;

        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        audioSource = GetComponent<AudioSource>();

        animator.Play("Idle");

        screamHash = Animator.StringToHash("Scream");
        chaseHash = Animator.StringToHash("Chase");
        attackHash = Animator.StringToHash("Attack");
        deathHash = Animator.StringToHash("Death");
        pausedHash = Animator.StringToHash("Paused");
    }

    void Update()
    {
        if (target != null && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == chaseHash)
        {
            nav.SetDestination(target.transform.position);
        }
    }

    private void SwitchState(State newState)
    {
        if (state == newState) return;
        state = newState;

        //switch (state)
        //{
        //    case State.STANDING:
        //        animator.Play("Idle");
        //        break;
        //    case State.SCREAMING:
        //        animator.Play("Scream");
        //        break;
        //    case State.CHASING:
        //        animator.Play("Chase");
        //        break;
        //    case State.ATTACKING:
        //        animator.Play("Attack");
        //        break;
        //    case State.DEAD:
        //        animator.Play("Death");
        //        break;
        //    default:
        //        animator.Play("Idle"); 
        //        break;
        //}
    }

    public IEnumerator Engage(GameObject obj)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.5f));
        target = obj;
        SwitchState(State.CHASING);
        animator.SetBool("Moving", true);
    }

    private IEnumerator TurnToward(GameObject obj)
    {
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
        colliders.Add(other);

        if (target != null) return;

        if (other.CompareTag("Player"))
        {
            target = other.gameObject;
            SwitchState(State.SCREAMING);
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
        colliders.Remove(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    public void Die(GameObject source)
    {
        SwitchState(State.DEAD);
        animator.SetTrigger("Die");
        nav.isStopped = true;
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        colliders.Clear();
    }

    public void OnAlerted(GameObject source)
    {
        nav.SetDestination(source.transform.position);
        animator.SetBool("Moving", true);
        health.onDamaged -= OnAlerted;
    }
}
