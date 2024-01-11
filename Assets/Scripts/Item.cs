using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int xPosition;
    public int yPosition;
    [SerializeField] float alertnessModifier;
    private GameController gameController;

    [SerializeField] private Vector3 offset;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    public void SetPositionInit(Tile tile)
    {
        gameController = FindObjectOfType<GameController>();
        transform.position = tile.GetPosition() + offset;
        gameController.GetTile(xPosition, yPosition).SetItem(this);
    }

    /// <summary>
    /// Uses the item. Assumes that the player is holding the item.
    /// </summary>
    /// <returns>Returns false if the item is used up, and true if the item is reusable</returns>
    public virtual bool Use()
    {
        return false;
    }
}
