using System;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 9f;
    [SerializeField] private float maxStamina = 50 * 5;
    [SerializeField] private float staminaRegenRate = 20;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    [SerializeField] private float baseMouseSensitivity = 400f;
    [SerializeField] private Camera playerCamera;
    private float xRotation = 0f;
    private Vector2 lookInput;

    private Rigidbody rb;
    private Health health;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isRunning;
    public bool isZoomed;
    private PlayerInput playerInput;
    private float stamina;
    private float mouseSensitivity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInput();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        stamina = maxStamina;
        mouseSensitivity = baseMouseSensitivity;

        health = GetComponent<Health>();
        health.onDeath += OnDeath;
    }

    public void OnDeath(GameObject source)
    {
        // Handle player death (e.g., respawn, show game over screen, etc.)
        Debug.Log("Player has died.");
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        HandleMouseLook();
    }

    void FixedUpdate()
    {
        MovePlayer();

        if (!isRunning && stamina < maxStamina)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            if (stamina > maxStamina) stamina = maxStamina;
        }
    }

    void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    void OnRun()
    {
        if (isGrounded && stamina > 0)
            isRunning = true;
    }

    void OnRunEnd()
    {
        isRunning = false;
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
    
    void OnMovement(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }   

    void OnLook(InputValue inputValue)
    {
        lookInput = inputValue.Get<Vector2>();
    }
    
    public void AimDownSights(float? factor = null)
    {
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

    void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void MovePlayer()
    {
        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        direction.Normalize();

        if (isRunning) stamina -= 1;

        if (stamina < 0)
        {
            stamina = 0;
            isRunning = false;
        }

        float movementSpeed = isRunning ? runSpeed : moveSpeed;
        if (isZoomed) movementSpeed *= 0.4f;
        rb.linearVelocity = new Vector3(direction.x * movementSpeed, rb.linearVelocity.y, direction.z * movementSpeed);
    }
}
