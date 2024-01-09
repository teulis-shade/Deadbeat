using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int xPosition;
    public int yPosition;
    Item heldItem;
    private GameController gameController;

    private Vector3 offset;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(int x, int y)
    {
        xPosition += x;
        yPosition += y;
        transform.position = gameController.GetTile(xPosition, yPosition).GetPosition() + offset;
    }

    public void SetPositionInit(Tile tile)
    {
        transform.position = tile.GetPosition() + offset;
    } 
}
