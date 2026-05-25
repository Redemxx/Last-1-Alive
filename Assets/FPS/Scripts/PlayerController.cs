using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Burst.Intrinsics.X86.Avx;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 9f;
    [SerializeField] private float maxStamina = 50 * 5;
    [SerializeField] private float staminaRegenRate = 20;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float safeFallHeight = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.05f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip fallImpactSound;
    public int healthPacks;
    public bool hasFlashlight;

    private float xRotation = 0f;
    private Vector2 lookInput;

    private Rigidbody rb;
    private Animator animator;
    public Health health { get; private set; }
    private AudioSource audioSource;
    private Light flashlight;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isRunning;
    public bool isZoomed;
    private PlayerInput playerInput;
    private float stamina;
    private float mouseSensitivity;
    private float baseMouseSensitivity;
    private float maxAirHeight = 0;

    private Transform lefthandRest;
    private Transform righthandRest;
    [SerializeField]  private Transform camRoot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerInput = new PlayerInput();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        stamina = maxStamina;
        mouseSensitivity = GameState.Instance.mouseSensitivity;
        baseMouseSensitivity = mouseSensitivity;

        health = GetComponent<Health>();
        health.onDeath += OnDeath;
        health.ApplyModifier(-1f * GameState.Instance.GameModifier());

        audioSource = GetComponentInChildren<AudioSource>();
        flashlight = GetComponentInChildren<Light>();

        lefthandRest = transform.Find("LeftHandRest");
        righthandRest = transform.Find("RightHandRest");
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, lefthandRest.position);
        animator.SetIKPosition(AvatarIKGoal.RightHand, righthandRest.position);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        baseMouseSensitivity = sensitivity;
        mouseSensitivity = sensitivity;
    }

    public void PlayPickupSound()
    {
        audioSource.PlayOneShot(pickupSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void OnDeath(GameObject source)
    {
        Time.timeScale = 0f;
        FPSUI.Instance.ChangeMenu(2); 
    }
    
    void Update()
    {
        MovePlayer();
    }

    void FixedUpdate()
    {
        CheckGround();

        if (rb.linearVelocity.sqrMagnitude <= 0.5f) {
            animator.SetBool("Moving", false);
        } else {
            animator.SetBool("Moving", true);
        }

        if (isRunning) stamina -= 1;

        if (!isRunning && stamina < maxStamina)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            if (stamina > maxStamina) stamina = maxStamina;
        }

        playerCamera.transform.position = camRoot.position;
    }

    void OnJump()
    {
        if (Time.timeScale == 0) return;
        if (isGrounded)
        {
            animator.SetTrigger("Jump");
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    void OnRun()
    {
        if (Time.timeScale == 0) return;
        if (isGrounded && stamina > maxStamina/2)
        {
            animator.SetBool("Running", true);
            isRunning = true;
        }
    }

    void OnRunEnd()
    {
        isRunning = false;
        animator.SetBool("Running", false);
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (!isGrounded)
        {
            float currentHeight = transform.position.y;
            if (currentHeight > maxAirHeight) maxAirHeight = currentHeight;
        }
        else
        {
            if (maxAirHeight - transform.position.y > safeFallHeight)
            {
                audioSource.PlayOneShot(fallImpactSound);
                float fallDamage = 2 * (maxAirHeight - transform.position.y - safeFallHeight) + 10;
                fallDamage *= (1f + GameState.Instance.GameModifier());
                health.TakeDamage(gameObject, Mathf.CeilToInt(fallDamage));
            }
            maxAirHeight = transform.position.y;
        }
    }
    
    void OnMovement(InputValue inputValue)
    {
        if (Time.timeScale == 0) return;
        moveInput = inputValue.Get<Vector2>();
    }   

    void OnLook(InputValue inputValue)
    {
        if (Time.timeScale == 0f) return;
        //Vector2 tmp = inputValue.Get<Vector2>();
        lookInput = inputValue.Get<Vector2>();

        //float mouseX = tmp.x * mouseSensitivity;
        //float mouseY = tmp.y * mouseSensitivity;

        //xRotation -= mouseY;
        //xRotation = Mathf.Clamp(xRotation, -90, 90);
        //playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //transform.Rotate(Vector3.up * mouseX);
    }

    void OnToggleLight()
    {
        if (Time.timeScale == 0) return;
        if (!hasFlashlight) return;
        flashlight.enabled = !flashlight.enabled;
    }

    void OnMenu()
    {
        if (health.GetHealth() <= 0)   
        {
            FPSUI.Instance.ExitToMainMenu();
        }
        else
        {
            if (Time.timeScale == 0f)
                FPSUI.Instance.ResumeGame();
            else
                FPSUI.Instance.PauseGame();
        }
    }

    void OnHeal()
    {
        if (Time.timeScale == 0) return;
        if (healthPacks <= 0) return;

        healthPacks--;
        health.Heal(Mathf.CeilToInt(35 * (1f - GameState.Instance.GameModifier())));
    }
    
    public void AimDownSights(float? factor = null)
    {
        if (Time.timeScale == 0f) return;
        if (factor.HasValue)
        {
            mouseSensitivity = baseMouseSensitivity * factor.Value;
            isZoomed = true;
        }
        else
        {
            mouseSensitivity = baseMouseSensitivity;
            isZoomed = false;
        }
    }

    void MovePlayer()
    {
        if (Time.timeScale == 0f) return;

        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        direction.Normalize();

        if (stamina < 0)
        {
            stamina = 0;
            isRunning = false;
            animator.SetBool("Running", false);
        }

        float movementSpeed = isRunning ? runSpeed : moveSpeed;
        if (isZoomed) movementSpeed *= 0.4f;
        rb.linearVelocity = new Vector3(direction.x * movementSpeed, rb.linearVelocity.y, direction.z * movementSpeed);


        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
