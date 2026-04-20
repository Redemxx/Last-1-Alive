using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.FilePathAttribute;

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
    [SerializeField] private int health = 28;
    [SerializeField] private GameObject bulletPrefab;

    private NavMeshAgent nav;
    private Animator animator;
    private Rigidbody rb;

    private GameObject target;
    private Vector3 targetPosition = Vector3.zero;
    private State state = State.STANDING;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        animator.Play("Idle");
    }

    void Update()
    {
    }

    private void SwitchState(State newState)
    {
        if (state == newState) return;
        state = newState;

        switch (state)
        {
            case State.STANDING:
                animator.Play("Idle");
                break;
            case State.SCREAMING:
                animator.Play("Scream");
                break;
            case State.CHASING:
                animator.Play("Chase");
                break;
            case State.ATTACKING:
                animator.Play("Attack");
                break;
            case State.DEAD:
                animator.Play("Death");
                break;
            default:
                animator.Play("Idle"); 
                break;
        }
    }
}
