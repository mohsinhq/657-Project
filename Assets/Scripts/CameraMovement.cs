using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform player; // The player's transform
    [SerializeField] private float rotationSpeed = 0.1f; // Speed of camera rotation
    [SerializeField] private float xClamp = 75f; // Limits for vertical rotation
    [SerializeField] private float distanceFromPlayer = 5f; // Distance from the player
    [SerializeField] private float heightOffset = 2f; // Height offset above the player

    private float xRotation = 0f; // Vertical rotation
    private float yRotation = 0f; // Horizontal rotation
    private PlayerControls playerControls; // Reference to input actions

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Look.performed += ctx => ReceiveInput(ctx.ReadValue<Vector2>());
        playerControls.Player.Look.canceled += ctx => ReceiveInput(Vector2.zero);
    }

    private void OnEnable()
    {
        playerControls.Player.Enable(); // Enable input actions
    }

    private void OnDisable()
    {
        playerControls.Player.Disable(); // Disable input actions
    }

    private void LateUpdate()
    {
        if (player == null) return; // Exit if player reference is null

        // Check if the left mouse button is being held down
        if (Mouse.current.leftButton.isPressed)
        {
            // Calculate rotation based on mouse input
            Vector2 mouseInput = playerControls.Player.Look.ReadValue<Vector2>();
            yRotation += mouseInput.x * rotationSpeed; // Horizontal rotation
            xRotation -= mouseInput.y * rotationSpeed; // Vertical rotation
            xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp); // Clamp vertical rotation

            // Calculate the desired position and rotation of the camera
            Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);
            Vector3 position = player.position - rotation * Vector3.forward * distanceFromPlayer; // Position behind the player
            position.y += heightOffset; // Add height offset

            // Apply the position and rotation to the camera
            transform.position = position;
            transform.LookAt(player); // Make the camera look at the player
        }
    }

    private void ReceiveInput(Vector2 mouseInput)
    {
        // You can process the input if needed, but it's handled in LateUpdate
    }

    private void OnDestroy()
    {
        playerControls.Player.Look.performed -= ctx => ReceiveInput(ctx.ReadValue<Vector2>());
        playerControls.Player.Look.canceled -= ctx => ReceiveInput(Vector2.zero);
    }
}
