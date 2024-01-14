using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int xPosition;
    public int yPosition;
    [SerializeField] float alertness;
    Item heldItem;
    private GameController gameController;


    public enum FacingDirection
    {
        LEFT, RIGHT, UP, DOWN
    }

    public FacingDirection currentDirection; 

    [SerializeField] private Vector3 offset;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    public bool Move(int x, int y)
    {
        if (gameController.GetTile(xPosition + x, yPosition + y) == null)
        {
            return false;
        }
        if (!gameController.GetTile(xPosition + x, yPosition + y).CheckEntity())
        {
            gameController.GetTile(xPosition, yPosition).EntityLeaves();
            xPosition += x;
            yPosition += y;
            transform.position = gameController.GetTile(xPosition, yPosition).GetPosition() + offset;
            gameController.GetTile(xPosition, yPosition).SetEntity(this);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetPositionInit(Tile tile)
    {
        gameController = FindObjectOfType<GameController>();
        transform.position = tile.GetPosition() + offset;
        gameController.GetTile(xPosition, yPosition).SetEntity(this);
    }

    public void IncreaseSuspicion(int severity)
    {
        FindObjectOfType<SuspicionMeter>().IncreaseSuspicion(severity * alertness);
    }

    public void Die()
    {
        if (heldItem != null)
        {

        }
    }
}
