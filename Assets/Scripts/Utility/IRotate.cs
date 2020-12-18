using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRotate
{
    Sprite GetRotatedPlaceSprite(int rotation);
    Vector2 GetRotatedBoundingBox(int rotation);
    void InitializeRotated(int rotation);
}
