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
        if (x == 1)
        {
            currentDirection = FacingDirection.RIGHT;
        }
        else if (x == -1)
        {
            currentDirection = FacingDirection.LEFT;
        }
        else if (y == 1)
        {
            currentDirection = FacingDirection.UP;
        }
        else if (y == -1) 
        {
            currentDirection = FacingDirection.DOWN;
        }
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
        if (GetComponent<PlayerController>() != null)
        {
            return;
        }
        FindObjectOfType<SuspicionMeter>().IncreaseSuspicion(severity * alertness);
        StopCoroutine("SuspicionCoroutine");
        StartCoroutine("SuspicionCoroutine");
    }

    public void Die()
    {
        Debug.Log("OWWWWWWWWW");
        if (GetComponent<MoveRoutine>() != null)
        {
            Destroy(GetComponent<MoveRoutine>());
        }
        foreach (Tile tile in gameController.CheckCircle(xPosition, yPosition, 4))
        {
            if (tile.GetEntity() != null)
            {
                tile.GetEntity().IncreaseSuspicion(5);
            }
        }
        GetComponent<SpriteRenderer>().color = Color.green;
        gameController.GetTile(xPosition, yPosition).EntityLeaves();
        Item item = gameObject.AddComponent<Item>();
        gameController.GetTile(xPosition, yPosition).SetItem(item);
        item.xPosition = xPosition;
        item.yPosition = yPosition;
        item.alertnessModifier = .5f;
        item.reusable = true;
        Destroy(this);
    }

    IEnumerator SuspicionCoroutine()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(.5f);
        GetComponent<SpriteRenderer>().color = Color.white;

    }

    public Tile CheckFacingTile()
    {
        switch (currentDirection)
        {
            case FacingDirection.UP:
                return gameController.GetTile(xPosition, yPosition + 1);

            case FacingDirection.RIGHT:
                return gameController.GetTile(xPosition + 1, yPosition);

            case FacingDirection.LEFT:
                return gameController.GetTile(xPosition - 1, yPosition);

            case FacingDirection.DOWN:
                return gameController.GetTile(xPosition, yPosition - 1);

            default:
                return null;
        }
    }
}
