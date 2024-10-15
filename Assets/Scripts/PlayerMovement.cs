using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float sprintSpeed = 1f;
    public float jumpForce = 5f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent unwanted rotations
    }

    void Update()
    {
        // Initialize movement variables
        float moveX = 0f;
        float moveZ = 0f;

        // WASD Movement
        if (Input.GetKey(KeyCode.W)) // Move forward
        {
            moveZ = 1f;
        }
        if (Input.GetKey(KeyCode.S)) // Move backward
        {
            moveZ = -1f;
        }
        if (Input.GetKey(KeyCode.A)) // Move left
        {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D)) // Move right
        {
            moveX = 1f;
        }

        // Calculate movement direction
        Vector3 move = (cameraTransform.forward * moveZ + cameraTransform.right * moveX).normalized;
        move.y = 0; // Prevent movement in Y axis
        
        // Movement
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.MovePosition(transform.position + move * sprintSpeed * Time.deltaTime);
        }
        else
        {
            rb.MovePosition(transform.position + move * moveSpeed * Time.deltaTime);
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor")) // Updated to "Floor"
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor")) // Updated to "Floor"
        {
            isGrounded = false;
        }
    }
}
