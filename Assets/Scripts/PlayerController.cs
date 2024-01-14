using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    GameController gameController;
    Entity playerEntity;
    [SerializeField] int alertRadius;

    [SerializeField] Item leftItem;
    [SerializeField] Item rightItem;
    [SerializeField] Image leftHand;
    [SerializeField] Image rightHand;
    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        playerEntity = GetComponent<Entity>();
    }
    private void Update()
    {
        if (leftItem != null)
        {
            leftHand.enabled = true;
            leftHand.sprite = leftItem.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            leftHand.enabled = false;
        }
        if (rightItem != null)
        {
            rightHand.enabled = true;
            rightHand.sprite = rightItem.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            rightHand.enabled = false;
        }
    }
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        if (gameController.CheckInput())
        {
            Debug.Log("YOU MADE IT");
        }
        else
        {
            Debug.Log("YOU MISSED THE BEAT");
        }
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        Vector2 movement = ctx.ReadValue<Vector2>();
        if (Mathf.Abs(movement.x) >= Mathf.Abs(movement.y))
        {
            if (movement.x < 0)
            {
                playerEntity.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                playerEntity.GetComponent<SpriteRenderer>().flipX = false;
            }
            playerEntity.Move((int)(movement.x * 1.5f), 0);
        }
        else
        {
            playerEntity.Move(0, (int)(movement.y * 1.5f));
        }
        if (gameController.CheckInput())
        {
            Debug.Log("Hit Beat");
        }
        else
        {
            foreach (Tile tile in gameController.CheckCircle(playerEntity.xPosition, playerEntity.yPosition, alertRadius))
            {
                if (tile.CheckEntity())
                {
                    if (tile.GetEntity() != playerEntity)
                    {
                        tile.GetEntity().IncreaseSuspicion(1 + (leftItem == null ?  0 : leftItem.alertnessModifier) + (rightItem == null ? 0 : rightItem.alertnessModifier));
                    }
                }
            }
            Debug.Log("AAAAAAAAAAHHHHHHHH");
        }
    }

    public void OnItemLeftDrop(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        if (!gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).CheckItem())
        {
            if (leftItem == null)
            {
                return;
            }
            leftItem.gameObject.SetActive(true);
            leftItem.SetPositionInit(gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition));
            leftItem = null;
        }
        else
        {
            Item currentHeld = leftItem;
            leftItem = gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).GetItem();
            gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).TakeItem();
            if (currentHeld != null)
            {
                currentHeld.gameObject.SetActive(true);
                currentHeld.SetPositionInit(gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition));
            }
        }
    }

    public void OnItemRightDrop(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        if (!gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).CheckItem())
        {
            if (rightItem == null)
            {
                return;
            }
            rightItem.gameObject.SetActive(true);
            rightItem.SetPositionInit(gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition));
            rightItem = null;
        }
        else
        {
            Item currentHeld = rightItem;
            rightItem = gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).GetItem();
            gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).TakeItem();
            if (currentHeld != null)
            {
                currentHeld.gameObject.SetActive(true);
                currentHeld.SetPositionInit(gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition));
            }
        }
    }

    public void OnItemRight(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        if (!gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).CheckItem())
        {
            if (rightItem == null)
            {
                return;
            }
            if (!rightItem.Use())
            {
                rightItem = null;
            }
        }
        else
        {
            Item currentHeld = rightItem;
            rightItem = gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).GetItem();
            gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).TakeItem();
            if (currentHeld != null)
            {
                currentHeld.gameObject.SetActive(true);
                currentHeld.SetPositionInit(gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition));
            }
        }
    }

    public void OnItemLeft(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        if (!gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).CheckItem())
        {
            if (leftItem == null)
            {
                return;
            }
            if (!leftItem.Use())
            {
                leftItem = null;
            }
        }
        else
        {
            Item currentHeld = leftItem;
            leftItem = gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).GetItem();
            gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition).TakeItem();
            if (currentHeld != null)
            {
                currentHeld.gameObject.SetActive(true);
                currentHeld.SetPositionInit(gameController.GetTile(playerEntity.xPosition, playerEntity.yPosition));
            }
        }
    }

    public void PickUpStart(Item item)
    {
        if (rightItem == null)
        {
            rightItem = item;
            item.gameObject.SetActive(false);
        }
        else if (leftItem == null)
        {
            leftItem = item;
            item.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("There are 3 items that begin on the players person");
        }
    }
}
