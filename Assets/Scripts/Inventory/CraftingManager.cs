using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraftingManager : MonoBehaviour
{
    /// <summary>
    /// Whether the crafting manager can accept a queue of items to craft, or whether it will attempt to continue to produce a recipe given to it.
    /// SetRecipe will either queue or set a fixed recipe based on this state.
    /// Need to work in warehouse requesting
    /// </summary>

    public abstract void SetRecipe(Recipe recipe);
    public abstract Recipe GetRecipe();
}
