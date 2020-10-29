using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CraftingManagerQueue : CraftingManager
{
    LinkedList<KeyValuePair<Recipe, int>> recipeList = null;
    Recipe recipe = null;
    int currentAmount = 0;
    Inventory inventory;
    public int recipeProgress { get; private set; } = 0;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<Inventory>();
        recipeList = new LinkedList<KeyValuePair<Recipe,int>>();
        if (inventory == null) //if there is no inventory, this can't do anything and should not exist
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (recipe != null)
        {
            recipeProgress++;
            if (recipeProgress > recipe.craftingTime)
            {
                if (inventory.InsertItems(recipe.Results))
                {
                    recipeProgress = 0;
                    currentAmount--;
                    if (currentAmount == 0)
                    {
                        recipe = null;
                    }
                }
            }
        }
        if (recipe == null && recipeList.Count > 0)
        {
            recipe = recipeList.First.Value.Key;
            currentAmount = recipeList.First.Value.Value;
            recipeList.RemoveFirst();
        }
    }

    public override void SetRecipe(Recipe recipe)
    {
        if (inventory.ConsumeItems(recipe.Ingredients))
        {
            recipeList.AddLast(new KeyValuePair<Recipe,int>(recipe,1));
        }
    }

    public void SetRecipe(Recipe recipe, int amount)
    {
        List<Ingredient> ingredients = new List<Ingredient>();
        for(int i = 0; i < recipe.Ingredients.Count; i++)
        {
            ingredients.Add(new Ingredient(recipe.Ingredients[i].ItemID, amount * recipe.Ingredients[i].Amount));
        }
        if(inventory.ConsumeItems(ingredients))
        {
            recipeList.AddLast(new KeyValuePair<Recipe, int>(recipe, amount));
        }
    }

    public override Recipe GetRecipe()
    {
        return recipe;
    }
    
    public int GetRecipeAmount()
    {
        return currentAmount;
    }

    public KeyValuePair<Recipe, int> GetRecipeByIndex(int index) 
    {
        LinkedListNode<KeyValuePair<Recipe, int>> traverse = recipeList.First;
        for(int i = 0; i < index; i++)
        {
            traverse = traverse.Next;
        }
        return traverse.Value;
    }

    public int GetQueuedRecipeCount()
    {
        return recipeList.Count;
    }

    public bool ReduceCurrentAmountBy(int amount)
    {
        amount = Mathf.Min(amount, currentAmount);
        List<Ingredient> ingredientsToPushBack = new List<Ingredient>();
        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            ingredientsToPushBack.Add(new Ingredient(recipe.Ingredients[i].ItemID, amount * recipe.Ingredients[i].Amount));
        }
        if (recipe.Ingredients.Count == 0 || inventory.InsertItems(ingredientsToPushBack))
        {
            currentAmount -= amount;
            if(currentAmount == 0)
            {
                recipe = null;
                recipeProgress = 0;
            }
            if (recipe == null && recipeList.Count > 0)
            {
                recipe = recipeList.First.Value.Key;
                currentAmount = recipeList.First.Value.Value;
                recipeList.RemoveFirst();
            }
            return true;
        }
        return false;
    }
}
