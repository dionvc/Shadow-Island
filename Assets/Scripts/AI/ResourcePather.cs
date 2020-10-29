using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePather : MonoBehaviour
{
    public enum MoveState
    {
        toBuilding,
        atBuilding,
        toWarehouse,
        requestingPort,
        enteringPort
    }

    public enum ResourceState
    {
        resource,
        noResource
    }

    List<ItemStack> resources;
    WarehouseManager warehouseManager;
    WarehouseRequester warehouseRequester;
    MoveState moveState = MoveState.toBuilding;
    ResourceState resourceState = ResourceState.noResource;
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
        if (moveState == MoveState.toBuilding)
        {
            //going to building
            move = warehouseRequester.transform.position - this.transform.position;
            if (move.magnitude < satisfactionRadius)
            {
                //deposit resources
                moveState = MoveState.atBuilding;
            }
            else
            {
                this.transform.position += move.normalized * speed;
            }
        }
        else if(moveState == MoveState.atBuilding)
        {
            counter++;
            if(counter >= gatheringTime)
            {
                counter = 0;
                moveState = MoveState.toWarehouse;
                resources = warehouseRequester.GatherResources();
            }
        }
        else if(moveState == MoveState.toWarehouse)
        {
            move = warehouseManager.transform.position - this.transform.position;
            if (move.magnitude < warehouseRequestRadius)
            {
                targetPort = warehouseManager.RequestPort().transform.position;
                moveState = MoveState.requestingPort;
            }
            else
            {
                this.transform.position += move.normalized * speed;
            }
        }
        else if(moveState == MoveState.requestingPort)
        {
            counter++;
            if(counter > requestWait)
            {
                moveState = MoveState.enteringPort;
                counter = 0;
            }
        }
        else if(moveState == MoveState.enteringPort)
        {
            move = targetPort - this.transform.position;
            if(move.magnitude < satisfactionRadius)
            {
                warehouseManager.DepositResources(resources);
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
            srenderer.sprite = regular[(int)moveState%2];
        }
        else if(move.x > 0 && move.x > Mathf.Abs(move.y))
        {
            srenderer.sprite = right[(int)moveState%2];
        }
        else if(move.x < 0 && -move.x > Mathf.Abs(move.y))
        {
            srenderer.sprite = left[(int)moveState%2];
        }
        else if(move.y > 0 && move.y > Mathf.Abs(move.x))
        {
            srenderer.sprite = up[(int)moveState%2];
        }
        else if(move.y < 0 && move.y > Mathf.Abs(move.x))
        {
            srenderer.sprite = down[(int)moveState%2];
        }
    }

    public void SetRequester(WarehouseRequester requester)
    {
        this.warehouseRequester = requester;
    }

    public void SetWarehouse(WarehouseManager warehouseManager)
    {
        this.warehouseManager = warehouseManager;
    }
}
