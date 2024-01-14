using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int xPosition;
    public int yPosition;
    [SerializeField] float alertnessModifier;
    private GameController gameController;
    [SerializeField] bool reusable;
    [SerializeField] bool startsOnPlayer;

    [SerializeField] private Vector3 offset;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        GetComponent<SpriteRenderer>().sortingOrder = -1;
    }

    public void SetPositionInit(Tile tile)
    {
        if (startsOnPlayer)
        {
            FindObjectOfType<PlayerController>().PickUpStart(this);
            startsOnPlayer = false;
        }
        else
        {
            transform.position = tile.GetPosition() + offset;
            tile.SetItem(this);
        }
    }

    /// <summary>
    /// Uses the item. Assumes that the player is holding the item.
    /// </summary>
    /// <returns>Returns false if the item is used up, and true if the item is reusable</returns>
    public virtual bool Use()
    {
        return reusable;
    }
}
