using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Color highlightColor = Color.cyan;
    private Color regularColor = Color.white;
    //Only 1 tile should be selected at a time (Unless player is swapping, then max is 2)
    public bool selected = false;
    public static float totalSelected = 0;
    public static float maxSelected = 1;
    private static List<GameObject> selectedTileList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //Set Sprite renderer
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    //Select and Highlight current tile and deselect previous tile if one has been selected.
    //If swapping, 2 tiles can be selected.
    private void OnMouseDown()
    {
        selected = !selected;

        if (selected)
        {
            AddTileList();

            if (selectedTileList.Count > maxSelected)
            {
                DeselectOtherTile();
            }

            sprite.color = highlightColor;
        }
        else
        {
            sprite.color = regularColor;
            selectedTileList.Remove(gameObject);
            totalSelected = selectedTileList.Count;
        }

        if (totalSelected > 0)
        {
            MakeMoveController.SetIsActive(true);
        }
        else
        {
            MakeMoveController.SetIsActive(false);
        }
    }

    private void AddTileList()
    {
        selectedTileList.Add(gameObject);
        totalSelected = selectedTileList.Count;
    }

    private void DeselectOtherTile()
    {
        //Oldest Selected tile will be at begenning of list
        GameObject objectToDeselect = selectedTileList[0];
        objectToDeselect.GetComponent<SpriteRenderer>().color = regularColor;
        objectToDeselect.GetComponent<TileController>().selected = false;
        selectedTileList.RemoveAt(0);
        totalSelected = selectedTileList.Count;
    }

    public static void PlaceLetter()
    {
        GameObject tile = selectedTileList[0];
        Debug.Log("Make Move Tile Name: " + tile.name);
        GameObject.Find("GameManager").GetComponent<GameManager>().MakeMove(tile.transform.position, tile.transform.rotation);
    }
}
