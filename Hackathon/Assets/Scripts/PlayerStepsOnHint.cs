using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStepsOnHint : MonoBehaviour
{
    public TMP_Text hint;
    public GameObject player;
    
    private void CheckPlayerPosition()
    {
        // Get the player's position
        Vector3 playerPosition = player.transform.position;

        // Get the prop's position
        Vector3 propPosition = transform.position;

        // Check if the player's coordinates cross the prop's coordinates
        if (playerPosition.x >= propPosition.x - 0.5f && playerPosition.x <= propPosition.x + 0.5f &&
            playerPosition.y >= propPosition.y - 0.5f && playerPosition.y <= propPosition.y + 0.5f)
        {
            // Hint is shown
            hint.gameObject.SetActive(true);
        }
        else
        {
            // Hint is hidden
            hint.gameObject.SetActive(false);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerPosition();        
    }
}
