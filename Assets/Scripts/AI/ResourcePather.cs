using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePather : MonoBehaviour
{
    public enum GatheringState
    {
        NoResource = 1,
        Resource = 0,
        EnteringPort = 2,
        Gathering = 3,
        RequestingPort = 4
    }
    WarehouseManager warehouseManager;
    BuildingManager buildingManager;
    GatheringState gatheringState = GatheringState.NoResource;
    Vector3 targetPort = Vector3.zero;
    Vector3 move = Vector3.zero;
    float satisfactionRadius = 0.25f;
    float warehouseRequestRadius = 5.0f;
    float speed = 0.1f;
    [SerializeField] Sprite[] regular = null;
    [SerializeField] Sprite[] up = null;
    [SerializeField] Sprite[] down = null;
    [SerializeField] Sprite[] left = null;
    [SerializeField] Sprite[] right = null;
    SpriteRenderer srenderer = null;
    int counter = 0;
    int gatheringTime = 60;
    int requestWait = 60;

    private void Start()
    {
        srenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move = new Vector3(0, 0, 0);
        if (gatheringState == GatheringState.NoResource)
        {
            //going to building
            move = buildingManager.transform.position - this.transform.position;
            if (move.magnitude < satisfactionRadius)
            {
                buildingManager.GatherResource();
                gatheringState = GatheringState.Gathering;
            }
            else
            {
                this.transform.position += move.normalized * speed;
            }
        }
        else if(gatheringState == GatheringState.Gathering)
        {
            counter++;
            if(counter >= gatheringTime)
            {
                counter = 0;
                gatheringState = GatheringState.Resource;
            }
        }
        else if(gatheringState == GatheringState.Resource)
        {
            //going to warehouse
            move = warehouseManager.transform.position - this.transform.position;
            if (move.magnitude < warehouseRequestRadius)
            {
                targetPort = warehouseManager.RequestPort().transform.position;
                gatheringState = GatheringState.RequestingPort;
            }
            else
            {
                this.transform.position += move.normalized * speed;
            }
        }
        else if(gatheringState == GatheringState.RequestingPort)
        {
            counter++;
            if(counter > requestWait)
            {
                gatheringState = GatheringState.EnteringPort;
                counter = 0;
            }
        }
        else if(gatheringState == GatheringState.EnteringPort)
        {
            move = targetPort - this.transform.position;
            if(move.magnitude < satisfactionRadius)
            {
                Destroy(this.gameObject);
            }
            else
            {
                this.transform.position += move.normalized * speed;
            }
        }
        
        //handle sprite representation
        if(move.x == 0 && move.y == 0)
        {
            srenderer.sprite = regular[(int)gatheringState%2];
        }
        else if(move.x > 0 && move.x > Mathf.Abs(move.y))
        {
            srenderer.sprite = right[(int)gatheringState%2];
        }
        else if(move.x < 0 && -move.x > Mathf.Abs(move.y))
        {
            srenderer.sprite = left[(int)gatheringState%2];
        }
        else if(move.y > 0 && move.y > Mathf.Abs(move.x))
        {
            srenderer.sprite = up[(int)gatheringState%2];
        }
        else if(move.y < 0 && move.y > Mathf.Abs(move.x))
        {
            srenderer.sprite = down[(int)gatheringState%2];
        }
    }

    public void SetBuilding(BuildingManager buildingManager)
    {
        this.buildingManager = buildingManager;
    }

    public void SetWarehouse(WarehouseManager warehouseManager)
    {
        this.warehouseManager = warehouseManager;
    }
}
