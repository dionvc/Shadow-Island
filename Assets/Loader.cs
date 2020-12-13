using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour, IBeltElement, IRotate
{
    IBeltElement belt;
    Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetRotatedPlaceSprite(int rotation)
    {
        throw new System.NotImplementedException();
    }

    public void InitializeRotated(int rotation)
    {
        throw new System.NotImplementedException();
    }

    public void SetNextBeltElement(IBeltElement beltElement)
    {
        belt = beltElement;
    }

    public void SetPrevBeltElement(IBeltElement beltElement)
    {
        belt = beltElement;
    }

    public ConveyorBelt.OutputOrientation GetOutputOrientation()
    {
        return 0;
    }

    public bool TryInsertVertical(ItemEntity item, float y, int callerFrame)
    {
        throw new System.NotImplementedException();
    }

    public bool TryInsertHorizontal(ItemEntity item, float x, int callerFrame)
    {
        throw new System.NotImplementedException();
    }

    public bool CheckCollisionVertical(float y, int callerFrame)
    {
        throw new System.NotImplementedException();
    }

    public bool CheckCollisionHorizontal(float x, int callerFrame)
    {
        throw new System.NotImplementedException();
    }
}
