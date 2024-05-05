using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static IAController;

public class Gameplay : MonoBehaviour
{
    private int trialsLeft = 3;

    public GameObject player;
    public TMP_Text hint;
    public RawImage[] images;

    public GameObject goodEnding;
    public GameObject badEnding;

    private IAController iaController;

    public void HideAssets()
    {
        goodEnding.SetActive(false);
        badEnding.SetActive(false);
    }

    public void ResetPlayerPosition()
    {
        player.transform.position = new Vector3(0, -4, 0);
        player.SetActive(true);
    }

    public void UpdateTrialsValue()
    {
        trialsLeft -= 1;
    }

    public int GetTrials()
    {
        return trialsLeft;
    }

    public void NextTrial(bool correct)
    {
        if (trialsLeft > 0)
        {
            if (correct)
            {
                iaController = GetComponent<IAController>();
                iaController.getText(hint, images[0], images[1], images[2]);
                UpdateTrialsValue();
                HideAssets();
                ResetPlayerPosition();
                /* Transition fade to black - 3sec
                *  Get next hint
                *  Get images
                *  Update level
                *  Transition fade to clear
                */
                Debug.Log("Updating level...");
                Debug.Log("Next trial!");
            }
            else
            {
                EndGame();
            }
        }
        else
        {
            EndGame();
        }
    }

    public void ResetGame()
    {
        trialsLeft = 3;
        goodEnding.SetActive(false);
        badEnding.SetActive(false);
    }

    public void EndGame()
    {
        if (trialsLeft == 0)
        {
            Debug.Log("You win!");
        }
        else
        {
            Debug.Log("Game over!");
            badEnding.SetActive(true);
        }
    }
}
