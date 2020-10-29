using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Definitions : MonoBehaviour
{

    #region Singleton Code
    private static Definitions instance;
    public static Definitions Instance
    {
        get
        {
            if (instance == null) Debug.LogError("No Instance of Definitions in the Scene!");
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Just one Instance of Definitions allowed!");
        }
    }
    #endregion

    [SerializeField] ParticleSystem.EmitParams explosionParams;

    public List<Recipe> RecipeDefinitions = new List<Recipe>();

    private Dictionary<int, Recipe> recipeDic;

    public Dictionary<int, Recipe> RecipeDictionary
    {
        get
        {
            if(recipeDic == null)
            {
                recipeDic = new Dictionary<int, Recipe>();
                foreach (Recipe recipe in RecipeDefinitions) recipeDic[recipe.recipeID] = recipe;
            }
            return recipeDic;
        }
    }

    [SerializeField] List<Item> ItemDefinitions = new List<Item>();
    [SerializeField] List<ItemThrowable> ItemThrowableDefintions = new List<ItemThrowable>();

    private Dictionary<int, Item> itemDic;
    public Dictionary<int, Item> ItemDictionary
    {
        get
        {
            // Definitions are never changed in game, so just copy references over once:
            if (itemDic == null)
            {
                itemDic = new Dictionary<int, Item>();
                foreach (Item item in ItemDefinitions) itemDic[item.id] = item;
                foreach (Item item in ItemThrowableDefintions) itemDic[item.id] = item;
            }
            return itemDic;
        }
    }
}