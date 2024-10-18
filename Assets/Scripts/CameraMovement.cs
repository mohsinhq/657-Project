using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player; // The player's transform
    public Vector3 offset; // Offset from the player
    public float rotationSpeed; // Speed of camera rotation
    public float mouseSensitivity;

    private float currentRotationAngleX = 0f; // Horizontal rotation
    private float currentRotationAngleY = 0f; // Vertical rotation

    public float minYAngle = -60f; // Limits for vertical rotation
    public float maxYAngle = 60f;

    void Start()
    {
        rotationSpeed = 50f;
        mouseSensitivity = 0.1f;

        // Check if player is assigned
        if (player == null)
        {
            Debug.LogError("Player reference is not assigned! Please assign the player GameObject in the Inspector.");
            return; // Exit if player reference is null
        }

        // Set the initial offset based on camera and player position
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        // Check if player is assigned
        if (player == null)
        {
            Debug.LogError("Player reference is not assigned! Please assign the player GameObject in the Inspector.");
            return; // Exit if player reference is null
        }

        // Rotate the camera while the left mouse button is being held down
        if (Input.GetMouseButton(0)) // 0 is the left mouse button
        {
            // Get mouse X and Y movements and apply sensitivity
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y"); // Invert Y to mimic typical camera controls

            // Horizontal rotation (around the Y-axis)
            currentRotationAngleX += mouseX * rotationSpeed * mouseSensitivity;

            // Vertical rotation (around the X-axis), clamped to avoid flipping the camera
            currentRotationAngleY += mouseY * rotationSpeed * mouseSensitivity;
            currentRotationAngleY = Mathf.Clamp(currentRotationAngleY, minYAngle, maxYAngle);
        }

        // Rotate the camera with left and right arrows (horizontal rotation)
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentRotationAngleX -= rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            currentRotationAngleX += rotationSpeed * Time.deltaTime;
        }

        // Create a quaternion rotation from the angles
        Quaternion rotationX = Quaternion.Euler(0, currentRotationAngleX, 0); // Horizontal
        Quaternion rotationY = Quaternion.Euler(currentRotationAngleY, 0, 0); // Vertical

        // Combine both rotations to get the final rotation
        Quaternion finalRotation = rotationY * rotationX;

        // Set the new position and apply rotation
        Vector3 newPosition = player.position + finalRotation * offset;
        transform.position = newPosition;

        // Make the camera look at the player
        transform.LookAt(player);
    }
}
