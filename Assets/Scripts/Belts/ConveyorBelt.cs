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
    [SerializeField] float beltSpacing = 0.25f;
    Rotation rotationState;
    OutputOrientation outputOrientation;
    [SerializeField] int maxBeltCapacity = 4;

    /// <summary>
    /// I'm sure someone's done it better.  Don't think about it too hard.
    /// </summary>
    public void FixedUpdate()
    {
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
                
                if (nextBelt == null && velocityMultiplier * (temp.y - this.transform.position.y) >= beltSpacing)
                {
                    temp.y = this.transform.position.y + velocityMultiplier * beltSpacing;
                }
                else if (Mathf.Abs(temp.y - this.transform.position.y) > 0.5f) //will do nothing if item can't be inserted to next belt
                {
                    if (nextBelt.GetOutputOrientation() != this.outputOrientation && nextBelt.TryInsertHorizontal(items[i], temp.x))
                    {
                        items[i].transform.position = temp;
                        for (int j = i; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[items.Length - 1] = null;
                        heldItems--;
                    }
                    if (nextBelt.TryInsertVertical(items[i], temp.y))
                    {
                        items[i].transform.position = temp;
                        for (int j = i; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[items.Length - 1] = null;
                        heldItems--;
                    }
                }
                else if ((i == 0 && (nextBelt == null || !nextBelt.CheckCollisionVertical(temp.y))) ||
                    (i - 1 >= 0 && Mathf.Abs(temp.y - items[i - 1].transform.position.y) >= beltSpacing)) //moves item forward if it isn't outside the belt (need to check against items in front)
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

                if (nextBelt == null && velocityMultiplier * (temp.x - this.transform.position.x) >= beltSpacing)
                {
                    temp.x = this.transform.position.x + velocityMultiplier * beltSpacing;
                }
                else if (Mathf.Abs(temp.x - this.transform.position.x) > 0.5f) //will do nothing if item can't be inserted to next belt
                {
                    if (nextBelt.GetOutputOrientation() != this.outputOrientation && nextBelt.TryInsertVertical(items[i], temp.y))
                    {
                        items[i].transform.position = temp;
                        for (int j = i; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[items.Length - 1] = null;
                        heldItems--;
                    }
                    if (nextBelt.TryInsertHorizontal(items[i], temp.x))
                    {
                        items[i].transform.position = temp;
                        for (int j = i; j < items.Length - 1; j++)
                        {
                            items[j] = items[j + 1];
                        }
                        items[items.Length - 1] = null;
                        heldItems--;
                    }
                }
                else if ((i == 0 && (nextBelt == null || !nextBelt.CheckCollisionHorizontal(temp.x))) ||
                    (i - 1 >= 0 && Mathf.Abs(temp.x - items[i - 1].transform.position.x) >= beltSpacing)) //moves item forward if it isn't outside the belt (need to check against items in front)
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
    public bool TryInsertHorizontal(ItemEntity item, float x)
    {
        if (bufferedInput != null || heldItems != 0 && (heldItems == maxBeltCapacity || Mathf.Abs(x - items[heldItems - 1].transform.position.x) < beltSpacing))
        {
            return false;
        }
        bufferedInput = item;
        return true;
    }

    public bool TryInsertVertical(ItemEntity item, float y)
    {
        if (bufferedInput != null || heldItems != 0 && (heldItems == maxBeltCapacity || Mathf.Abs(y - items[heldItems - 1].transform.position.y) < beltSpacing))
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
    public bool CheckCollisionHorizontal(float x)
    {
        if (heldItems == 0)
        {
            return false;
        }
        if (Mathf.Abs(x - items[heldItems - 1].transform.position.x) < beltSpacing)
        {
            return true;
        }
        return false;
    }

    public bool CheckCollisionVertical(float y)
    {
        if (heldItems == 0)
        {
            return false;
        }
        if (Mathf.Abs(y - items[heldItems - 1].transform.position.y) < beltSpacing)
        {
            return true;
        }
        return false;
    }

    public Sprite GetRotatedPlaceSprite(int rotation)
    {
        rotation = rotation % 4;
        return AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotatedSpritePropertyId].sprites[rotation];
    }

    public void InitializeRotated(int rotation)
    {
        rotation = rotation % 4;
        Collider2D[] resultsX = new Collider2D[3];
        Collider2D[] resultsY = new Collider2D[3];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = 1 << LayerMask.NameToLayer("ConveyorBelt");
        int countX = Physics2D.OverlapBox(this.transform.position, new Vector2(2, 0.25f), 0, filter, resultsX);
        int countY = Physics2D.OverlapBox(this.transform.position, new Vector2(0.25f, 2), 0, filter, resultsY);
        items = new ItemEntity[maxBeltCapacity];
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
                        if (temp.CanConnectToBeltElement(this))
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
                        if (temp.CanConnectToBeltElement(this))
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
                        if (temp.CanConnectToBeltElement(this))
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
                        if (temp.CanConnectToBeltElement(this))
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
        this.GetComponent<SpriteRenderer>().sprite = AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotatedSpritePropertyId].sprites[rotation];
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

    public bool CanConnectToBeltElement(IBeltElement prevElement)
    {
        OutputOrientation orientationPrev = prevElement.GetOutputOrientation();
        if(orientationPrev == OutputOrientation.East)
        {
            return outputOrientation != OutputOrientation.West;
        }
        else if(orientationPrev == OutputOrientation.North)
        {
            return outputOrientation != OutputOrientation.South;
        }
        else if(orientationPrev == OutputOrientation.West)
        {
            return outputOrientation != OutputOrientation.East;
        }
        else if(orientationPrev == OutputOrientation.South)
        {
            return outputOrientation != OutputOrientation.North;
        }
        return false;
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
}
