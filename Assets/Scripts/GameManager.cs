using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //references
    public GameManager gameManager;
    public GameObject[] shapesPrefabs;
    //Keep track of game mechanics. Turn 0 = circle, 1 = triangle
    public int currentTurn = 0;
    //Dictates the type of move being played
    public bool isSwapActive = false;
    //Determined by whether swap is active, 1 or 2
    private int maxSelected;
    //Must = maxSelected for move to be made. Cannot be more than maxSelected
    public int totalTilesSelected { get; set; }

    //Grid with tile values. -1 = empty, 0 = circle, 1 = triangle
    private int[,] tileGrid = new int[3, 3] { 
        { -1, -1, -1 }, 
        { -1, -1, -1 }, 
        { -1, -1, -1 } 
    };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //When Swap mode is active, 2 tiles can be selected.
    public int GetMaxSelected()
    {
        maxSelected = isSwapActive ? 2 : 1;
        return maxSelected;
    }

    //returns the prefab for whichever turn it is. Circle for 0, Triangle for 1
    private GameObject GetCurrentTurn()
    {
        currentTurn = currentTurn % 2;
        return shapesPrefabs[currentTurn];
    }

    //If Make Move is active, either place a letter/shape or swap 2 placed objects
    public void MakeMove()
    {
        if (isSwapActive)
        {
            Swap();
        }
        else
        {
            GameObject prefab = GetCurrentTurn();
            PlaceLetter(prefab, 0);
        }
        //Reset selected tiles after every turn and move turn count;
        currentTurn++;
        TileController.ResetAllSelected();
        SetMakeMoveActive();
    }

    //Standard move
    private void PlaceLetter(GameObject prefab, int tilePosition)
    {
        GameObject tile = TileController.selectedTileList[tilePosition];
        GameObject letter = Instantiate(prefab, tile.transform.position, tile.transform.rotation);
        letter.name = tile.name + "-letter";
        tile.GetComponent<TileController>().isSelectable = false;
    }

    //Swap contents of 2 occupied spaces
    private void Swap()
    {
        List<GameObject> tileList = TileController.selectedTileList;
        //Switch places in list
        GameObject tempObject = tileList[0];
        TileController.selectedTileList[0] = tileList[1];
        TileController.selectedTileList[1] = tempObject;

        //Create new instance of letter/shape from other position into current position, then delete old shape
        for(int i = 0; i < 2; i++)
        {
            GameObject prefab = GameObject.Find(TileController.selectedTileList[(i+1)%2].name + "-letter");
            PlaceLetter(prefab, i);
            Destroy(prefab);
        }

        //Set hasBeen Swapped to true so tiles are grayed out.
        foreach (GameObject tile in TileController.selectedTileList)
        {
            tile.GetComponent<TileController>().hasBeenSwapped = true;
            tile.GetComponent<TileController>().isSelectable = false;
        }

        //Set Swap mode to false and reset selected tiles.
        isSwapActive = false;
        SwapManager.SwapAllSelectable();
    }

    //The Make Move button should only be active if the number of selected tiles equals the maximum allowed.
    public void SetMakeMoveActive()
    {
        bool isActive = (totalTilesSelected == maxSelected) ? true : false;
        GameObject.Find("Make_Move_Button").GetComponent<MakeMoveController>().isActive = isActive;
    }
}
