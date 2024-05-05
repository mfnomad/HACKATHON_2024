using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gameplay : MonoBehaviour
{
    private int trialsLeft = 3;
    private int score = 0;

    public GameObject goodEnding;
    public GameObject badEnding;

    public void UpdateScore()
    {
        score += 1;
    }

    public void UpdateTrials()
    {
        trialsLeft -= 1;
    }

    public int GetScore()
    {
        return score;
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
                UpdateScore();
            }
            UpdateTrials();
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

    public void ResetGame()
    {
        trialsLeft = 3;
        score = 0;
        goodEnding.SetActive(false);
        badEnding.SetActive(false);
    }

    public void EndGame()
    {
        if (trialsLeft == 0)
        {
            if (score == 3)
            {
                Debug.Log("You win!");
            }
            else
            {
                Debug.Log("You lose!");
            }
        }
    }

    public void Start()
    {
        this.ResetGame();
    }
}
