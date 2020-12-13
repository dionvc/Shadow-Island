using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBeltElement
{
    bool TryInsertVertical(ItemEntity item, float y, int callerFrame); //try to insert vertical from the top or bottom
    bool TryInsertHorizontal(ItemEntity item, float x, int callerFrame); //try to insert horizontal from the left or right
    void SetNextBeltElement(IBeltElement beltElement);
    void SetPrevBeltElement(IBeltElement beltElement);
    bool CheckCollisionVertical(float y, int callerFrame);
    bool CheckCollisionHorizontal(float x, int callerFrame);
    ConveyorBelt.OutputOrientation GetOutputOrientation();
}
