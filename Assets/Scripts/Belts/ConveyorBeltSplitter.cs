using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltSplitter : MonoBehaviour, IRotate, IBeltElement, IItemFilter
{
    IBeltElement leftInput; //left corresponds to above
    IBeltElement rightInput;
    IBeltElement leftOutput;
    IBeltElement rightOutput;
    ConveyorBelt.OutputOrientation outputOrientation;
    [SerializeField] int rotatedSpritePropertyId = 2;
    [SerializeField] int maxBufferCapacity = 8;
    int heldItems = 0;
    ItemEntity[] items;
    int modFrame = 0;
    int filter = -1;
    int filterSide = -1; //-1 means no filter
    string[] filterNames = {"Left", "Right"};

    // Update is called once per frame
    void FixedUpdate()
    {
        modFrame %= 2;
        if (items[0] != null)
        {
            if (
                (
                ((modFrame == 0 || rightOutput == null) && filterSide == -1) || 
                ((filterSide == 0 && filter == items[0].item.id) || (filterSide == 1 && filter != items[0].item.id))
                ) &&
                leftOutput != null)
            {
                if (outputOrientation == ConveyorBelt.OutputOrientation.East)
                {
                    if (leftOutput.TryInsertHorizontal(items[0], this.transform.position.x + 0.51f))
                    {
                        Vector3 temp = this.transform.position;
                        temp.x += 0.51f;
                        temp.y += 0.5f;
                        items[0].transform.position = temp;
                        for (int j = 0; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[items.Length - 1] = null;
                        heldItems--;
                    }
                }
                else if (outputOrientation == ConveyorBelt.OutputOrientation.West)
                {
                    if (leftOutput.TryInsertHorizontal(items[0], this.transform.position.x - 0.51f))
                    {
                        Vector3 temp = this.transform.position;
                        temp.x -= 0.51f;
                        temp.y += 0.5f;
                        items[0].transform.position = temp;
                        for (int j = 0; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[items.Length - 1] = null;
                        heldItems--;
                    }
                }
                else if (outputOrientation == ConveyorBelt.OutputOrientation.North)
                {
                    if (leftOutput.TryInsertVertical(items[0], this.transform.position.y + 0.51f))
                    {
                        Vector3 temp = this.transform.position;
                        temp.y += 0.51f;
                        temp.x -= 0.5f;
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
                    if (leftOutput.TryInsertVertical(items[0], this.transform.position.y - 0.51f))
                    {
                        Vector3 temp = this.transform.position;
                        temp.y -= 0.51f;
                        temp.x -= 0.5f;
                        items[0].transform.position = temp;
                        for (int j = 0; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[items.Length - 1] = null;
                        heldItems--;
                    }
                }
                modFrame++;
            }
            else if (
                (
                ((modFrame == 1 || leftOutput == null) && filterSide == -1) ||
                ((filterSide == 1 && filter == items[0].item.id) || (filterSide == 0 && filter != items[0].item.id))
                ) &&
                rightOutput != null)
            {
                if (outputOrientation == ConveyorBelt.OutputOrientation.East)
                {
                    if (rightOutput.TryInsertHorizontal(items[0], this.transform.position.x + 0.51f))
                    {
                        Vector3 temp = this.transform.position;
                        temp.x += 0.51f;
                        temp.y -= 0.5f;
                        items[0].transform.position = temp;
                        for (int j = 0; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[items.Length - 1] = null;
                        heldItems--;
                    }
                }
                else if (outputOrientation == ConveyorBelt.OutputOrientation.West)
                {
                    if (rightOutput.TryInsertHorizontal(items[0], this.transform.position.x - 0.51f))
                    {
                        Vector3 temp = this.transform.position;
                        temp.x -= 0.51f;
                        temp.y -= 0.5f;
                        items[0].transform.position = temp;
                        for (int j = 0; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[items.Length - 1] = null;
                        heldItems--;
                    }
                }
                else if (outputOrientation == ConveyorBelt.OutputOrientation.North)
                {
                    if (rightOutput.TryInsertVertical(items[0], this.transform.position.y + 0.51f))
                    {
                        Vector3 temp = this.transform.position;
                        temp.y += 0.51f;
                        temp.x += 0.5f;
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
                    if (rightOutput.TryInsertVertical(items[0], this.transform.position.y - 0.51f))
                    {
                        Vector3 temp = this.transform.position;
                        temp.y -= 0.51f;
                        temp.x += 0.5f;
                        items[0].transform.position = temp;
                        for (int j = 0; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[items.Length - 1] = null;
                        heldItems--;
                    }
                }
                modFrame++;
            }
        }
    }

    public bool TryInsertHorizontal(ItemEntity item, float x)
    {
        if(heldItems < items.Length)
        {
            item.transform.position = this.transform.position;
            items[heldItems] = item;
            heldItems++;
            return true;
        }
        return false;
    }

    public bool TryInsertVertical(ItemEntity item, float y)
    {
        if(heldItems < items.Length)
        {
            item.transform.position = this.transform.position;
            items[heldItems] = item;
            heldItems++;
            return true;
        }
        return false;
    }

    public bool CanConnectToBeltElement(IBeltElement prevElement)
    {
        return prevElement.GetOutputOrientation() == outputOrientation;
    }

    public bool CheckCollisionHorizontal(float x)
    {
        return false;
    }

    public bool CheckCollisionVertical(float y)
    {
        return false;
    }

    public ConveyorBelt.OutputOrientation GetOutputOrientation()
    {
        return outputOrientation;
    }

    public Vector2 GetRotatedBoundingBox(int rotation)
    {
        rotation = rotation % 4;
        if(rotation == 0 || rotation == 2)
        {
            return new Vector2(1, 2);
        }
        else
        {
            return new Vector2(2, 1);
        }
    }

    public Sprite GetRotatedPlaceSprite(int rotation)
    {
        rotation = rotation % 4;
        return AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotatedSpritePropertyId].sprites[rotation];
    }

    /// <summary>
    /// TODO: handle case where position is == (a splitter leading into a splitter)
    /// </summary>
    /// <param name="rotation"></param>
    public void InitializeRotated(int rotation)
    {
        rotation = rotation % 4;
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = 1 << LayerMask.NameToLayer("ConveyorBelt");
        Collider2D[] resultsTopAndLeft = new Collider2D[3];
        Collider2D[] resultsBottomAndRight = new Collider2D[3];
        int countTop;
        int countBottom;
        int countLeft;
        int countRight;
        items = new ItemEntity[maxBufferCapacity];

        switch(rotation)
        {
            case 0:
                outputOrientation = ConveyorBelt.OutputOrientation.East;
                GetComponent<BoxCollider2D>().size = new Vector2(1, 2);
                //rundown: drop overlapbox for left and right
                //if a belt is added later, setnextbelt and setprevbelt will take care of properly assigning belts
                countTop = Physics2D.OverlapBox(this.transform.position + new Vector3(0, 0.5f), new Vector2(2, 0.5f), 0, filter, resultsTopAndLeft);
                countBottom = Physics2D.OverlapBox(this.transform.position + new Vector3(0, -0.5f), new Vector2(2, 0.5f), 0, filter, resultsBottomAndRight);
                for(int i = 0; i < countTop; i++)
                {
                    if (resultsTopAndLeft[i].gameObject == this.gameObject) continue;
                    if (resultsTopAndLeft[i].transform.position.x < this.transform.position.x)
                    {
                        IBeltElement temp = resultsTopAndLeft[i].GetComponent<IBeltElement>();
                        if(temp.GetOutputOrientation() == this.outputOrientation)
                        {
                            temp.SetNextBeltElement(this);
                            leftInput = temp;
                        }
                    }
                    else if (resultsTopAndLeft[i].transform.position.x > this.transform.position.x)
                    {
                        IBeltElement temp = resultsTopAndLeft[i].GetComponent<IBeltElement>();
                        if(temp.CanConnectToBeltElement(this))
                        {
                            temp.SetPrevBeltElement(this);
                            leftOutput = temp;
                        }
                    }
                }

                for (int i = 0; i < countBottom; i++)
                {
                    if (resultsBottomAndRight[i].gameObject == this.gameObject) continue;
                    if (resultsBottomAndRight[i].transform.position.x < this.transform.position.x)
                    {
                        IBeltElement temp = resultsBottomAndRight[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == this.outputOrientation)
                        {
                            temp.SetNextBeltElement(this);
                            rightInput = temp;
                        }
                    }
                    else if (resultsBottomAndRight[i].transform.position.x > this.transform.position.x)
                    {
                        IBeltElement temp = resultsBottomAndRight[i].GetComponent<IBeltElement>();
                        if (temp.CanConnectToBeltElement(this))
                        {
                            temp.SetPrevBeltElement(this);
                            rightOutput = temp;
                        }
                    }
                }

                break;
            case 1:
                outputOrientation = ConveyorBelt.OutputOrientation.North;
                GetComponent<BoxCollider2D>().size = new Vector2(2, 1);

                countLeft = Physics2D.OverlapBox(this.transform.position + new Vector3(-0.5f, 0), new Vector2(0.5f, 2), 0, filter, resultsTopAndLeft);
                countRight = Physics2D.OverlapBox(this.transform.position + new Vector3(0.5f, 0), new Vector2(0.5f, 2), 0, filter, resultsBottomAndRight);
                for (int i = 0; i < countLeft; i++)
                {
                    if (resultsTopAndLeft[i].gameObject == this.gameObject) continue;
                    if (resultsTopAndLeft[i].transform.position.y < this.transform.position.y)
                    {
                        IBeltElement temp = resultsTopAndLeft[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == this.outputOrientation)
                        {
                            temp.SetNextBeltElement(this);
                            leftInput = temp;
                        }
                    }
                    else if (resultsTopAndLeft[i].transform.position.y > this.transform.position.y)
                    {
                        IBeltElement temp = resultsTopAndLeft[i].GetComponent<IBeltElement>();
                        if (temp.CanConnectToBeltElement(this))
                        {
                            temp.SetPrevBeltElement(this);
                            leftOutput = temp;
                        }
                    }
                }

                for (int i = 0; i < countRight; i++)
                {
                    if (resultsBottomAndRight[i].gameObject == this.gameObject) continue;
                    if (resultsBottomAndRight[i].transform.position.y < this.transform.position.y)
                    {
                        IBeltElement temp = resultsBottomAndRight[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == this.outputOrientation)
                        {
                            temp.SetNextBeltElement(this);
                            rightInput = temp;
                        }
                    }
                    else if (resultsBottomAndRight[i].transform.position.y > this.transform.position.y)
                    {
                        IBeltElement temp = resultsBottomAndRight[i].GetComponent<IBeltElement>();
                        if (temp.CanConnectToBeltElement(this))
                        {
                            temp.SetPrevBeltElement(this);
                            rightOutput = temp;
                        }
                    }
                }

                break;
            case 2:
                outputOrientation = ConveyorBelt.OutputOrientation.West;
                GetComponent<BoxCollider2D>().size = new Vector2(1, 2);
                countTop = Physics2D.OverlapBox(this.transform.position + new Vector3(0, 0.5f), new Vector2(2, 0.5f), 0, filter, resultsTopAndLeft);
                countBottom = Physics2D.OverlapBox(this.transform.position + new Vector3(0, -0.5f), new Vector2(2, 0.5f), 0, filter, resultsBottomAndRight);
                for (int i = 0; i < countTop; i++)
                {
                    if (resultsTopAndLeft[i].gameObject == this.gameObject) continue;
                    if (resultsTopAndLeft[i].transform.position.x > this.transform.position.x)
                    {
                        IBeltElement temp = resultsTopAndLeft[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == this.outputOrientation)
                        {
                            temp.SetNextBeltElement(this);
                            leftInput = temp;
                        }
                    }
                    else if (resultsTopAndLeft[i].transform.position.x < this.transform.position.x)
                    {
                        IBeltElement temp = resultsTopAndLeft[i].GetComponent<IBeltElement>();
                        if (temp.CanConnectToBeltElement(this))
                        {
                            temp.SetPrevBeltElement(this);
                            leftOutput = temp;
                        }
                    }
                }

                for (int i = 0; i < countBottom; i++)
                {
                    if (resultsBottomAndRight[i].gameObject == this.gameObject) continue;
                    if (resultsBottomAndRight[i].transform.position.x > this.transform.position.x)
                    {
                        IBeltElement temp = resultsBottomAndRight[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == this.outputOrientation)
                        {
                            temp.SetNextBeltElement(this);
                            rightInput = temp;
                        }
                    }
                    else if (resultsBottomAndRight[i].transform.position.x < this.transform.position.x)
                    {
                        IBeltElement temp = resultsBottomAndRight[i].GetComponent<IBeltElement>();
                        if (temp.CanConnectToBeltElement(this))
                        {
                            temp.SetPrevBeltElement(this);
                            rightOutput = temp;
                        }
                    }
                }

                break;
            default:
            case 3:
                outputOrientation = ConveyorBelt.OutputOrientation.South;
                GetComponent<BoxCollider2D>().size = new Vector2(2, 1);

                countLeft = Physics2D.OverlapBox(this.transform.position + new Vector3(-0.5f, 0), new Vector2(0.5f, 2), 0, filter, resultsTopAndLeft);
                countRight = Physics2D.OverlapBox(this.transform.position + new Vector3(0.5f, 0), new Vector2(0.5f, 2), 0, filter, resultsBottomAndRight);
                for (int i = 0; i < countLeft; i++)
                {
                    if (resultsTopAndLeft[i].gameObject == this.gameObject) continue;
                    if (resultsTopAndLeft[i].transform.position.y > this.transform.position.y)
                    {
                        IBeltElement temp = resultsTopAndLeft[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == this.outputOrientation)
                        {
                            temp.SetNextBeltElement(this);
                            leftInput = temp;
                        }
                    }
                    else if (resultsTopAndLeft[i].transform.position.y < this.transform.position.y)
                    {
                        IBeltElement temp = resultsTopAndLeft[i].GetComponent<IBeltElement>();
                        if (temp.CanConnectToBeltElement(this))
                        {
                            temp.SetPrevBeltElement(this);
                            leftOutput = temp;
                        }
                    }
                }

                for (int i = 0; i < countRight; i++)
                {
                    if (resultsBottomAndRight[i].gameObject == this.gameObject) continue;
                    if (resultsBottomAndRight[i].transform.position.y > this.transform.position.y)
                    {
                        IBeltElement temp = resultsBottomAndRight[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == this.outputOrientation)
                        {
                            temp.SetNextBeltElement(this);
                            rightInput = temp;
                        }
                    }
                    else if (resultsBottomAndRight[i].transform.position.y < this.transform.position.y)
                    {
                        IBeltElement temp = resultsBottomAndRight[i].GetComponent<IBeltElement>();
                        if (temp.CanConnectToBeltElement(this))
                        {
                            temp.SetPrevBeltElement(this);
                            rightOutput = temp;
                        }
                    }
                }

                break;
        }
        GetComponent<SpriteRenderer>().sprite = AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotatedSpritePropertyId].sprites[rotation];
    }

    /// <summary>
    /// Belts that
    /// </summary>
    /// <param name="beltElement"></param>
    public void SetNextBeltElement(IBeltElement beltElement)
    {
        Vector2 beltPos = beltElement.GetPosition();
        switch(outputOrientation)
        {
            case ConveyorBelt.OutputOrientation.East:
                if (beltElement.GetOutputOrientation() == ConveyorBelt.OutputOrientation.West) return;
                if (beltPos.y > this.transform.position.y)
                {
                    leftOutput = beltElement;
                }
                else if (beltPos.y < this.transform.position.y)
                {
                    rightOutput = beltElement;
                }
                else if (beltPos.y == this.transform.position.y)
                {
                    leftOutput = beltElement;
                    rightOutput = beltElement;
                }
                break;
            case ConveyorBelt.OutputOrientation.North:
                if (beltElement.GetOutputOrientation() == ConveyorBelt.OutputOrientation.South) return;
                if (beltPos.x < this.transform.position.x)
                {
                    leftOutput = beltElement;
                }
                else if (beltPos.x > this.transform.position.x)
                {
                    rightOutput = beltElement;
                }
                else if (beltPos.x == this.transform.position.x)
                {
                    leftOutput = beltElement;
                    rightOutput = beltElement;
                }
                break;
            case ConveyorBelt.OutputOrientation.West:
                if (beltElement.GetOutputOrientation() == ConveyorBelt.OutputOrientation.East) return;
                if (beltPos.y > this.transform.position.y)
                {
                    leftOutput = beltElement;
                }
                else if (beltPos.y < this.transform.position.y)
                {
                    rightOutput = beltElement;
                }
                else if (beltPos.y == this.transform.position.y)
                {
                    leftOutput = beltElement;
                    rightOutput = beltElement;
                }
                break;
            default:
            case ConveyorBelt.OutputOrientation.South:
                if (beltElement.GetOutputOrientation() == ConveyorBelt.OutputOrientation.North) return;
                if (beltPos.x < this.transform.position.x)
                {
                    leftOutput = beltElement;
                }
                else if (beltPos.x > this.transform.position.x)
                {
                    rightOutput = beltElement;
                }
                else if (beltPos.x == this.transform.position.x)
                {
                    rightOutput = beltElement;
                    leftOutput = beltElement;
                }
                break;
        }
    }

    /// <summary>
    /// Belts that input to splitter
    /// </summary>
    /// <param name="beltElement"></param>
    public void SetPrevBeltElement(IBeltElement beltElement)
    {
        if (beltElement.GetOutputOrientation() != this.outputOrientation) return;
        Vector2 beltPos = beltElement.GetPosition();
        switch (outputOrientation)
        {
            case ConveyorBelt.OutputOrientation.East:
                if (beltPos.y > this.transform.position.y)
                {
                    leftInput = beltElement;
                }
                else if (beltPos.y < this.transform.position.y)
                {
                    rightInput = beltElement;
                }
                break;
            case ConveyorBelt.OutputOrientation.North:
                if (beltPos.x < this.transform.position.x)
                {
                    leftInput = beltElement;
                }
                else if (beltPos.x > this.transform.position.x)
                {
                    rightInput = beltElement;
                }
                break;
            case ConveyorBelt.OutputOrientation.West:
                if (beltPos.y > this.transform.position.y)
                {
                    leftInput = beltElement;
                }
                else if (beltPos.y < this.transform.position.y)
                {
                    rightInput = beltElement;
                }
                break;
            default:
            case ConveyorBelt.OutputOrientation.South:
                if (beltPos.x < this.transform.position.x)
                {
                    leftInput = beltElement;
                }
                else if (beltPos.x > this.transform.position.x)
                {
                    rightInput = beltElement;
                }
                break;
        }
    }


    public Vector2 GetPosition()
    {
        return this.transform.position;
    }

    public int GetFilterItem()
    {
        return filter;
    }

    public string GetFilter()
    {
        if(filterSide == 0)
        {
            return "Left";
        }
        else if(filterSide == 1)
        {
            return "Right";
        }
        else
        {
            return "None";
        }
    }

    public void SetFilter(string filterName, int itemId)
    {
        if(itemId == -1 || filterName == "None")
        {
            filterSide = -1;
            filter = -1;
            return;
        }
        if(filterName == "Left")
        {
            filterSide = 0;
            filter = itemId;
        }
        else if(filterName == "Right")
        {
            filterSide = 1;
            filter = itemId;
        }
    }

    public string[] GetFilterNames()
    {
        return filterNames;
    }
}
