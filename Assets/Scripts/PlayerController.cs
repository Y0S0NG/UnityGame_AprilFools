using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Public variables in the Inspector
    public float moveSpeed = 5f;           // How fast the player moves
    public float jumpSpeed = 8f;          // Initial jump velocity
    public float gravity = -9.81f;        // Gravity acceleration
    public Transform cameraTransform;     // Assign this to the child camera transform

    private CharacterController controller;
    private float verticalVelocity;       // Tracks upward/downward velocity for jumping

    // Mouse look variables
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;         // For camera vertical rotation limit

    void Start()
    {
        // Get the CharacterController component on this same GameObject
        controller = GetComponent<CharacterController>();

        // If not assigned in Inspector, try to find a child camera automatically
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cameraTransform = cam.transform;
            }
        }

        // Lock the cursor (optional)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        // Get horizontal and vertical input (WASD / arrow keys)
        float moveX = Input.GetAxis("Horizontal");  // Left/right
        float moveZ = Input.GetAxis("Vertical");    // Forward/back

        // Transform direction from local to world space
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Apply movement speed
        move *= moveSpeed;

        // If grounded, allow jumping
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f; // small downward force to keep grounded
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpSpeed;
            }
        }
        else
        {
            // Apply gravity
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Combine horizontal movement with vertical velocity (jumping/falling)
        move.y = verticalVelocity;

        // Move the CharacterController
        controller.Move(move * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player around the Y-axis (turn left/right)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera up/down, clamping the angle so we don¡¯t flip upside down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotation to camera transform
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
