using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseRequester : MonoBehaviour
{
    WarehouseManager warehouse;
    InventoryRecipe inventoryRecipe;

    // Start is called before the first frame update
    void Start()
    {
        ContactFilter2D filter;
        filter = new ContactFilter2D();
        filter.layerMask = LayerMask.NameToLayer("WarehouseNetwork");
        filter.useTriggers = true;
        filter.useLayerMask = true;
        Collider2D[] collider = new Collider2D[1];
        if(Physics2D.OverlapBox(transform.position, GetComponent<BoxCollider2D>().size, 0, filter, collider) == 1)
        {
            warehouse = collider[0].gameObject.GetComponent<WarehouseManager>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Functionality: call for items when they drop below 2x ingredients count, but only check periodically
    }
}
