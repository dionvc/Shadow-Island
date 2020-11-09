using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseManager : MonoBehaviour
{
    [SerializeField] GameObject resourceGatherer;
    [SerializeField] GameObject[] ports;
    Inventory inventory;
    int resourceGathererCount = 60;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public GameObject RequestPort()
    {
        //returns a port for the resource gatherer to enter
        return ports[0];
    }

    public void RequestGatherer(WarehouseRequester requester, List<Ingredient> requestedItems)
    {
        //instantiate a resourceGatherer to go to building and get resources
        ResourcePather rp = Instantiate(resourceGatherer, RequestPort().transform.position, Quaternion.identity).GetComponent<ResourcePather>();
        rp.SetRequester(requester);
        rp.SetWarehouse(this);
    }

    public bool DepositResources(List<ItemStack> resources)
    {
        for(int i = 0; i < resources.Count; i++)
        {
            resources[i] = inventory.InsertStack(resources[i]);
            if(resources[i] != null)
            {
                return false;
            }
        }
        return true;
    } 
}
