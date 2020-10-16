using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public int recipeID;
    public Sprite recipeSprite;
    public string name = "";
    public List<Ingredient> Ingredients = new List<Ingredient>();
    public List<Ingredient> Results = new List<Ingredient>();
    public float craftingTime = 1; //in terms of ticks, with 50 ticks being a second (based on fixedupdate)
}


[System.Serializable]
public class Ingredient
{
    public int ItemID;
    public int Amount;
    public Item item
    {
        get
        {
            return Definitions.Instance.ItemDictionary[ItemID];
        }
    }
}