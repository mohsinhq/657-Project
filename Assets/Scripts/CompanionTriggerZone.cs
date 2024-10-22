using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionTriggerZone : MonoBehaviour
{
    public GameObject floatingText; 
    private bool playerNearby = false;

    void Start()
    {
        floatingText.SetActive(false); 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            floatingText.SetActive(true); 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            floatingText.SetActive(false); 
        }
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            FindObjectOfType<CompanionAI>().Befriend();
        }
    }
}
