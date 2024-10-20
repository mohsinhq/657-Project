using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public Transform CurrentPlayer; // The player's transform
    public float rotationSpeed = 0.1f; // Speed of camera rotation

    private PlayerControls playerControls;
    private float xAxisRotation = 0f; // Vertical rotation
    private float yAxisRotation = 0f; // Horizontal rotation

    void Start()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Enable(); // Enable input actions
    }

    void Update()
    {
        // Always follow the player
        Vector3 targetPosition = CurrentPlayer.position - Quaternion.Euler(xAxisRotation, yAxisRotation, 0) * Vector3.forward * 12;
        targetPosition.y += 5; // Add height offset
        transform.position = targetPosition;

        // Check if the left mouse button is held down
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mouseInput = playerControls.Player.Look.ReadValue<Vector2>();
            yAxisRotation += mouseInput.x * rotationSpeed;
            xAxisRotation -= mouseInput.y * rotationSpeed;
            xAxisRotation = Mathf.Clamp(xAxisRotation, -28f, 30f);
        }

        // Apply rotation and look at the player
        transform.rotation = Quaternion.Euler(xAxisRotation, yAxisRotation, 0);
        transform.LookAt(CurrentPlayer);
    }
}
