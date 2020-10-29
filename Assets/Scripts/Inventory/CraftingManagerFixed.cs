using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManagerFixed : CraftingManager
{
    Recipe recipe = null;
    Mineable mineable = null;
    InventoryFixed inventory;

    public int recipeProgress {get; private set;} = 0;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<InventoryFixed>();
        if(inventory == null) //if there is no inventory, this can't do anything and should not exist
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(mineable != null)
        {
            recipeProgress++;
            if(recipeProgress > mineable.miningTime)
            {
                mineable.OnMine(inventory);
                recipeProgress = 0;
            }
        }
        else if (recipe != null)
        {
            if (recipeProgress == 0)
            {
                if (inventory.ConsumeItems(recipe.Ingredients))
                {
                    recipeProgress++;
                }
            }
            else
            {
                recipeProgress++;
                if (recipeProgress > recipe.craftingTime)
                {
                    if (inventory.InsertItems(recipe.Results))
                    {
                        recipeProgress = 0;
                    }
                }
            }
        }
    }

    public override void SetRecipe(Recipe recipe)
    {

    }

    /// <summary>
    /// Use this method for buildings that should produce items from mineable resources
    /// </summary>
    public void SetMineable(Mineable mineable)
    {
        this.mineable = mineable;
    }

    public float GetRemainingTime()
    {
        if(mineable != null)
        {
            return mineable.miningTime;
        }
        else if(recipe != null)
        {
            return recipe.craftingTime;
        }
        else
        {
            return 0;
        }
    }

    public override Recipe GetRecipe()
    {
        return null;
    }
}
