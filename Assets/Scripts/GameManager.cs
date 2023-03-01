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

    //Game State and win conditions
    private GameObject[] score;
    private bool circleWinCondition = false;
    private bool triangleWinCondition = false;
    private int tilesSwapped = 0;
    private int openTiles = 9;
    public bool collisionsFinished = false;

    private GameObject[,] boardState = new GameObject[3, 3];

    // Start is called before the first frame update
    void Start()
    {
        score = GameObject.FindGameObjectsWithTag("Line");
    }

    // Update is called once per frame
    void Update()
    {
        //Game state needs to be checked once all collisions have been calculated
        if (collisionsFinished)
        {
            UpdateGameState();
            collisionsFinished = false;
        }
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
        TileController.ResetAllSelected();
        SetMakeMoveActive();
    }

    //Standard move
    private void PlaceLetter(GameObject prefab, int tilePosition)
    {
        GameObject tile = TileController.selectedTileList[tilePosition];
        Debug.Log("Tile position to be placed in: " + tile.GetComponent<TileController>().row + ", " + tile.GetComponent<TileController>().column);
        Debug.Log("Prefab name: " + prefab.name);
        GameObject letter = Instantiate(prefab, tile.transform.position, tile.transform.rotation);
        letter.name = tile.name + "-" + prefab.name;
        tile.GetComponent<TileController>().isSelectable = false;
        Debug.Log("Enter set board state");
        SetBoardState(letter);
        openTiles--;
        Debug.Log("Finish Place letter");
    }

    //Swap contents of 2 occupied spaces
    private void Swap()
    {
        List<GameObject> placeList = new List<GameObject>();

        //Create new instance of letter/shape from other position into current position, then delete old shape
        for(int i = 0; i < 2; i++)
        {
            int tileRow = TileController.selectedTileList[(i + 1) % 2].GetComponent<TileController>().row;
            int tileColumn = TileController.selectedTileList[(i + 1) % 2].GetComponent<TileController>().column;
            Debug.Log("Second letter position: " + tileRow + ", " + tileColumn);
            GameObject prefab = boardState[tileRow, tileColumn];
            prefab.name = prefab.name.Substring(prefab.name.LastIndexOf('-') + 1);
            Debug.Log("Prefab Name to switch into current position: " + prefab.name);
            placeList.Add(prefab);
        }

        //Replace current letters and destroy them once new ones are placed.
        foreach (GameObject g in placeList)
        {
            PlaceLetter(g, placeList.IndexOf(g));
            Destroy(g);
        }

        //Set hasBeen Swapped to true so tiles are grayed out.
        foreach (GameObject tile in TileController.selectedTileList)
        {
            tile.GetComponent<TileController>().hasBeenSwapped = true;
            tile.GetComponent<TileController>().isSelectable = false;
        }

        tilesSwapped += 2;
        //Counteract the 2 that are removed when calling PlaceLetter in the loop.
        openTiles += 2;
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

    private void SetBoardState(GameObject placedLetter)
    {
        int row;
        int column;

        if (placedLetter.name.Contains("Top")) 
            {row = 0;}
        else if (placedLetter.name.Contains("Middle")) 
            {row = 1;}
        else 
            {row = 2;}

        if (placedLetter.name.Contains("Left")) 
            {column = 0;}
        else if (placedLetter.name.Contains("Center")) 
            {column = 1;}
        else 
            {column = 2;}

        boardState.SetValue(placedLetter, new int[] { row, column });
    }

    private void UpdateGameState()
    {
        //Check if move has created 3 in a row
        circleWinCondition = false;
        triangleWinCondition = false;
        foreach (GameObject line in score)
        {
            if (line.GetComponent<ScoreLines>().collidedCircleList.Count == 3)
            {
                circleWinCondition = true;
            }

            if (line.GetComponent<ScoreLines>().collidedTriangleList.Count == 3)
            {
                triangleWinCondition = true;
            }
        }

        Debug.Log("Circle have 3 in a row? " + circleWinCondition);
        Debug.Log("Triangle have 3 in a row? " + triangleWinCondition);

        currentTurn++;
        CheckGameState();
    }

   private void CheckGameState()
    {
        Debug.Log("Checking Game State");
        Debug.Log("Current turn: " + GetCurrentTurn().name);
        Debug.Log("Tiles Swapped: " + tilesSwapped);
        Debug.Log("Open Tiles: " + openTiles);

        //Check if game is won
        CheckEndOfGame(false);

        //If condition is met, then there are no available moves
        if (tilesSwapped == 8 && openTiles == 0)
        {
            //See if previous move created a winning state for that player
            currentTurn++;
            CheckEndOfGame(true);
        }
    }

    private void CheckEndOfGame(bool noMoreMoves)
    {
        if (circleWinCondition && GetCurrentTurn().name.Contains("Circle"))
        {
            EndGame("Circle");
            return;
        }
        else if (triangleWinCondition && GetCurrentTurn().name.Contains("Triangle"))
        {
            EndGame("Triangle");
            return;
        }
        else if (noMoreMoves == true)
        {
            EndGame("Tie");
            return;
        }
    }

    private void EndGame(string winner)
    {
        if (winner.Equals("Tie"))
        {
            Debug.Log("It's a tie!");
        }
        else
        {
            Debug.Log(winner + " wins!");
        }
    }
}
