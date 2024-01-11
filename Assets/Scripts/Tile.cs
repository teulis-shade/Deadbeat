using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Tile : MonoBehaviour
{
    private Entity occupyingEntity;
    private Item occupyingItem;

    void Start()
    {
        
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEntity(Entity entity)
    {
        if (occupyingEntity != null)
        {
            Debug.LogError("HEY, " + occupyingEntity.gameObject.name + " is already at the position that " + entity.gameObject.name + " is in.");
            return;
        }
        occupyingEntity = entity;
    }

    public void EntityLeaves()
    {
        occupyingEntity = null;
    }

    public bool CheckEntity()
    {
        return occupyingEntity != null;
    }

    public Entity GetEntity()
    {
        return occupyingEntity;
    }

    public void TakeItem()
    {
        occupyingItem.gameObject.SetActive(false);
        occupyingItem = null;
    }

    public void SetItem(Item item)
    {
        if (occupyingItem != null)
        {
            Debug.LogError("HEY, " + occupyingItem.gameObject.name + " is already at the position that " + item.gameObject.name + " is in.");
            return;
        }
        occupyingItem = item;
    }

    public bool CheckItem()
    {
        return occupyingItem != null;
    }

    public Item GetItem()
    {
        return occupyingItem;
    }
}
