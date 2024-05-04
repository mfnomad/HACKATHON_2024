using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStepsOnHint : MonoBehaviour
{
    // This method is called when another collider enters the trigger collider attached to this GameObject
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other collider belongs to the player
        if (other.CompareTag("PF Player"))
        {
            // Get the player's position
            Vector3 playerPosition = other.transform.position;

            // Get the prop's position
            Vector3 propPosition = transform.position;

            // Check if the player's coordinates cross the prop's coordinates
            if (playerPosition.x >= propPosition.x - 0.5f && playerPosition.x <= propPosition.x + 0.5f &&
                playerPosition.y >= propPosition.y - 0.5f && playerPosition.y <= propPosition.y + 0.5f)
            {
                // Log the message to the console
                Debug.Log("Player touched prop");
            }
        }
    }
}
