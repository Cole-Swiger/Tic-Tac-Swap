using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        //Load the main game
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        //Close the game
        Application.Quit();
    }
}
