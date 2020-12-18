using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public Sprite itemSprite;
    public string name;
    public string description;
    public GameObject placeableResult = null;
    public int maxStack;
    public int id;
}
