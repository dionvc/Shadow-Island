using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreviewLocation : MonoBehaviour
{
    public int xPos { get; private set; } = 0;
    public int yPos { get; private set; } = 0;
    public void SetXY(int x, int y)
    {
        xPos = x;
        yPos = y;
    }
}
