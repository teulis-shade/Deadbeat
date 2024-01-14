using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baton : Item
{
    public override bool Use()
    {
        Tile facingTile = FindObjectOfType<PlayerController>().GetComponent<Entity>().CheckFacingTile();
        if (facingTile != null)
        {
            Entity entity = facingTile.GetEntity();
            if (entity != null)
            {
                entity.Die();
            }
        }
        return base.Use();
    }
}
