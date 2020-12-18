using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBeltElement
{
    bool TryInsertVertical(ItemEntity item, float y); //try to insert vertical from the top or bottom
    bool TryInsertHorizontal(ItemEntity item, float x); //try to insert horizontal from the left or right
    void SetNextBeltElement(IBeltElement beltElement);
    void SetPrevBeltElement(IBeltElement beltElement);
    bool CheckCollisionVertical(float y);
    bool CheckCollisionHorizontal(float x);
    ConveyorBelt.OutputOrientation GetOutputOrientation();
    bool CanConnectToBeltElement(IBeltElement prevElement);
    Vector2 GetPosition();
}
