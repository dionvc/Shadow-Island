using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManagerFixed : CraftingManager
{
    Recipe recipe = null;
    Mineable[] mineables;
    int currentMineable = 0;
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
        if (mineables != null && currentMineable < mineables.Length && mineables[currentMineable] == null)
        {
            currentMineable++;
        }
        if (mineables != null && currentMineable < mineables.Length)
        {
            recipeProgress++;
            if(recipeProgress > mineables[currentMineable].miningTime)
            {
                if(mineables[currentMineable].OnMine(inventory))
                {
                    recipeProgress = 0;
                }
                
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
    public void SetMineables(List<Mineable> mineables)
    {
        this.mineables = new Mineable[mineables.Count];
        mineables.CopyTo(this.mineables);
    }

    public float GetRemainingTime()
    {
        if(currentMineable < mineables.Length)
        {
            return mineables[currentMineable].miningTime;
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
