using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeMoveController : MonoBehaviour
{
    public static bool isActive = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (isActive)
        {
            Debug.Log("Button Pressed");
            TileController.PlaceLetter();
        }
        else
        {
            Debug.Log("Please select a tile before making a move");
        }
    }

    public static void SetIsActive(bool active)
    {
        isActive = active;
    }
}
