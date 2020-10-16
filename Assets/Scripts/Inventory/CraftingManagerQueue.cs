using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManagerQueue : CraftingManager
{
    Queue<Recipe> recipeQueue = null;
    Recipe recipe = null;
    Inventory inventory;
    int recipeProgress = 0;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<Inventory>();
        recipeQueue = new Queue<Recipe>();
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
                    recipe = null;
                }
            }
        }
        if (recipe == null && recipeQueue.Count > 0)
        {
            recipe = recipeQueue.Dequeue();
        }
    }

    public override void SetRecipe(Recipe recipe)
    {
        if (inventory.ConsumeItems(recipe.Ingredients))
        {
            recipeQueue.Enqueue(recipe);
        }
    }

    public override Recipe GetRecipe()
    {
        return recipe;
    }

    public override Queue<Recipe> GetRecipes()
    {
        return recipeQueue;
    }
}
