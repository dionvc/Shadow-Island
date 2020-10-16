using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManagerFixed : CraftingManager
{
    Recipe recipe = null;
    Inventory inventory;
    public int recipeProgress {get; private set;} = 0;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<Inventory>();
        if(inventory == null) //if there is no inventory, this can't do anything and should not exist
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(recipe != null && recipeProgress == 0)
        {
            if (inventory.ConsumeItems(recipe.Ingredients))
            {
                recipeProgress++;
            }
        }
        else if(recipe != null)
        {
            recipeProgress++;
            if(recipeProgress > recipe.craftingTime)
            {
                if (inventory.InsertItems(recipe.Results))
                {
                    recipeProgress = 0;
                }
            }
        }
    }

    //does nothing, instead need to dynamically calculate recipe from input
    public override void SetRecipe(Recipe recipe)
    {

    }

    public override Recipe GetRecipe()
    {
        return null;
    }
    public override Queue<Recipe> GetRecipes()
    {
        return null;
    }
}
