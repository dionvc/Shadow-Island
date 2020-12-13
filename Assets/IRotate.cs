using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRotate
{
    Sprite GetRotatedPlaceSprite(int rotation);
    void InitializeRotated(int rotation);
}
