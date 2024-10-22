using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code that helps the companion follow the Player

public class CompanionAI : MonoBehaviour
{
    // Attributes
    public Transform player; 
    public float followDistance = 3.0f; 
    public float followSpeed = 3.0f; 

    public GameObject floatingText; 
    private bool isBefriended = false;

    void Start()
    {
    }

    // Turns off floating text if befriended and starts following player
    void Update()
    {
        if (isBefriended)
        {
            floatingText.SetActive(false); 
            Vector3 targetPosition = player.position - player.forward * followDistance;
            targetPosition.y = transform.position.y; 

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    public void Befriend()
    {
        
        floatingText.SetActive(false); 
        isBefriended = true;
    }
}
