using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseManager : MonoBehaviour
{
    [SerializeField] GameObject resourceGatherer;
    [SerializeField] GameObject[] ports;
    int resourceGathererCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
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

    public void RequestGatherer(BuildingManager building)
    {
        //instantiate a resourceGatherer to go to building and get resources
        ResourcePather rp = Instantiate(resourceGatherer, RequestPort().transform.position, Quaternion.identity).GetComponent<ResourcePather>();
        rp.SetBuilding(building);
        rp.SetWarehouse(this);
    }
}
