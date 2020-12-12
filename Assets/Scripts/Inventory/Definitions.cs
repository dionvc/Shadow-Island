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

    private Dictionary<string, List<Recipe>> recipeDic;
    private Dictionary<int, Recipe> recipeDicIndexed;

    public Dictionary<string, List<Recipe>> RecipeDictionary
    {
        get
        {
            if(recipeDic == null)
            {
                recipeDic = new Dictionary<string, List<Recipe>>();
                foreach (Recipe recipe in RecipeDefinitions)
                {
                    for (int i = 0; i < recipe.craftingTags.Length; i++)
                    {
                        if(!recipeDic.ContainsKey(recipe.craftingTags[i]))
                        {
                            recipeDic[recipe.craftingTags[i]] = new List<Recipe>();
                        }
                        recipeDic[recipe.craftingTags[i]].Add(recipe);
                    }
                }
            }
            return recipeDic;
        }
    }

    public Dictionary<int, Recipe> RecipeDictionaryIndexed
    {
        get
        {
            if(recipeDicIndexed == null)
            {
                recipeDicIndexed = new Dictionary<int, Recipe>();
                foreach(Recipe recipe in RecipeDefinitions)
                {
                    recipeDicIndexed[recipe.recipeID] = recipe;
                }
            }
            return recipeDicIndexed;
        }
    }
    [SerializeField] List<Item> ItemDefinitions = new List<Item>();
    [SerializeField] List<ItemThrowable> ItemThrowableDefintions = new List<ItemThrowable>();
    [SerializeField] List<ItemAmmo> ItemAmmoDefinitions = new List<ItemAmmo>();

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
                foreach (Item item in ItemAmmoDefinitions) itemDic[item.id] = item;
            }
            return itemDic;
        }
    }

    private void Start()
    {
        foreach(ItemAmmo ammo in ItemAmmoDefinitions)
        {
            ammo.InitializeFilter();
        }
    }
}