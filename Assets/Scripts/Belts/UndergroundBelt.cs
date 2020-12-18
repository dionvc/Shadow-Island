using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundBelt : MonoBehaviour, IRotate, IBeltElement
{

    //Thinking 8 variants with an in and out for each direction
    //Linking to a variant will be somewhat restricted where inputs will only be able to link if they're in the same direction
    //Output will be rather straightforward where any belt not pointing in to output can be linked
    //If its an input to ground then it will search x number of blocks ahead for output in the same direction
    //If its an output from ground then it will search x number of blocks behind for an input in the same direction
    //if its an input to ground then only an output to ground can call setnextbelt
    //if its an output from ground then only an input to ground can call setbrevbelt (can connect should check if caller is under
    enum TransitionGround
    {
        input,
        output
    }

    IBeltElement output;
    IBeltElement input;
    ConveyorBelt.OutputOrientation outputOrientation;
    TransitionGround transitionGround;
    [SerializeField] int rotatedSpritePropertyId;
    [SerializeField] float beltSpeed;
    ItemEntity[] items;
    int heldItems = 0;
    int searchDist = 5;

    void FixedUpdate()
    {
        if (output != null && items[0] != null) //push into output belt
        {
            //push input output belt
            if (outputOrientation == ConveyorBelt.OutputOrientation.East)
            {
                if (output.TryInsertHorizontal(items[0], this.transform.position.x + 0.51f))
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
            else if (outputOrientation == ConveyorBelt.OutputOrientation.West)
            {
                if (output.TryInsertHorizontal(items[0], this.transform.position.x - 0.51f))
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
            else if (outputOrientation == ConveyorBelt.OutputOrientation.North)
            {
                if (output.TryInsertVertical(items[0], this.transform.position.y + 0.51f))
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
                if (output.TryInsertVertical(items[0], this.transform.position.y - 0.51f))
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

    public bool CanConnectToBeltElement(IBeltElement prevElement)
    {
        UndergroundBelt belt = prevElement as UndergroundBelt;
        if(transitionGround == TransitionGround.output && belt != null) 
        {
            return true;
        }
        else if(transitionGround == TransitionGround.input && prevElement.GetOutputOrientation() == this.outputOrientation)
        {
            return true;
        }
        return false;
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

    public Vector2 GetPosition()
    {
        return this.transform.position;
    }


    public Vector2 GetRotatedBoundingBox(int rotation)
    {
        return this.GetComponent<BoxCollider2D>().size;
    }

    public Sprite GetRotatedPlaceSprite(int rotation)
    {
        return AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotatedSpritePropertyId].sprites[rotation];
    }

    public void InitializeRotated(int rotation)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = 1 << LayerMask.NameToLayer("ConveyorBelt");
        int countRegular = 0;
        int countUnderground = 0;
        Collider2D[] regular = new Collider2D[1];
        Collider2D[] underground = new Collider2D[searchDist + 2];
        items = new ItemEntity[4];
        
        switch(rotation)
        {
            case 0: //east input
                outputOrientation = ConveyorBelt.OutputOrientation.East;
                transitionGround = TransitionGround.input;
                countRegular = Physics2D.OverlapBox((Vector2)this.transform.position - new Vector2(1, 0), new Vector2(0.5f, 0.5f), 0, filter, regular);
                countUnderground = Physics2D.OverlapBox((Vector2)this.transform.position + new Vector2(searchDist/2.0f, 0), new Vector2(searchDist, 0.5f), 0, filter, underground);
                if(countRegular >= 1)
                {
                    IBeltElement temp = regular[0].GetComponent<IBeltElement>();
                    if(temp.GetOutputOrientation() == outputOrientation)
                    {
                        input = temp;
                        temp.SetNextBeltElement(this);
                    }
                }

                for(int i = 0; i < countUnderground; i++)
                {
                    UndergroundBelt temp;
                    if(underground[i].TryGetComponent(out temp) && 
                        (output == null || output.GetPosition().x > temp.GetPosition().x) && //check if one found is closer than current connection
                        (temp.transitionGround == TransitionGround.output && temp.outputOrientation == outputOrientation)//check orientation and that it is output
                        )
                    {
                        output = temp;
                        temp.SetPrevBeltElement(this);
                    }
                }
                break;
            case 1: //east output
                outputOrientation = ConveyorBelt.OutputOrientation.East;
                transitionGround = TransitionGround.output;
                countRegular = Physics2D.OverlapBox((Vector2)this.transform.position + new Vector2(1, 0), new Vector2(0.5f, 0.5f), 0, filter, regular);
                countUnderground = Physics2D.OverlapBox((Vector2)this.transform.position - new Vector2(searchDist/2.0f, 0), new Vector2(searchDist, 0.5f), 0, filter, underground);
                
                if (countRegular >= 1)
                {
                    IBeltElement temp = regular[0].GetComponent<IBeltElement>();
                    if (temp.CanConnectToBeltElement(this))
                    {
                        output = temp;
                        temp.SetPrevBeltElement(this);
                    }
                }

                for (int i = 0; i < countUnderground; i++)
                {
                    UndergroundBelt temp;
                    if (underground[i].TryGetComponent(out temp) &&
                        (input == null || input.GetPosition().x < temp.GetPosition().x) && //check if one found is closer than current connection
                        (temp.transitionGround == TransitionGround.input && temp.outputOrientation == outputOrientation)//check orientation and that it is output
                        )
                    {
                        input = temp;
                        input.SetNextBeltElement(this);
                    }
                }
                break;
            case 2: //north input
                outputOrientation = ConveyorBelt.OutputOrientation.North;
                transitionGround = TransitionGround.input;
                countRegular = Physics2D.OverlapBox((Vector2)this.transform.position - new Vector2(0, 1), new Vector2(0.5f, 0.5f), 0, filter, regular);
                countUnderground = Physics2D.OverlapBox((Vector2)this.transform.position + new Vector2(0, searchDist/2.0f), new Vector2(0.5f, searchDist), 0, filter, underground);
                
                if (countRegular >= 1)
                {
                    IBeltElement temp = regular[0].GetComponent<IBeltElement>();
                    if (temp.GetOutputOrientation() == outputOrientation)
                    {
                        input = temp;
                        temp.SetNextBeltElement(this);
                    }
                }

                for (int i = 0; i < countUnderground; i++)
                {
                    UndergroundBelt temp;
                    if (underground[i].TryGetComponent(out temp) &&
                        (output == null || output.GetPosition().y > temp.GetPosition().y) && //check if one found is closer than current connection
                        (temp.transitionGround == TransitionGround.output && temp.outputOrientation == outputOrientation)//check orientation and that it is output
                        )
                    {
                        output = temp;
                        output.SetPrevBeltElement(this);
                    }
                }

                break;
            case 3: //north output
                outputOrientation = ConveyorBelt.OutputOrientation.North;
                transitionGround = TransitionGround.output;
                countRegular = Physics2D.OverlapBox((Vector2)this.transform.position + new Vector2(0, 1), new Vector2(0.5f, 0.5f), 0, filter, regular);
                countUnderground = Physics2D.OverlapBox((Vector2)this.transform.position - new Vector2(0, searchDist / 2.0f), new Vector2(0.5f, searchDist), 0, filter, underground);
                if (countRegular >= 1)
                {
                    IBeltElement temp = regular[0].GetComponent<IBeltElement>();
                    if (temp.CanConnectToBeltElement(this))
                    {
                        output = temp;
                        temp.SetPrevBeltElement(this);
                    }
                }

                for (int i = 0; i < countUnderground; i++)
                {
                    UndergroundBelt temp;
                    if (underground[i].TryGetComponent(out temp) &&
                        (input == null || input.GetPosition().y < temp.GetPosition().y) && //check if one found is closer than current connection
                        (temp.transitionGround == TransitionGround.input && temp.outputOrientation == outputOrientation)//check orientation and that it is output
                        )
                    {
                        input = temp;
                        input.SetNextBeltElement(this);
                    }
                }

                break;
            case 4: //west input
                outputOrientation = ConveyorBelt.OutputOrientation.West;
                transitionGround = TransitionGround.input;
                countRegular = Physics2D.OverlapBox((Vector2)this.transform.position + new Vector2(1, 0), new Vector2(0.5f, 0.5f), 0, filter, regular);
                countUnderground = Physics2D.OverlapBox((Vector2)this.transform.position - new Vector2(searchDist / 2.0f, 0), new Vector2(searchDist, 0.5f), 0, filter, underground);
                if (countRegular >= 1)
                {
                    IBeltElement temp = regular[0].GetComponent<IBeltElement>();
                    if (temp.GetOutputOrientation() == outputOrientation)
                    {
                        input = temp;
                        temp.SetNextBeltElement(this);
                    }
                }

                for (int i = 0; i < countUnderground; i++)
                {
                    UndergroundBelt temp;
                    if (underground[i].TryGetComponent(out temp) &&
                        (output == null || output.GetPosition().x < temp.GetPosition().x) && //check if one found is closer than current connection
                        (temp.transitionGround == TransitionGround.output && temp.outputOrientation == outputOrientation)//check orientation and that it is output
                        )
                    {
                        output = temp;
                        temp.SetPrevBeltElement(this);
                    }
                }
                break;
            case 5: //west output
                outputOrientation = ConveyorBelt.OutputOrientation.West;
                transitionGround = TransitionGround.output;
                countRegular = Physics2D.OverlapBox((Vector2)this.transform.position - new Vector2(1, 0), new Vector2(0.5f, 0.5f), 0, filter, regular);
                countUnderground = Physics2D.OverlapBox((Vector2)this.transform.position + new Vector2(searchDist / 2.0f, 0), new Vector2(searchDist, 0.5f), 0, filter, underground);
                
                if (countRegular >= 1)
                {
                    IBeltElement temp = regular[0].GetComponent<IBeltElement>();
                    if (temp.CanConnectToBeltElement(this))
                    {
                        output = temp;
                        temp.SetPrevBeltElement(this);
                    }
                }

                for (int i = 0; i < countUnderground; i++)
                {
                    UndergroundBelt temp;
                    if (underground[i].TryGetComponent(out temp) &&
                        (input == null || input.GetPosition().x > temp.GetPosition().x) && //check if one found is closer than current connection
                        (temp.transitionGround == TransitionGround.input && temp.outputOrientation == outputOrientation)//check orientation and that it is output
                        )
                    {
                        input = temp;
                        input.SetNextBeltElement(this);
                    }
                }
                break;
            case 6: //south input
                outputOrientation = ConveyorBelt.OutputOrientation.South;
                transitionGround = TransitionGround.input;
                countRegular = Physics2D.OverlapBox((Vector2)this.transform.position + new Vector2(0, 1), new Vector2(0.5f, 0.5f), 0, filter, regular);
                countUnderground = Physics2D.OverlapBox((Vector2)this.transform.position - new Vector2(0, searchDist / 2.0f), new Vector2(0.5f, searchDist), 0, filter, underground);
                if (countRegular >= 1)
                {
                    IBeltElement temp = regular[0].GetComponent<IBeltElement>();
                    if (temp.GetOutputOrientation() == outputOrientation)
                    {
                        input = temp;
                        temp.SetNextBeltElement(this);
                    }
                }

                for (int i = 0; i < countUnderground; i++)
                {
                    UndergroundBelt temp;
                    if (underground[i].TryGetComponent(out temp) &&
                        (output == null || output.GetPosition().y < temp.GetPosition().y) && //check if one found is closer than current connection
                        (temp.transitionGround == TransitionGround.output && temp.outputOrientation == outputOrientation)//check orientation and that it is output
                        )
                    {
                        output = temp;
                        output.SetPrevBeltElement(this);
                    }
                }

                break;
            default: //south output
            case 7:
                outputOrientation = ConveyorBelt.OutputOrientation.South;
                transitionGround = TransitionGround.output;
                countRegular = Physics2D.OverlapBox((Vector2)this.transform.position - new Vector2(0, 1), new Vector2(0.5f, 0.5f), 0, filter, regular);
                countUnderground = Physics2D.OverlapBox((Vector2)this.transform.position + new Vector2(0, searchDist / 2.0f), new Vector2(0.5f, searchDist), 0, filter, underground);
                if (countRegular >= 1)
                {
                    IBeltElement temp = regular[0].GetComponent<IBeltElement>();
                    if (temp.CanConnectToBeltElement(this))
                    {
                        output = temp;
                        temp.SetPrevBeltElement(this);
                    }
                }

                for (int i = 0; i < countUnderground; i++)
                {
                    UndergroundBelt temp;
                    if (underground[i].TryGetComponent(out temp) &&
                        (input == null || input.GetPosition().y < temp.GetPosition().y) && //check if one found is closer than current connection
                        (temp.transitionGround == TransitionGround.input && temp.outputOrientation == outputOrientation)//check orientation and that it is output
                        )
                    {
                        input = temp;
                        input.SetNextBeltElement(this);
                    }
                }
                break;
        }
        GetComponent<SpriteRenderer>().sprite = AnimationPropertiesPool.Instance.rotatedSpriteProperties[rotatedSpritePropertyId].sprites[rotation];
    }

    public void SetNextBeltElement(IBeltElement beltElement)
    {
        UndergroundBelt belt = beltElement as UndergroundBelt;
        if(transitionGround == TransitionGround.input && belt != null && belt.transitionGround == TransitionGround.output)
        {
            output = belt;
        }
        else if(transitionGround == TransitionGround.output)
        {
            output = beltElement;
        }
    }

    public void SetPrevBeltElement(IBeltElement beltElement)
    {
        UndergroundBelt belt = beltElement as UndergroundBelt;
        if(transitionGround == TransitionGround.output && belt != null && belt.transitionGround == TransitionGround.input)
        {
            input = belt;
        }
        else if(transitionGround == TransitionGround.input && beltElement.GetOutputOrientation() == outputOrientation)
        {
            input = beltElement;
        }
    }

    public bool TryInsertHorizontal(ItemEntity item, float x)
    {
        if(heldItems < items.Length)
        {
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
            items[heldItems] = item;
            heldItems++;
            return true;
        }
        return false;
    }
}
