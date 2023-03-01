using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreLines : MonoBehaviour
{
    GameManager gameManager;
    public List<GameObject> collidedCircleList;
    public List<GameObject> collidedTriangleList;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected");
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
        gameManager.collisionsFinished = true;
    }

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
