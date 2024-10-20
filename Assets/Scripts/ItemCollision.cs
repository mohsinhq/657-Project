using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollision : MonoBehaviour { private void OnCollisionEnter(Collision Touches) { if (Touches.gameObject.CompareTag("Player")) { gameObject.SetActive(false); } } }
