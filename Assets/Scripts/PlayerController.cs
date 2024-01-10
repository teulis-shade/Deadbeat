using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    GameController gameController;
    Entity playerEntity;
    [SerializeField] int alertRadius;
    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        playerEntity = GetComponent<Entity>();
    }
    public void OnInteract()
    {
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
                        tile.GetEntity().IncreaseSuspicion(1);
                        tile.GetEntity().GetComponent<SpriteRenderer>().color = Color.red;
                    }
                }
            }
            Debug.Log("AAAAAAAAAAHHHHHHHH");
        }
    }
}
