using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManagerRecipe : CraftingManager
{
    Recipe recipe = null;
    InventoryRecipe inventory;
    public int recipeProgress { get; private set; } = 0;
    Animator animator = null;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<InventoryRecipe>();
        TryGetComponent(out animator);
        if(animator != null)
        {
            animator.speed = 0;
        }
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
                if (animator != null)
                {
                    animator.speed = 1;
                }
            }
            else if(animator != null)
            {
                animator.speed = 0;
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

    public override void SetRecipe(Recipe recipe)
    {
        if(this.recipe != null && this.recipe.recipeID == recipe.recipeID)
        {
            return;
        }
        recipeProgress = 0;
        this.recipe = recipe;
        inventory.SetRecipe(recipe);
    }

    public override Recipe GetRecipe()
    {
        return recipe;
    }
}
