using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void EntityEnters()
    {

    }
}
