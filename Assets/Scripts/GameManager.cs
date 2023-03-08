using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private int maxSelected = 1;
    //Must = maxSelected for move to be made. Cannot be more than maxSelected
    public int totalTilesSelected { get; set; }

    //Audio
    private AudioSource gameAudio;
    public AudioClip placeSound;
    public AudioClip swapAction;
    public AudioClip gameOver;

    //Game State and win conditions
    private GameObject[] score;
    private bool circleWinCondition = false;
    private bool triangleWinCondition = false;
    private int tilesSwapped = 0;
    private int openTiles = 9;
    public bool collisionsFinished = false;

    private GameObject[,] boardState = new GameObject[3, 3];
    public bool isGameOver = false;
    //Game Over screen/menu
    private GameObject background;

    // Start is called before the first frame update
    void Start()
    {
        score = GameObject.FindGameObjectsWithTag("Line");
        //Deactivate Game Over screen at start
        background = GameObject.Find("Game Over Background");
        background.SetActive(false);

        gameAudio = GetComponent<AudioSource>();
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

        //Update UI to reflect current turn and game state
        if (GetCurrentTurn().name.Equals("Circle"))
        {
            GameObject.Find("Turn Object").GetComponent<TextMeshProUGUI>().text = "Circle";
        }
        else
        {
            GameObject.Find("Turn Object").GetComponent<TextMeshProUGUI>().text = "Triangle";
        }

        if (isSwapActive)
        {
            GameObject.Find("Swap State").GetComponent<TextMeshProUGUI>().text = "Active";
        }
        else
        {
            GameObject.Find("Swap State").GetComponent<TextMeshProUGUI>().text = "Inactive";
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
            gameAudio.PlayOneShot(swapAction, 1);
        }
        else
        {
            GameObject prefab = GetCurrentTurn();
            PlaceLetter(prefab, 0);
            gameAudio.PlayOneShot(placeSound, 1);
        }
        //Reset selected tiles after every turn and move turn count;
        TileController.ResetAllSelected();
        SetMakeMoveActive();
    }

    //Standard move
    private void PlaceLetter(GameObject prefab, int tilePosition)
    {
        GameObject tile = TileController.selectedTileList[tilePosition];
        GameObject letter = Instantiate(prefab, tile.transform.position, tile.transform.rotation);
        letter.name = tile.name + "-" + prefab.name;

        tile.GetComponent<TileController>().isSelectable = false;
        SetBoardState(letter);
        openTiles--;
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
            GameObject prefab = boardState[tileRow, tileColumn];
            prefab.name = prefab.name.Substring(prefab.name.LastIndexOf('-') + 1);
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
        openTiles += 2; //Counteract the 2 that are removed when calling PlaceLetter in the loop.

        //Set Swap mode to false and reset selected tiles.
        isSwapActive = false;
        SwapManager.SwapAllSelectable();
    }

    //The Make Move button should only be active if the number of selected tiles equals the maximum allowed.
    public void SetMakeMoveActive()
    {
        bool isActive = (totalTilesSelected == maxSelected) ? true : false;

        if (isActive)
        {
            GameObject.Find("Make_Move").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("Make_Move").GetComponent<Button>().interactable = false;
        }
    }

    //2D array is used to keep track of objects in tiles
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

    //After letter is placed, update the game state to match current letter positions
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

        currentTurn++;
        CheckGameState();
    }

    //Check if a player has won or if the game is over.
   private void CheckGameState()
    {
        //If condition is met, then there are no available moves
        if (tilesSwapped == 8 && openTiles == 0)
        {
            //For final move, rules are different. If both have 3 in a row, it's a tie, regardless of who's turn it is.
            if (circleWinCondition && triangleWinCondition)
            {
                EndGame("Tie");
            }

            else
            {
                //See if previous move created a winning state for that player
                currentTurn++;
                CheckEndOfGame(true);
            }
            return;
        }

        //Check if game is won on normal turn.
        CheckEndOfGame(false);
    }

    //After a move, check if the game is over
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

    //Emd Game and display winner and Game Over menu
    private void EndGame(string winner)
    {
        isGameOver = true;
        background.SetActive(true);
        if (winner.Equals("Tie"))
        {
            GameObject.Find("Winner").GetComponent<TextMeshProUGUI>().text = "It's a Tie!";
        }
        else
        {
            GameObject.Find("Winner").GetComponent<TextMeshProUGUI>().text = winner + " Wins!";
        }
        gameAudio.PlayOneShot(gameOver, 1);
    }

    //Reset Game to original state
    public void ResetGame()
    {
        GameObject[] letters = GameObject.FindGameObjectsWithTag("Letter");
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        //Destroy all placed letter
        foreach (GameObject letter in letters)
        {
            Destroy(letter);
        }

        //Reset tiles
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<TileController>().isSelectable = true;
            tile.GetComponent<TileController>().hasBeenSwapped = false;
        }

        //Reset all values
        currentTurn = 0;
        isSwapActive = false;
        tilesSwapped = 0;
        openTiles = 9;
        circleWinCondition = false;
        triangleWinCondition = false;
        isGameOver = false;
    }
}
