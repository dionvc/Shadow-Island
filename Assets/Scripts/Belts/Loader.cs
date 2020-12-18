using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour, IBeltElement, IRotate, IItemFilter
{
    IBeltElement outputBelt; //belt which feeds out
    IBeltElement inputBelt; //belt which feeds in
    Inventory inputInventory; //inventory which feeds into
    Inventory outputInventory; //inventory which items are sent to 
    ConveyorBelt.OutputOrientation outputOrientation;
    ItemEntity[] items;
    [SerializeField] int rotateSpritePropertyId = 1;
    [SerializeField] int inputBuffer = 4;
    int heldItems = 0;
    int filter = -1;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(inputInventory != null && heldItems < items.Length) //pull from input inventory
        {
            ItemEntity item = inputInventory.GetOutputItemEntity(this.transform.position, filter);
            if (item != null)
            {
                items[heldItems] = item;
                heldItems++;
            }
        }
        //try to pull from input belt (actually this is handled by prev belts) but this does give me the idea to fix belt wiggle maybe
        if(outputInventory != null && heldItems > 0) //push into output inventory
        {
            if (outputInventory.TryInsertItem(items[0].item))
            {
                items[0].ConsumeItem();
                for (int j = 0; j < items.Length - 1; j++)
                {
                    items[j] = items[j + 1];
                }
                items[items.Length - 1] = null;
                heldItems--;
            }
        }
        if(outputBelt != null && items[0] != null) //push into output belt
        {
            //push input output belt
            if(outputOrientation == ConveyorBelt.OutputOrientation.East)
            {
                if(outputBelt.TryInsertHorizontal(items[0], this.transform.position.x + 0.51f))
                {
                    Vector3 temp = this.transform.position;
                    temp.x += 0.51f;
                    items[0].transform.position = temp;
                    for (int j = 0; j < items.Length - 1; j++)
                    {
                        items[j] = items[j + 1];
                    }
                    items[items.Length - 1] = null;
                    heldItems--;
                }
            }
            else if(outputOrientation == ConveyorBelt.OutputOrientation.West)
            {
                if(outputBelt.TryInsertHorizontal(items[0], this.transform.position.x - 0.51f))
                {
                    Vector3 temp = this.transform.position;
                    temp.x -= 0.51f;
                    items[0].transform.position = temp;
                    for (int j = 0; j < items.Length - 1; j++)
                    {
                        items[j] = items[j + 1];
                    }
                    items[items.Length - 1] = null;
                    heldItems--;
                }
            }
            else if(outputOrientation == ConveyorBelt.OutputOrientation.North)
            {
                if(outputBelt.TryInsertVertical(items[0], this.transform.position.y + 0.51f))
                {
                    Vector3 temp = this.transform.position;
                    temp.y += 0.51f;
                    items[0].transform.position = temp;
                    for (int j = 0; j < items.Length - 1; j++)
                    {
                        items[j] = items[j + 1];
                    }
                    items[items.Length - 1] = null;
                    heldItems--;
                }
            }
            else
            {
                if(outputBelt.TryInsertVertical(items[0], this.transform.position.y - 0.51f))
                {
                    Vector3 temp = this.transform.position;
                    temp.y -= 0.51f;
                    items[0].transform.position = temp;
                    for (int j = 0; j < items.Length - 1; j++)
                    {
                        items[j] = items[j + 1];
                    }
                    items[items.Length - 1] = null;
                    heldItems--;
                }
            }
        }
    }

    public Sprite GetRotatedPlaceSprite(int rotation)
    {
        rotation = rotation % 4;
        return AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotateSpritePropertyId].sprites[rotation];
    }

    public void InitializeRotated(int rotation)
    {
        rotation = rotation % 4;
        items = new ItemEntity[inputBuffer];
        int countBelts = 0;
        int countMachines = 0;
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = 1 << LayerMask.NameToLayer("ConveyorBelt");
        ContactFilter2D machineFilter = new ContactFilter2D();
        machineFilter.useLayerMask = true;
        machineFilter.layerMask = 1 << LayerMask.NameToLayer("Damageable");
        Collider2D[] resultsBelts = new Collider2D[3];
        Collider2D[] resultsMachines = new Collider2D[3];
        switch(rotation)
        {
            case 0: //east output
                outputOrientation = ConveyorBelt.OutputOrientation.East;
                countBelts = Physics2D.OverlapBox(this.transform.position, new Vector2(2, 0.25f), 0, filter, resultsBelts);
                countMachines = Physics2D.OverlapBox(this.transform.position, new Vector2(2, 0.25f), 0, machineFilter, resultsMachines);
                for(int i = 0; i < countBelts; i++)
                {
                    if (resultsBelts[i].gameObject == this.gameObject) continue;
                    if (resultsBelts[i].transform.position.x < this.transform.position.x)
                    {
                        IBeltElement temp = resultsBelts[i].GetComponent<IBeltElement>();
                        if(temp.GetOutputOrientation() == ConveyorBelt.OutputOrientation.East)
                        {
                            inputBelt = temp;
                            temp.SetNextBeltElement(this);
                        }
                    }
                    else if(resultsBelts[i].transform.position.x > this.transform.position.x)
                    {
                        IBeltElement temp = resultsBelts[i].GetComponent<IBeltElement>();
                        if(temp.CanConnectToBeltElement(this))
                        {
                            outputBelt = temp;
                            outputBelt.SetPrevBeltElement(this);
                        }
                    }
                }

                for(int i = 0; i < countMachines; i++)
                {
                    if(resultsMachines[i].transform.position.x < this.transform.position.x)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out inputInventory);
                    }
                    else if(resultsMachines[i].transform.position.x > this.transform.position.x)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out outputInventory);
                    }
                }
                break;
            case 1: //north output
                outputOrientation = ConveyorBelt.OutputOrientation.North;
                countBelts = Physics2D.OverlapBox(this.transform.position, new Vector2(0.25f, 2), 0, filter, resultsBelts);
                countMachines = Physics2D.OverlapBox(this.transform.position, new Vector2(0.25f, 2), 0, machineFilter, resultsMachines);

                for (int i = 0; i < countBelts; i++)
                {
                    if (resultsBelts[i].gameObject == this.gameObject) continue;
                    if (resultsBelts[i].transform.position.y < this.transform.position.y)
                    {
                        IBeltElement temp = resultsBelts[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == ConveyorBelt.OutputOrientation.North)
                        {
                            inputBelt = temp;
                            temp.SetNextBeltElement(this);
                        }
                    }
                    else if (resultsBelts[i].transform.position.y > this.transform.position.y)
                    {
                        IBeltElement temp = resultsBelts[i].GetComponent<IBeltElement>();
                        if (temp.CanConnectToBeltElement(this))
                        {
                            outputBelt = temp;
                            outputBelt.SetPrevBeltElement(this);
                        }
                    }
                }

                for (int i = 0; i < countMachines; i++)
                {
                    if (resultsMachines[i].transform.position.y < this.transform.position.y)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out inputInventory);
                    }
                    else if (resultsMachines[i].transform.position.y > this.transform.position.y)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out outputInventory);
                    }
                }
                break;
            case 2: //west output
                outputOrientation = ConveyorBelt.OutputOrientation.West;
                countBelts = Physics2D.OverlapBox(this.transform.position, new Vector2(2, 0.25f), 0, filter, resultsBelts);
                countMachines = Physics2D.OverlapBox(this.transform.position, new Vector2(2, 0.25f), 0, machineFilter, resultsMachines);
                for (int i = 0; i < countBelts; i++)
                {
                    if (resultsBelts[i].gameObject == this.gameObject) continue;
                    if (resultsBelts[i].transform.position.x > this.transform.position.x)
                    {
                        IBeltElement temp = resultsBelts[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == ConveyorBelt.OutputOrientation.West)
                        {
                            inputBelt = temp;
                            temp.SetNextBeltElement(this);
                        }
                    }
                    else if (resultsBelts[i].transform.position.x < this.transform.position.x)
                    {
                        IBeltElement temp = resultsBelts[i].GetComponent<IBeltElement>();
                        if (temp.CanConnectToBeltElement(this))
                        {
                            outputBelt = temp;
                            outputBelt.SetPrevBeltElement(this);
                        }
                    }
                }

                for (int i = 0; i < countMachines; i++)
                {
                    if (resultsMachines[i].transform.position.x > this.transform.position.x)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out inputInventory);
                    }
                    else if (resultsMachines[i].transform.position.x < this.transform.position.x)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out outputInventory);
                    }
                }
                break;
            default:
            case 3: //south output
                outputOrientation = ConveyorBelt.OutputOrientation.South;
                countBelts = Physics2D.OverlapBox(this.transform.position, new Vector2(0.25f, 2), 0, filter, resultsBelts);
                countMachines = Physics2D.OverlapBox(this.transform.position, new Vector2(0.25f, 2), 0, machineFilter, resultsMachines);

                for (int i = 0; i < countBelts; i++)
                {
                    if (resultsBelts[i].gameObject == this.gameObject) continue;
                    if (resultsBelts[i].transform.position.y > this.transform.position.y)
                    {
                        IBeltElement temp = resultsBelts[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == ConveyorBelt.OutputOrientation.South)
                        {
                            inputBelt = temp;
                            temp.SetNextBeltElement(this);
                        }
                    }
                    else if (resultsBelts[i].transform.position.y < this.transform.position.y)
                    {
                        IBeltElement temp = resultsBelts[i].GetComponent<IBeltElement>();
                        if (temp.CanConnectToBeltElement(this))
                        {
                            outputBelt = temp;
                            outputBelt.SetPrevBeltElement(this);
                        }
                    }
                }

                for (int i = 0; i < countMachines; i++)
                {
                    if (resultsMachines[i].transform.position.y > this.transform.position.y)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out inputInventory);
                    }
                    else if (resultsMachines[i].transform.position.y < this.transform.position.y)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out outputInventory);
                    }
                }
                break;
        }
        this.GetComponent<SpriteRenderer>().sprite = AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotateSpritePropertyId].sprites[rotation];
    }

    public void CheckForInventory()
    {
        int countMachines;
        ContactFilter2D machineFilter = new ContactFilter2D();
        machineFilter.useLayerMask = true;
        machineFilter.layerMask = 1 << LayerMask.NameToLayer("Damageable");
        Collider2D[] resultsMachines = new Collider2D[3];
        switch(outputOrientation)
        {
            case ConveyorBelt.OutputOrientation.East:
                countMachines = Physics2D.OverlapBox(this.transform.position, new Vector2(2, 0.25f), 0, machineFilter, resultsMachines);
                for (int i = 0; i < countMachines; i++)
                {
                    if (resultsMachines[i].transform.position.x < this.transform.position.x)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out inputInventory);
                    }
                    else if (resultsMachines[i].transform.position.x > this.transform.position.x)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out outputInventory);
                    }
                }
                break;
            case ConveyorBelt.OutputOrientation.North:
                countMachines = Physics2D.OverlapBox(this.transform.position, new Vector2(0.25f, 2), 0, machineFilter, resultsMachines);
                for (int i = 0; i < countMachines; i++)
                {
                    if (resultsMachines[i].transform.position.y < this.transform.position.y)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out inputInventory);
                    }
                    else if (resultsMachines[i].transform.position.y > this.transform.position.y)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out outputInventory);
                    }
                }
                break;
            case ConveyorBelt.OutputOrientation.West:
                countMachines = Physics2D.OverlapBox(this.transform.position, new Vector2(2, 0.25f), 0, machineFilter, resultsMachines);
                for (int i = 0; i < countMachines; i++)
                {
                    if (resultsMachines[i].transform.position.x > this.transform.position.x)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out inputInventory);
                    }
                    else if (resultsMachines[i].transform.position.x < this.transform.position.x)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out outputInventory);
                    }
                }
                break;
            default:
            case ConveyorBelt.OutputOrientation.South:
                countMachines = Physics2D.OverlapBox(this.transform.position, new Vector2(0.25f, 2), 0, machineFilter, resultsMachines);
                for (int i = 0; i < countMachines; i++)
                {
                    if (resultsMachines[i].transform.position.y > this.transform.position.y)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out inputInventory);
                    }
                    else if (resultsMachines[i].transform.position.y < this.transform.position.y)
                    {
                        resultsMachines[i].gameObject.TryGetComponent(out outputInventory);
                    }
                }
                break;
        }
    }

    public void SetNextBeltElement(IBeltElement beltElement)
    {
        outputBelt = beltElement;
    }

    public void SetPrevBeltElement(IBeltElement beltElement)
    {
        inputBelt = beltElement;
    }

    public ConveyorBelt.OutputOrientation GetOutputOrientation()
    {
        return outputOrientation;
    }

    public bool TryInsertVertical(ItemEntity item, float y)
    {
        if(heldItems < items.Length && (filter == -1 || item.item.id == filter))
        {
            item.transform.position = this.transform.position;
            items[heldItems] = item;
            heldItems++;
            return true;
        }
        return false;
    }

    public bool TryInsertHorizontal(ItemEntity item, float x)
    {
        if(heldItems < items.Length && (filter == -1 || item.item.id == filter))
        {
            item.transform.position = this.transform.position;
            items[heldItems] = item;
            heldItems++;
            return true;
        }
        return false;
    }

    public bool CheckCollisionVertical(float y)
    {
        return false;
    }

    public bool CheckCollisionHorizontal(float x)
    {
        return false;
    }

    public bool CanConnectToBeltElement(IBeltElement prevElement)
    {
        return prevElement.GetOutputOrientation() == outputOrientation;
    }

    public Vector2 GetRotatedBoundingBox(int rotation)
    {
        rotation = rotation % 4;
        return this.GetComponent<BoxCollider2D>().size;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public int GetFilterItem()
    {
        return filter;
    }
    public string GetFilter()
    {
        if(filter == -1)
        {
            return "None";
        }
        else
        {
            return "Enabled";
        }
    }

    public void SetFilter(string filterName, int itemId)
    {
        if (filterName == "Enabled")
        {
            filter = itemId;
        }
        else
        {
            filter = -1;
        }
    }

    public string[] GetFilterNames()
    {
        return new string[] { "Enabled" };
    }
}
