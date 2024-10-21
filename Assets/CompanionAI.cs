using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionAI : MonoBehaviour
{
    public Transform player; 
    public float followDistance = 3.0f; 
    public float followSpeed = 3.0f; 

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
