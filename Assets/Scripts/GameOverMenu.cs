using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
  
    //Reset Board
    public void Restart()
    {
        gameManager.ResetGame();
    }

    //Return to Main Menu
    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }
}
