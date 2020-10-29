using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseRequester : MonoBehaviour
{

    enum RequestState
    {
        called,
        notCalled
    }
    WarehouseManager warehouse;
    InventoryRecipe inventoryRecipe;
    RequestState request = RequestState.notCalled;
    [SerializeField] LayerMask mask = 0;
    int counter = 0;

    // Start is called before the first frame update
    void Start()
    {

        inventoryRecipe = GetComponent<InventoryRecipe>();
        ContactFilter2D filter;
        filter = new ContactFilter2D();
        filter.layerMask = mask;
        filter.useTriggers = true;
        filter.useLayerMask = true;
        Collider2D[] collider = new Collider2D[1];
        if(Physics2D.OverlapBox(transform.position, GetComponent<BoxCollider2D>().size, 0, filter, collider) == 1)
        {
            warehouse = collider[0].gameObject.transform.parent.GetComponent<WarehouseManager>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Functionality: call for items when they drop below 2x ingredients count, but only check periodically
        counter++;
        if(counter > 60)
        {
            if (warehouse != null && request != RequestState.called && inventoryRecipe.NeedGather())
            {
                warehouse.RequestGatherer(this);
                request = RequestState.called;
            }
            counter = 0;
        }
        
    }

    public List<ItemStack> GatherResources()
    {
        request = RequestState.notCalled;
        return inventoryRecipe.GatherResources();
    }
}
