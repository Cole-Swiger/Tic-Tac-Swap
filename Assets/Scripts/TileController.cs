using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    //references
    private SpriteRenderer sprite;
    GameManager gameManager;

    //Tile colors
    private Color highlightColor = new(0.329f, 0.286f, 0.659f);
    private Color regularColor = Color.white;
    private Color unplayableColor = Color.gray;

    //Only 1 tile should be selected at a time (Unless player is swapping, then max is 2)
    //total and max selected, and the list of selected tiles will be same for all instances.
    public bool isSelectable = true;
    public bool selected = false;
    public bool hasBeenSwapped = false;
    public static int maxSelected = 1;
    public static List<GameObject> selectedTileList = new List<GameObject>();

    //Used for game state in GameManager
    public int row;
    public int column;

    //Audio
    private AudioSource tileAudio;
    public AudioClip selectSound;
    public AudioClip deselectSound;

    // Start is called before the first frame update
    void Start()
    {
        //Set Sprite renderer and GameManager
        sprite = gameObject.GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        tileAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Set tile color
        if (selected)
        {
            GetComponent<SpriteRenderer>().color = highlightColor;
        }
        else if (hasBeenSwapped)
        {
            GetComponent<SpriteRenderer>().color = unplayableColor;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = regularColor;
        }
    }

    //Select and Highlight current tile and deselect previous tile if one has been selected.
    //If swapping, 2 tiles can be selected before one is deselected.
    private void OnMouseDown()
    {
        if (!isSelectable || gameManager.isGameOver)
        {
            return;
        }
        selected = !selected;
        maxSelected = gameManager.GetMaxSelected();

        //If clicked tile wasn't selected before, add to list and highlight it. Otherwise deselect it.
        if (selected)
        {
            AddTileList();

            //Deselect oldest selected tile if over max selected.
            if (selectedTileList.Count > maxSelected)
            {
                GameObject objectToDeselect = selectedTileList[0];
                ResetSelected(objectToDeselect);
            }

            sprite.color = highlightColor;
            tileAudio.PlayOneShot(selectSound, 1);
        }
        else
        {
            ResetSelected(gameObject);
            tileAudio.PlayOneShot(deselectSound, 1);
        }
        gameManager.SetMakeMoveActive();
    }

    //Add a tile to the end of the selected tile list and update total.
    private void AddTileList()
    {
        selectedTileList.Add(gameObject);
        gameManager.totalTilesSelected = selectedTileList.Count;
    }

    //Deslect all selected tiles
    public static void ResetAllSelected()
    {
        foreach (GameObject g in selectedTileList)
        {
            g.GetComponent<TileController>().selected = false;
            g.GetComponent<SpriteRenderer>().color = g.GetComponent<TileController>().regularColor;
        }
        selectedTileList.Clear();
        GameObject.Find("GameManager").GetComponent<GameManager>().totalTilesSelected = selectedTileList.Count;
    }

    //Deslect specified selcted tile
    private void ResetSelected(GameObject gameObj)
    {
        gameObj.GetComponent<TileController>().selected = false;
        gameObj.GetComponent<SpriteRenderer>().color = regularColor;
        selectedTileList.Remove(gameObj);
        gameManager.totalTilesSelected = selectedTileList.Count;
    }
}
