using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour, IBeltElement, IRotate
{
    enum Rotation
    {
        Horizontal,
        Vertical
    }
    public enum OutputOrientation
    {
        North,
        South,
        East,
        West
    }
    IBeltElement nextBelt;
    IBeltElement prevBelt; //if a belt is aligned to this one then it is assigned here
    IBeltElement prevBeltNorthOrEast; //if the belt is horizontal then this holds belt to the north
    IBeltElement prevBeltSouthOrWest; //if the belt is horizontal then this holds belt to the south
    ItemEntity[] items;
    ItemEntity bufferedInput;
    [SerializeField] float conveyorBeltSpeed = 0.05f;
    [SerializeField] int rotatedSpritePropertyId = 0;
    int velocityMultiplier = 1;
    int heldItems = 0;
    Rotation rotationState;
    OutputOrientation outputOrientation;
    int lastFrameUpdate;

    /// <summary>
    /// I'm sure someone's done it better.  Don't think about it too hard.
    /// </summary>
    public void FixedUpdate()
    {
        lastFrameUpdate = Time.frameCount;
        if (rotationState == Rotation.Vertical)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    break;
                }
                Vector3 temp = items[i].transform.position;
                float deltaX = temp.x - this.transform.position.x;
                if (deltaX > 2 * conveyorBeltSpeed)
                {
                    temp.x -= 2 * conveyorBeltSpeed;
                }
                else if (deltaX < -2 * conveyorBeltSpeed)
                {
                    temp.x += 2 * conveyorBeltSpeed;
                }
                else
                {
                    temp.x = this.transform.position.x;
                }
                temp.y += conveyorBeltSpeed * velocityMultiplier;
                
                if (nextBelt == null && velocityMultiplier * (temp.y - this.transform.position.y) >= 0.325f)
                {
                    temp.y = this.transform.position.y + velocityMultiplier * 0.325f;
                }
                else if (Mathf.Abs(temp.y - this.transform.position.y) > 0.5f) //will do nothing if item can't be inserted to next belt
                {
                    if (nextBelt.GetOutputOrientation() != this.outputOrientation && nextBelt.TryInsertHorizontal(items[i], temp.x, lastFrameUpdate))
                    {
                        for (int j = i; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[2] = null;
                        heldItems--;
                    }
                    if (nextBelt.TryInsertVertical(items[i], temp.y, lastFrameUpdate))
                    {
                        for (int j = i; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[2] = null;
                        heldItems--;
                    }
                }
                else if ((i == 0 && (nextBelt == null || !nextBelt.CheckCollisionVertical(temp.y, lastFrameUpdate))) ||
                    (i - 1 >= 0 && Mathf.Abs(temp.y - items[i - 1].transform.position.y) >= 0.325f)) //moves item forward if it isn't outside the belt (need to check against items in front)
                {
                    items[i].transform.position = temp;
                }
            }
        }
        else
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    break;
                }
                Vector3 temp = items[i].transform.position;
                float deltaY = temp.y - this.transform.position.y;
                if (deltaY > 2 * conveyorBeltSpeed)
                {
                    temp.y -= 2 * conveyorBeltSpeed;
                }
                else if (deltaY < -2 * conveyorBeltSpeed)
                {
                    temp.y += 2 * conveyorBeltSpeed;
                }
                else
                {
                    temp.y = this.transform.position.y;
                }
                temp.x += conveyorBeltSpeed * velocityMultiplier;

                if (nextBelt == null && velocityMultiplier * (temp.x - this.transform.position.x) >= 0.325f)
                {
                    temp.x = this.transform.position.x + velocityMultiplier * 0.325f;
                }
                else if (Mathf.Abs(temp.x - this.transform.position.x) > 0.5f) //will do nothing if item can't be inserted to next belt
                {
                    if (nextBelt.GetOutputOrientation() != this.outputOrientation && nextBelt.TryInsertVertical(items[i], temp.y, lastFrameUpdate))
                    {
                        for (int j = i; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[2] = null;
                        heldItems--;
                    }
                    if (nextBelt.TryInsertHorizontal(items[i], temp.x, lastFrameUpdate))
                    {
                        for (int j = i; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[2] = null;
                        heldItems--;
                    }
                }
                else if ((i == 0 && (nextBelt == null || !nextBelt.CheckCollisionHorizontal(temp.x, lastFrameUpdate))) ||
                    (i - 1 >= 0 && Mathf.Abs(temp.x - items[i - 1].transform.position.x) >= 0.325f)) //moves item forward if it isn't outside the belt (need to check against items in front)
                {
                    items[i].transform.position = temp;
                }
            }
        }
        if(bufferedInput != null)
        {
            items[heldItems] = bufferedInput;
            heldItems++;
            bufferedInput = null;
        }
    }

    /// <summary>
    /// Tries to insert an item to the next belt
    /// </summary>
    /// <param name="item"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool TryInsertHorizontal(ItemEntity item, float x, int callerFrame)
    {
        float timeFactor = conveyorBeltSpeed * velocityMultiplier * (callerFrame - lastFrameUpdate);
        if(nextBelt == null)
        {
            timeFactor = 0;
        }
        if (bufferedInput != null || heldItems != 0 && (heldItems == 3 || Mathf.Abs(x - items[heldItems - 1].transform.position.x - timeFactor) < 0.325f))
        {
            return false;
        }
        bufferedInput = item;
        return true;
    }

    public bool TryInsertVertical(ItemEntity item, float y, int callerFrame)
    {
        float timeFactor = conveyorBeltSpeed * velocityMultiplier * (callerFrame - lastFrameUpdate);
        if(nextBelt == null)
        {
            timeFactor = 0;
        }
        if (bufferedInput != null || heldItems != 0 && (heldItems == 3 || Mathf.Abs(y - items[heldItems - 1].transform.position.y - timeFactor) < 0.325f))
        {
            return false;
        }
        bufferedInput = item;
        return true;
    }

    /// <summary>
    /// Check for inter-belt collision
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CheckCollisionHorizontal(float x, int callerFrame)
    {
        if (heldItems == 0)
        {
            return false;
        }
        float timeFactor = conveyorBeltSpeed * velocityMultiplier * (callerFrame - lastFrameUpdate);
        if(nextBelt == null)
        {
            timeFactor = 0;
        }
        if (Mathf.Abs(x - items[heldItems - 1].transform.position.x - timeFactor) < 0.325f)
        {
            return true;
        }
        return false;
    }

    public bool CheckCollisionVertical(float y, int callerFrame)
    {
        if (heldItems == 0)
        {
            return false;
        }
        float timeFactor = conveyorBeltSpeed * velocityMultiplier * (callerFrame - lastFrameUpdate);
        if(nextBelt == null)
        {
            timeFactor = 0;
        }
        if (Mathf.Abs(y - items[heldItems - 1].transform.position.y - timeFactor) < 0.325f)
        {
            return true;
        }
        return false;
    }

    public Sprite GetRotatedPlaceSprite(int rotation)
    {
        return AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotatedSpritePropertyId].sprites[rotation];
    }

    public void InitializeRotated(int rotation)
    {
        Collider2D[] resultsX = new Collider2D[3];
        Collider2D[] resultsY = new Collider2D[3];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = 1 << LayerMask.NameToLayer("ConveyorBelt");
        int countX = Physics2D.OverlapBox(this.transform.position, new Vector2(2, 0.25f), 0, filter, resultsX);
        int countY = Physics2D.OverlapBox(this.transform.position, new Vector2(0.25f, 2), 0, filter, resultsY);
        items = new ItemEntity[3];
        switch(rotation)
        {
            case 0:
                #region Horizontal ouput east
                rotationState = Rotation.Horizontal;
                outputOrientation = OutputOrientation.East;

                for (int i = 0; i < countX; i++)
                {
                    if (resultsX[i].gameObject == this.gameObject) continue;
                    if (resultsX[i].transform.position.x < this.transform.position.x)
                    {
                        IBeltElement temp = resultsX[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.East)
                        {
                            prevBelt = temp;
                            prevBelt.SetNextBeltElement(this);
                        }
                    }
                    else if (resultsX[i].transform.position.x > this.transform.position.x)
                    {
                        IBeltElement temp = resultsX[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() != OutputOrientation.West)
                        {
                            nextBelt = temp;
                            nextBelt.SetPrevBeltElement(this);
                        }
                    }
                }

                for (int i = 0; i < countY; i++)
                {
                    if (resultsY[i].gameObject == this.gameObject) continue;
                    if (resultsY[i].transform.position.y > this.transform.position.y)
                    {
                        IBeltElement temp = resultsY[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.South)
                        {
                            prevBeltNorthOrEast = temp;
                            prevBeltNorthOrEast.SetNextBeltElement(this);
                        }
                    }
                    else if (resultsY[i].transform.position.y < this.transform.position.y)
                    {
                        IBeltElement temp = resultsY[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.North)
                        {
                            prevBeltSouthOrWest = temp;
                            prevBeltSouthOrWest.SetNextBeltElement(this);
                        }
                    }
                }

                this.GetComponent<SpriteRenderer>().sprite = AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotatedSpritePropertyId].sprites[rotation];
                #endregion
                break;
            case 1:
                #region Vertical output north
                rotationState = Rotation.Vertical;
                outputOrientation = OutputOrientation.North;

                for (int i = 0; i < countY; i++)
                {
                    if (resultsY[i].gameObject == this.gameObject) continue;
                    if (resultsY[i].transform.position.y < this.transform.position.y)
                    {
                        IBeltElement temp = resultsY[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.North)
                        {
                            prevBelt = temp;
                            prevBelt.SetNextBeltElement(this);
                        }
                    }
                    else if (resultsY[i].transform.position.y > this.transform.position.y)
                    {
                        IBeltElement temp = resultsY[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() != OutputOrientation.South)
                        {
                            nextBelt = resultsY[i].GetComponent<IBeltElement>();
                            nextBelt.SetPrevBeltElement(this);
                        }
                    }
                }

                for (int i = 0; i < countX; i++)
                {
                    if (resultsX[i].gameObject == this.gameObject) continue;
                    if (resultsX[i].transform.position.x > this.transform.position.x)
                    {
                        IBeltElement temp = resultsX[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.West)
                        {
                            prevBeltNorthOrEast = temp;
                            prevBeltNorthOrEast.SetNextBeltElement(this);
                        }
                    }
                    else if (resultsX[i].transform.position.x < this.transform.position.x)
                    {
                        IBeltElement temp = resultsX[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.East)
                        {
                            prevBeltSouthOrWest = temp;
                            prevBeltSouthOrWest.SetNextBeltElement(this);
                        }
                    }
                }

                this.GetComponent<SpriteRenderer>().sprite = AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotatedSpritePropertyId].sprites[rotation];
                #endregion
                break;
            case 2:
                #region Horizontal output west
                rotationState = Rotation.Horizontal;
                outputOrientation = OutputOrientation.West;
                velocityMultiplier = -1;

                for (int i = 0; i < countX; i++)
                {
                    if (resultsX[i].gameObject == this.gameObject) continue;
                    if (resultsX[i].transform.position.x > this.transform.position.x)
                    {
                        IBeltElement temp = resultsX[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.West)
                        {
                            prevBelt = temp;
                            prevBelt.SetNextBeltElement(this);
                        }
                    }
                    else if (resultsX[i].transform.position.x < this.transform.position.x)
                    {
                        IBeltElement temp = resultsX[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() != OutputOrientation.East)
                        {
                            nextBelt = temp;
                            nextBelt.SetPrevBeltElement(this);
                        }
                    }
                }

                for (int i = 0; i < countY; i++)
                {
                    if (resultsY[i].gameObject == this.gameObject) continue;
                    if (resultsY[i].transform.position.y > this.transform.position.y)
                    {
                        IBeltElement temp = resultsY[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.South)
                        {
                            prevBeltNorthOrEast = temp;
                            prevBeltNorthOrEast.SetNextBeltElement(this);
                        }
                    }
                    else if (resultsY[i].transform.position.y < this.transform.position.y)
                    {
                        IBeltElement temp = resultsY[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.North)
                        {
                            prevBeltSouthOrWest = temp;
                            prevBeltSouthOrWest.SetNextBeltElement(this);
                        }
                    }
                }

                this.GetComponent<SpriteRenderer>().sprite = AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotatedSpritePropertyId].sprites[rotation];
                #endregion
                break;
            case 3:
            default:
                #region Vertical output south
                velocityMultiplier = -1;
                rotationState = Rotation.Vertical;
                outputOrientation = OutputOrientation.South;

                for (int i = 0; i < countY; i++)
                {
                    if (resultsY[i].gameObject == this.gameObject) continue;
                    if (resultsY[i].transform.position.y > this.transform.position.y)
                    {
                        IBeltElement temp = resultsY[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.South)
                        {
                            prevBelt = temp;
                            prevBelt.SetNextBeltElement(this);
                        }
                    }
                    else if (resultsY[i].transform.position.y < this.transform.position.y)
                    {
                        IBeltElement temp = resultsY[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() != OutputOrientation.North)
                        {
                            nextBelt = temp;
                            nextBelt.SetPrevBeltElement(this);
                        }
                    }
                }

                for (int i = 0; i < countX; i++)
                {
                    if (resultsX[i].gameObject == this.gameObject) continue;
                    if (resultsX[i].transform.position.x > this.transform.position.x)
                    {
                        IBeltElement temp = resultsX[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.West)
                        {
                            prevBeltNorthOrEast = temp;
                            prevBeltNorthOrEast.SetNextBeltElement(this);
                        }
                    }
                    else if (resultsX[i].transform.position.x < this.transform.position.x)
                    {
                        IBeltElement temp = resultsX[i].GetComponent<IBeltElement>();
                        if (temp.GetOutputOrientation() == OutputOrientation.East)
                        {
                            prevBeltSouthOrWest = temp;
                            prevBeltSouthOrWest.SetNextBeltElement(this);
                        }
                    }
                }

                #endregion
                break;
        } 
        items[0] = ItemEntityPool.Instance.CreateItemEntity(this.transform.position, Definitions.Instance.ItemDictionary[0]);
        heldItems++;
    }

    public void SetNextBeltElement(IBeltElement beltElement)
    {
        this.nextBelt = beltElement;
    }

    public void SetPrevBeltElement(IBeltElement beltElement)
    {
        this.prevBelt = beltElement;
    }

    public OutputOrientation GetOutputOrientation()
    {
        return outputOrientation;
    }
}
