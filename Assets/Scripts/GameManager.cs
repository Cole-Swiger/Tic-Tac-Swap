using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject[] shapesPrefabs;
    public int currentTurn = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject GetCurrentTurn()
    {
        currentTurn = currentTurn % 2;
        return shapesPrefabs[currentTurn];
    }

    public void MakeMove(Vector3 tilePosition, Quaternion tileRotation)
    {
        Instantiate(GetCurrentTurn(), tilePosition, tileRotation);
        currentTurn++;
    }
}
