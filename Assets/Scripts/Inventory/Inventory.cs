using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    bool inventoryHasChanged = true;
    private List<ItemStack> inventory;
    [SerializeField] private int size = 0;
    List<KeyValuePair<Recipe, int>> possibleRecipes;
    public ReadOnlyCollection<ItemStack> inventoryReadOnly { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        possibleRecipes = new List<KeyValuePair<Recipe, int>>();
        inventory = new List<ItemStack>(size);
        for (int i = 0; i < size; i++)
        {
            inventory.Add(null);
        }
        inventoryReadOnly = inventory.AsReadOnly();
    }

    public virtual void TransferMouse(Inventory mouse, int slotID)
    {
        if(mouse.inventory[0] == null)
        {
            mouse.inventory[0] = inventory[slotID];
            inventory[slotID] = null;

        }
        else if(mouse.inventory[0] != null && inventory[slotID] == null)
        {
            inventory[slotID] = mouse.inventory[0];
            mouse.inventory[0] = null;
        }
        else if(mouse.inventory[0] != null && inventory[slotID] != null)
        {
            if(inventory[slotID].item.id == mouse.inventory[0].item.id)
            {
                inventory[slotID].size += mouse.inventory[0].size;
                if(inventory[slotID].size > inventory[slotID].item.maxStack)
                {
                    mouse.inventory[0].size = inventory[slotID].size - inventory[slotID].item.maxStack;
                    inventory[slotID].size = inventory[slotID].item.maxStack;
                }
                else
                {
                    mouse.inventory[0] = null;
                }
            }
        }
        inventoryHasChanged = true;
    }

    public virtual bool CalculateRecipes(out List<KeyValuePair<Recipe, int>> calculatedRecipes)
    {
        bool changed = false;
        if (inventoryHasChanged)
        {
            changed = true;
            List<Recipe> recipes = Definitions.Instance.RecipeDefinitions;
            possibleRecipes.Clear();
            for (int i = 0; i < recipes.Count; i++)
            {
                int amount = 999;
                bool[] found = new bool[recipes[i].Ingredients.Count];
                for (int s = 0; s < recipes[i].Ingredients.Count; s++)
                {
                    int possibleAmount = 0;
                    for (int j = 0; j < inventory.Count; j++)
                    {
                        if (inventory[j] != null && inventory[j].item.id == recipes[i].Ingredients[s].ItemID)
                        {
                            found[s] = true;
                            possibleAmount += inventory[j].size;
                        }
                    }
                    possibleAmount = possibleAmount / recipes[i].Ingredients[s].Amount;
                    if (possibleAmount < amount)
                    {
                        amount = possibleAmount;
                    }
                }
                bool possible = true;
                for (int c = 0; c < found.Length; c++)
                {
                    possible = possible && found[c];
                }
                if (possible)
                {
                    possibleRecipes.Add(new KeyValuePair<Recipe, int>(recipes[i], amount));
                }
                else
                {
                    possibleRecipes.Add(new KeyValuePair<Recipe, int>(recipes[i], 0));
                }
            }
            inventoryHasChanged = false;
        }
        calculatedRecipes = possibleRecipes;
        return changed;
    }


    /// <summary>
    /// Consumes items from inventory for crafting. Fails if items cannot be found.
    /// There must be a better way to do this.  I just don't see it.  I added as many exit situations as possible, and got it down to two for loops.
    /// Maybe this is how other games do it.  This is really only for queueable crafting, the fixedinventory for machines will probably make this substantially easier
    /// </summary>
    /// <param name="ingredients"></param>
    /// <returns></returns>
    public virtual bool ConsumeItems(List<Ingredient> ingredients)
    {
        List<List<int>> slots = new List<List<int>>();
        for(int i = 0; i < ingredients.Count; i++)
        {
            List<int> slotIndices = new List<int>();
            //used so that I can subtract from the inventory with running into a gotcha situation where there actually wasn't enough
            //of an ingredient and I had already subtracted the items.
            int amount = 0;
            for(int j = 0; j < inventory.Count; j++)
            {
                if(inventory[j] != null && inventory[j].item.id == ingredients[i].ItemID)
                {
                    slotIndices.Add(j);
                    amount += inventory[j].size;
                }
            }
            if(slotIndices.Count == 0 || amount < ingredients[i].Amount)
            {
                return false;
            }
            slots.Add(slotIndices);
        }
        for(int i = 0; i < ingredients.Count; i++)
        {
            int amountToSubtract = ingredients[i].Amount;
            for(int j = 0; j < slots[i].Count; j++)
            {
                if(inventory[slots[i][j]].size > amountToSubtract)
                {
                    inventory[slots[i][j]].size -= amountToSubtract;
                    break;
                }
                else
                {
                    amountToSubtract -= inventory[slots[i][j]].size;
                    inventory[slots[i][j]] = null;
                }
            }
        }
        inventoryHasChanged = true;
        return true;
    }


    /// <summary>
    /// Fails (returns false) if items cannot be inserted, otherwise inserts the items and returns true.
    /// Oh god this is worse than ConsumeItems.
    /// </summary>
    /// <param name="results"></param>
    /// <returns></returns>
    public virtual bool InsertItems(List<Ingredient> results)
    {
        //Check that the collection of items can be inserted
        for(int i = 0; i < results.Count; i++)
        {
            int amountToBePlaced = results[i].Amount;
            for(int j = 0; j < inventory.Count; j++)
            {
                if (inventory[j] != null && inventory[j].item.id == results[i].ItemID)
                {
                    if(amountToBePlaced + inventory[j].size <= inventory[j].item.maxStack)
                    {
                        amountToBePlaced = 0;
                        break;
                    }
                    else
                    {
                        amountToBePlaced -= (inventory[j].item.maxStack - inventory[j].size);
                    }
                }
                else if (inventory[j] == null && amountToBePlaced > 0) {
                    inventory[j] = new ItemStack(results[i].item, 0); //reserve the slot so other results cannot consider slot empty
                    amountToBePlaced -= results[i].item.maxStack;
                }
            }
            if(amountToBePlaced > 0)
            {
                for(int j = 0; j < inventory.Count; j++) //clear reserved slots
                {
                    if(inventory[j].size == 0)
                    {
                        inventory[j] = null;
                    }
                }
                return false;
            }
        }

        //Insert the items
        //Previously reserved slots should now be filled without any worries
        for (int i = 0; i < results.Count; i++)
        {
            int amountToBePlaced = results[i].Amount;
            for (int j = 0; j < inventory.Count; j++)
            {
                if (inventory[j].item.id == results[i].ItemID)
                {
                    if (amountToBePlaced + inventory[j].size <= inventory[j].item.maxStack)
                    {
                        inventory[j].size += amountToBePlaced;
                        break;
                    }
                    else
                    {
                        amountToBePlaced -= (inventory[j].item.maxStack - inventory[j].size);
                        inventory[j].size = inventory[j].item.maxStack;
                    }
                }
            }
        }
        inventoryHasChanged = true;
        return true;
    }

    public void InsertAt(int index, ItemStack itemStack)
    {
        inventory[index] = itemStack;
    }

    /// <summary>
    /// Inserts items to inventory
    /// </summary>
    /// <param name="itemStack"></param>
    /// <returns>Remainder if cannot be inserted</returns>
    public virtual ItemStack InsertStack(ItemStack itemStack)
    {
        inventoryHasChanged = true;
        for(int i = 0; i < inventory.Count; i++)
        {
            if(inventory[i] == null)
            {
                inventory[i] = itemStack;
                return null;
            }
            else if(inventory[i].item.id == itemStack.item.id)
            {
                inventory[i].size += itemStack.size;
                if(inventory[i].size < inventory[i].item.maxStack)
                {
                    return null;
                }
                else
                {
                    itemStack.size = inventory[i].size - inventory[i].item.maxStack;
                    inventory[i].size = inventory[i].item.maxStack;
                }
            }
            if (itemStack.size <= 0)
            {
                return null;
            }
        }
        return itemStack;
    }

    public void ChangeStackCount(int index, int newCount)
    {
        if(newCount == 0)
        {
            inventory[index] = null;
        }
        if(inventory[index] != null)
        {
            inventory[index].size = newCount;
        }
    }
    public void DecrementStack(int index)
    {
        if (inventory[index] != null)
        {
            inventory[index].size -= 1;
            if(inventory[index].size == 0)
            {
                inventory[index] = null;
            }
        }
    }
}
