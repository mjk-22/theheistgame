using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Ensures that a Rigidbody component is attached to the GameObject
public class PlayerMovement : MonoBehaviour
{
    // ============================== Movement Settings ==============================
    [Header("Movement Settings")]
    [SerializeField] private float baseWalkSpeed = 5f;    // Base speed when walking
    [SerializeField] private float baseRunSpeed = 8f;     // Base speed when running
    [SerializeField] private float rotationSpeed = 10f;   // Speed at which the character rotates

    // ============================== Crouching Settings ==============================
    [Header("Crouch Settings")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;  // Keyboard key
    //[SerializeField] private KeyCode crouchKey = KeyCode.C;

    [SerializeField] private float crouchSpeedMultiplier = 0.5f;       // Move slower
    private float originalHeight = 1f;                                
    private bool isCrouching = false;       
    public bool IsCrouching => isCrouching;
                           


    // ============================== Jump Settings =================================
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;        // Jump force applied to the character
    [SerializeField] private float groundCheckDistance = 1.1f; // Distance to check for ground contact (Raycast)

    // ============================== Modifiable from other scripts ==================
    public float speedMultiplier = 1.0f; // Additional multiplier for character speed ( WINK WINK )

    // ============================== Private Variables ==============================
    private Rigidbody rb; // Reference to the Rigidbody component
    [SerializeField] private Transform cameraTransform; // Reference to the camera's transform (can be assigned manually)

    // Input variables
    private float moveX; // Stores horizontal movement input (A/D or Left/Right Arrow)
    private float moveZ; // Stores vertical movement input (W/S or Up/Down Arrow)
    private bool jumpRequest; // Flag to check if the player requested a jump
    private Vector3 moveDirection; // Stores the calculated movement direction

    // ============================== Animation Variables ==============================
    [Header("Anim values")]
    public float groundSpeed; // Speed value used for animations

    // ============================== Character State Properties ==============================
    /// <summary>
    /// Checks if the character is currently grounded using a Raycast.
    /// If false, the character is in the air.
    /// </summary>
    public bool IsGrounded => 
        Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, groundCheckDistance);

    /// <summary>
    /// Checks if the player is currently holding the "Run" button.
    /// </summary>
    //private bool IsRunning => Input.GetButton("Run");
    private bool IsRunning => Input.GetKey(KeyCode.LeftShift);

    // ============================== Unity Built-in Methods ==============================

    /// <summary>
    /// Called when the script is first initialized.
    /// </summary>
    private void Awake()
    {
        InitializeComponents(); // Initialize Rigidbody and Camera reference
    }

    /// <summary>
    /// Called after Awake, used as a fallback to find camera if it wasn't available in Awake.
    /// </summary>
    private void Start()
    {
        // Ensure camera is found (in case it wasn't available in Awake)
        if (cameraTransform == null)
        {
            FindCamera();
        }
    }

    /// <summary>
    /// Called every frame, used to register player input.
    /// </summary>
    private void Update()
    {
        if (PauseManager.IsPaused)
        {
            jumpRequest = false;
            return;
        }

        RegisterInput(); // Collect player input
    }

    /// <summary>
    /// Called every physics update (FixedUpdate ensures physics stability).
    /// </summary>
    private void FixedUpdate()
    {
        if (PauseManager.IsPaused) return;
        HandleMovement(); // Process movement and physics-based updates
    }

    // ============================== Initialization ==============================

    /// <summary>
    /// Initializes Rigidbody and camera reference.
    /// Also locks and hides the cursor for better control.
    /// </summary>
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        rb.freezeRotation = true; // Prevent Rigidbody from rotating due to physics interactions
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth physics interpolation
        originalHeight = transform.localScale.y;

        // Find camera if not manually assigned
        FindCamera();

        // Lock and hide the cursor for better gameplay control
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    /// <summary>
    /// Finds and assigns the camera reference. Tries multiple methods to ensure camera is found.
    /// </summary>
    private void FindCamera()
    {
        // If camera is already assigned manually, use it
        if (cameraTransform != null)
            return;

        // Try to find the main camera
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            return;
        }

        // Fallback: Find any camera in the scene
        Camera foundCamera = FindObjectOfType<Camera>();
        if (foundCamera != null)
        {
            cameraTransform = foundCamera.transform;
            //Debug.LogWarning("PlayerMovement: MainCamera not found, using first available camera.");
        }
        else
        {
            //Debug.LogError("PlayerMovement: No camera found! Movement will be in world space.");
        }
    }

    // ============================== Input Handling ==============================

    /// <summary>
    /// Reads player input values and registers movement/jump requests.
    /// </summary>
    private void RegisterInput()
    {
        moveX = Input.GetAxis("Horizontal"); // Get horizontal movement input
        moveZ = Input.GetAxis("Vertical");   // Get vertical movement input

        // Register a jump request if the player presses the Jump button
        if (Input.GetButtonDown("Jump"))
        {
            jumpRequest = true;
        }

        // Crouch Input (keyboard)
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = true;
            transform.localScale = new Vector3(
                transform.localScale.x,
                //crouchHeight,
                originalHeight,
                transform.localScale.z
            );
        }

        if (Input.GetKeyUp(crouchKey))
        {
            isCrouching = false;
            transform.localScale = new Vector3(
                transform.localScale.x,
                originalHeight,
                transform.localScale.z
            );
        }

    }

    // ============================== Movement Handling ==============================

    /// <summary>
    /// Handles movement-related logic: calculating direction, jumping, rotating, and moving.
    /// </summary>
    private void HandleMovement()
    {
        CalculateMoveDirection(); // Compute the movement direction based on input
        HandleJump(); // Process jump input
        RotateCharacter(); // Rotate the character towards the movement direction
        MoveCharacter(); // Move the character using velocity-based movement
    }

    /// <summary>
    /// Calculates the movement direction based on player input and camera orientation.
    /// </summary>
    private void CalculateMoveDirection()
    {
        // Try to find camera if it's still null (runtime fallback)
        if (cameraTransform == null)
        {
            FindCamera();
        }

        // If the camera is not assigned, move based on world space
        if (cameraTransform == null)
        {
            moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        }
        else
        {
            // Get forward and right vectors from the camera perspective
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // Ignore Y-axis movement to prevent unwanted tilting
            forward.y = 0f;
            right.y = 0f;

            // Normalize vectors to maintain consistent movement speed
            forward.Normalize();
            right.Normalize();

            // Calculate movement direction relative to the camera orientation
            moveDirection = (forward * moveZ + right * moveX).normalized;
        }
    }

    /// <summary>
    /// Handles jumping by applying an impulse force if the character is grounded.
    /// </summary>
    private void HandleJump()
    {
        // Apply jump force only if jump was requested and the character is grounded
        if (jumpRequest && IsGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply force upwards
            jumpRequest = false; // Reset jump request after applying jump
        }
    }

    /// <summary>
    /// Rotates the character towards the movement direction.
    /// </summary>
    private void RotateCharacter()
    {
        // Rotate only if the character is moving
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Moves the character using Rigidbody's velocity instead of MovePosition.
    /// This ensures smooth movement while avoiding physics conflicts.
    /// </summary>
    private void MoveCharacter()
    {
        // Determine movement speed (walking or running)
        //float speed = IsRunning ? baseRunSpeed : baseWalkSpeed;
        float speed = IsRunning ? baseRunSpeed : baseWalkSpeed;

        // If crouching on keyboard → slow down
        if (isCrouching)
            speed *= crouchSpeedMultiplier;

        
        // Set ground speed value for animation purposes
        groundSpeed = (moveDirection != Vector3.zero) ? speed : 0.0f;

        // Preserve the current Y velocity to maintain gravity effects
        Vector3 newVelocity = new Vector3(
            moveDirection.x * speed * speedMultiplier, 
            rb.velocity.y, // Keep the existing Y velocity for jumping & gravity
            moveDirection.z * speed * speedMultiplier
        );

        // Apply the new velocity directly
        rb.velocity = newVelocity;
    }
}