using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// This inventory is designed for craftingmanagerfixed when a recipe is set
/// </summary>
public class InventoryRecipe : Inventory
{
    private List<ItemStack> inputInventory;
    private List<ItemStack> outputInventory;
    public ReadOnlyCollection<ItemStack> inputInvReadOnly { get; private set; }
    public ReadOnlyCollection<ItemStack> outputInvReadOnly { get; private set; }
    CraftingManagerRecipe craftingManager;
    public int[] slotItemIDS;
    // Start is called before the first frame update
    void Start()
    {
        inputInventory = new List<ItemStack>();
        outputInventory = new List<ItemStack>();

        inputInvReadOnly = inputInventory.AsReadOnly();
        outputInvReadOnly = outputInventory.AsReadOnly();

        craftingManager = GetComponent<CraftingManagerRecipe>();
    }

    public override void TransferMouse(Inventory mouse, int slotID)
    {
        if (mouse.inventoryReadOnly[0] == null)
        {
            if (slotID < inputInventory.Count)
            {
                mouse.InsertAt(0, inputInventory[slotID]);
                inputInventory[slotID] = null;
            }
            else
            {
                mouse.InsertAt(0, outputInventory[slotID - inputInventory.Count]);
                outputInventory[slotID - inputInventory.Count] = null;
            }
        }
        else if (mouse.inventoryReadOnly[0] != null && slotID < inputInventory.Count && slotItemIDS[slotID] == mouse.inventoryReadOnly[0].item.id)
        {
            if(inputInventory[slotID] == null)
            {
                inputInventory[slotID] = new ItemStack(mouse.inventoryReadOnly[0].item, 0);
            }
            inputInventory[slotID].size += mouse.inventoryReadOnly[0].size;
            if (inputInventory[slotID].size > inputInventory[slotID].item.maxStack)
            {
                mouse.ChangeStackCount(0, inputInventory[slotID].size - inputInventory[slotID].item.maxStack);
                inputInventory[slotID].size = inputInventory[slotID].item.maxStack;
            }
            else
            {
                mouse.InsertAt(0, null);
            }
            //Cannot insert anything into results, only take from
        }
    }

    /// <summary>
    /// Consumes items from corresponding input slots.  Fails if too few of an ingredient is present.
    /// </summary>
    /// <param name="ingredients"></param>
    /// <returns></returns>
    public override bool ConsumeItems(List<Ingredient> ingredients)
    {
        for(int i = 0; i < inputInventory.Count; i++)
        {
            if(inputInventory[i] == null)
            {
                return false;
            }
            else if(inputInventory[i].size < ingredients[i].Amount) 
            {
                return false;
            }
        }
        for(int i = 0; i < inputInventory.Count; i++)
        {
            inputInventory[i].size -= ingredients[i].Amount;
            if(inputInventory[i].size == 0)
            {
                inputInventory[i] = null;
            }
        }
        return true;
    }

    /// <summary>
    /// Inserts items in corresponding results slots.  Fails if a corresponding result slot will be max stacked.
    /// </summary>
    /// <param name="results"></param>
    /// <returns></returns>
    public override bool InsertItems(List<Ingredient> results)
    {
        for(int i = 0; i < outputInventory.Count; i++)
        {
            if(outputInventory[i] != null && outputInventory[i].size + results[i].Amount > outputInventory[i].item.maxStack)
            {
                return false;
            }
        }
        for(int i = 0; i < outputInventory.Count; i++)
        {
            if(outputInventory[i] == null)
            {
                outputInventory[i] = new ItemStack(results[i].item, 0);
            }
            outputInventory[i].size += results[i].Amount;
        }
        return true;
    }

    public void SetRecipe(Recipe recipe)
    {
        //Need to handle what to do with existing items
        inputInventory.Clear();
        slotItemIDS = new int[recipe.Ingredients.Count + recipe.Results.Count];
        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            inputInventory.Add(null);
            slotItemIDS[i] = recipe.Ingredients[i].ItemID;
        }
        outputInventory.Clear();
        for(int i = 0; i < recipe.Results.Count; i++)
        {
            outputInventory.Add(null);
            slotItemIDS[i + recipe.Ingredients.Count] = recipe.Results[i].ItemID;
        }
    }

    public List<ItemStack> GatherResources()
    {
        List<ItemStack> resources = new List<ItemStack>();
        for(int i = 0; i < outputInventory.Count; i++)
        {
            resources.Add(outputInventory[i]);
            outputInventory[i] = null;
        }
        return resources;
    }

    public List<Ingredient> RequestIngredients()
    {
        List<Ingredient> requestedIngredients = new List<Ingredient>();
        for(int i = 0; i < inputInventory.Count; i++)
        {
            requestedIngredients.Add(new Ingredient(slotItemIDS[i], inputInventory[i].item.maxStack - inputInventory[i].size));
        }
        return requestedIngredients;
    }

    public List<Recipe> GetRecipes()
    {
        return Definitions.Instance.RecipeDefinitions;
    }

    public bool NeedGather()
    {
        for(int i = 0; i < outputInventory.Count; i++)
        {
            if(outputInventory[i] != null && outputInventory[i].size >= 2 * outputInventory[i].item.maxStack / 3)
            {
                return true;
            }
        }
        return false;
    }

    public bool NeedItems()
    {
        for (int i = 0; i < inputInventory.Count; i++)
        {
            if (inputInventory[i] == null || inputInventory[i].size < inputInventory[i].item.maxStack / 2)
            {
                return true;
            }
        }
        return false;
    }
}
