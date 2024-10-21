using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionAI : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public float followDistance = 3.0f; // Distance to maintain from the player
    public float followSpeed = 3.0f; // Speed of following

    public GameObject floatingText; 
    private bool isBefriended = false;

    void Start()
    {
    }

    void Update()
    {
        if (isBefriended)
        {
            floatingText.SetActive(false); 
            // Calculate the desired position
            Vector3 targetPosition = player.position - player.forward * followDistance;
            targetPosition.y = transform.position.y; // Maintain the same height

            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    public void Befriend()
    {
        floatingText.SetActive(false); 
        isBefriended = true;
    }
}
