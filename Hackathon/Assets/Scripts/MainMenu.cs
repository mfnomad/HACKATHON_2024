using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void QuitGame()
    {
        Debug.Log("Ha Salido del juego");
        Application.Quit();

    }

    public void Play()
    {
        SceneManager.LoadScene("Main");

    }
    
    // Start is called before the first frame update
    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
