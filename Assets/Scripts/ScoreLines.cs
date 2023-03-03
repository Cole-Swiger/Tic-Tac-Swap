using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreLines : MonoBehaviour
{
    GameManager gameManager;
    //Keep track of circles and triangles in each row, column, and diagonal
    public List<GameObject> collidedCircleList;
    public List<GameObject> collidedTriangleList;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    //Add collided shape to appropriate list
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject otherObject = collision.gameObject;
        if (otherObject.CompareTag("Letter"))
        {
            if (otherObject.name.Contains("Circle")) {
                collidedCircleList.Add(otherObject);
            }
            else
            {
                collidedTriangleList.Add(otherObject);
            }
        }
        //Tells the GameManager to calculate the game state
        gameManager.collisionsFinished = true;
    }

    //When swapping, remove old object from list
    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject otherObject = collision.gameObject;
        if (collision.gameObject.CompareTag("Letter"))
        {
            if (otherObject.name.Contains("Circle")) {
                collidedCircleList.Remove(otherObject);
            }
            else
            {
                collidedTriangleList.Remove(otherObject);
            }
        }
    }
}
