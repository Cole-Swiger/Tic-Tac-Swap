using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapManager : MonoBehaviour
{
    GameManager gameManager;
    private AudioSource swapAudio;
    public AudioClip swapButton;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        swapAudio = GetComponent<AudioSource>();
    }

    //Turns the Swap mode on and off and resets selected tiles.
    public void ToggleSwap()
    {
        gameManager.isSwapActive = !gameManager.isSwapActive;
        TileController.ResetAllSelected();
        SwapAllSelectable();
        gameManager.SetMakeMoveActive();
        swapAudio.PlayOneShot(swapButton, 1);
    }

    public static void SwapAllSelectable()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

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
