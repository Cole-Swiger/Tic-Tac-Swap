using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeMoveController : MonoBehaviour
{
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    //Call the MakeMove method from GameManager if active.
    public void MakeMove()
    {
        gameManager.MakeMove();
    }
}
