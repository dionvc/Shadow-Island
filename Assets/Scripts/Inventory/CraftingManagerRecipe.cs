using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManagerRecipe : CraftingManager
{
    Recipe recipe = null;
    InventoryRecipe inventory;
    public float recipeProgress { get; private set; } = 0;
    [SerializeField] float craftingSpeed = 1;
    [SerializeField] string idleSprite = "";
    [SerializeField] string animationSprite = "";
    int idleSpriteCode = 0;
    int animationSpriteCode = 0;
    int currentState = 0;
    Animator animator = null;
    SpriteRenderer spriteRenderer = null;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<InventoryRecipe>();
        TryGetComponent(out animator);
        TryGetComponent(out spriteRenderer);
        if (idleSprite != "")
        {
            idleSpriteCode = Animator.StringToHash(idleSprite);
        }
        if (animationSprite != "")
        {
            animationSpriteCode = Animator.StringToHash(animationSprite);
        }
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
                recipeProgress += craftingSpeed;
                if (animator != null)
                {
                    ChangeAnimationState(animationSpriteCode);
                    animator.speed = 1;
                }
            }
            else if(animator != null)
            {
                ChangeAnimationState(idleSpriteCode);
                animator.speed = 0;
            }
        }
        else if(recipe != null)
        {
            recipeProgress += craftingSpeed;
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

    void ChangeAnimationState(int state)
    {
        if (state == currentState || state == 0)
        {
            return;
        }
        animator.Play(state);
        currentState = state;
    }
}
