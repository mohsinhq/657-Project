using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player; // The player's transform
    public Vector3 offset; // Offset from the player
    public float rotationSpeed; // Speed of camera rotation
    public float mouseSensitivity = 0.5f;

    private float currentRotationAngle = 0f;

    void Start()
    {
        rotationSpeed = 50f;
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

        // Rotate the camera only while the left mouse button is being held down
        if (Input.GetMouseButton(0)) // 0 is the left mouse button
        {
            // Get mouse X movement and apply sensitivity
            float mouseX = Input.GetAxis("Mouse X");
            currentRotationAngle += mouseX * rotationSpeed * mouseSensitivity;
        }

        // Also, rotate the camera with left and right arrows (keeping your original input handling)
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentRotationAngle -= rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            currentRotationAngle += rotationSpeed * Time.deltaTime;
        }

        // Calculate the rotation of the camera
        Quaternion rotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the new position and apply rotation
        Vector3 newPosition = player.position + rotation * offset;
        transform.position = newPosition;

        // Make the camera look at the player
        transform.LookAt(player);
    }
}
