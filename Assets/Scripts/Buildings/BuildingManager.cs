using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] WarehouseManager warehouse;
    int resourceCount = 0;
    int productionTime = 60;
    int counter = 0;
    int callGatherer = 10;
    int maxResource = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        counter++;
        if(counter >= productionTime)
        {
            counter = 0;
            resourceCount++;
            if(resourceCount >= callGatherer)
            {
                warehouse.RequestGatherer(this);
            }
        }
    }

    public void GatherResource()
    {
        resourceCount = 0;
    }
}
