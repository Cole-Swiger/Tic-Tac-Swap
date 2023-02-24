using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapManager : MonoBehaviour
{
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Turns the Swap mode on and off and resets selected tiles.
    private void OnMouseDown()
    {
        gameManager.isSwapActive = !gameManager.isSwapActive;
        Debug.Log("Swap Active: " + gameManager.isSwapActive);
        TileController.ResetAllSelected();
        SwapAllSelectable();
        gameManager.SetMakeMoveActive();
    }

    public static void SwapAllSelectable()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        Debug.Log("Number of tiles: " + tiles.Length);

        foreach (GameObject tile in tiles)
        {
            if (!tile.GetComponent<TileController>().hasBeenSwapped)
            {
                //^= is like an XOR. So if left side is false, it returns true. If left is true, it return false.
                tile.GetComponent<TileController>().isSelectable ^= true;
            }
        }
    }
}
