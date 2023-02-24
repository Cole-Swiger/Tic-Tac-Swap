using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeMoveController : MonoBehaviour
{
    GameManager gameManager;
    //Only allow a move to be played if active is true.
    public bool isActive = false;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Call the MakeMove method from GameManager if active. Otherwise warn the player.
    private void OnMouseDown()
    {
        if (isActive)
        {
            gameManager.MakeMove();
        }
        else
        {
            Debug.Log("Please select a tile before making a move");
        }
    }
}
