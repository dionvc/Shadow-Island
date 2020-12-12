using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] GameObject craftingSlot;
    Inventory inventory;
    [SerializeField] MenuSlideOut menuToHideOnRecipeSelect = null;
    List<GameObject> craftingSlots;
    bool recalculateUI = true;
    // Start is called before the first frame update
    void Start()
    {
        craftingSlots = new List<GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        List<KeyValuePair<Recipe, int>> recipeList;
        List<Recipe> regularList;
        bool inventoryIsRecipe = inventory is InventoryRecipe;
        if (inventory != null && inventoryIsRecipe && recalculateUI)
        {
            regularList = ((InventoryRecipe)inventory).GetRecipes();
            while (craftingSlots.Count > regularList.Count)
            {
                Destroy(craftingSlots[craftingSlots.Count - 1]);
                craftingSlots.RemoveAt(craftingSlots.Count - 1);
            }
            for (int i = 0; i < craftingSlots.Count; i++)
            {
                UpdateSlot(regularList[i], craftingSlots[i]);
            }
            while (craftingSlots.Count < regularList.Count)
            {
                GameObject currentSlot = Instantiate(craftingSlot, this.transform);
                craftingSlots.Add(currentSlot);
                UpdateSlot(regularList[craftingSlots.Count - 1], currentSlot);
                currentSlot.GetComponent<Button>().onClick.AddListener(delegate { OnClickCraftingSlot(currentSlot); });
            }
            recalculateUI = false;
        }
        else if (inventory != null && !inventoryIsRecipe && inventory.CalculateRecipes(out recipeList) )
        {
            while (craftingSlots.Count > recipeList.Count)
            {
                Destroy(craftingSlots[craftingSlots.Count - 1]);
                craftingSlots.RemoveAt(craftingSlots.Count - 1);
            }
            for (int i = 0; i < craftingSlots.Count; i++)
            {
                UpdateSlot(recipeList[i], craftingSlots[i]);
            }
            while (craftingSlots.Count < recipeList.Count)
            {
                GameObject currentSlot = Instantiate(craftingSlot, this.transform);
                craftingSlots.Add(currentSlot);
                UpdateSlot(recipeList[craftingSlots.Count - 1], currentSlot);
                currentSlot.GetComponent<Button>().onClick.AddListener(delegate { OnClickCraftingSlot(currentSlot); });
            }
        }
    }

    void OnClickCraftingSlot(GameObject craftingSlot)
    {
        CraftingSlot slotScript = craftingSlot.GetComponent<CraftingSlot>();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            CraftingManagerQueue queue;
            if(inventory.gameObject.TryGetComponent(out queue))
            {
                queue.SetRecipe(Definitions.Instance.RecipeDictionaryIndexed[slotScript.recipeID], Mathf.Min(slotScript.amount, 5));
                return;
            }
        }
        else if(Input.GetKey(KeyCode.LeftControl))
        {
            CraftingManagerQueue queue;
            if (inventory.gameObject.TryGetComponent(out queue))
            {
                queue.SetRecipe(Definitions.Instance.RecipeDictionaryIndexed[craftingSlot.GetComponent<CraftingSlot>().recipeID], slotScript.amount);
                return;
            }
        }
        CraftingManager craftingManager;
        if (inventory.gameObject.TryGetComponent(out craftingManager))
        {
            craftingManager.SetRecipe(Definitions.Instance.RecipeDictionaryIndexed[slotScript.recipeID]);
        }
        if(menuToHideOnRecipeSelect != null)
        {
            menuToHideOnRecipeSelect.TogglePanelOff();
        }
    }

    void UpdateSlot(KeyValuePair<Recipe, int> recipe, GameObject craftingSlot)
    {
        if (recipe.Key != null)
        {
            GameObject image = craftingSlot.transform.GetChild(0).gameObject;
            image.SetActive(true);
            image.GetComponent<Image>().sprite = recipe.Key.recipeSprite;
            GameObject text = image.transform.GetChild(0).gameObject;
            if (recipe.Value > 1)
            {
                text.GetComponent<TMP_Text>().SetText(recipe.Value.ToString());
            }
            else
            {
                text.GetComponent<TMP_Text>().SetText("");
            }
        }
        else
        {
            GameObject image = craftingSlot.transform.GetChild(0).gameObject;
            image.SetActive(false);
        }
        if(recipe.Value <= 0)
        {
            craftingSlot.GetComponent<Button>().interactable = false;
        }
        else
        {
            craftingSlot.GetComponent<Button>().interactable = true;
        }
        CraftingSlot slotScript = craftingSlot.GetComponent<CraftingSlot>();
        slotScript.recipeID = recipe.Key.recipeID;
        slotScript.amount = recipe.Value;
    }

    void UpdateSlot(Recipe recipe, GameObject craftingSlot)
    {
        if (recipe != null)
        {
            GameObject image = craftingSlot.transform.GetChild(0).gameObject;
            image.SetActive(true);
            image.GetComponent<Image>().sprite = recipe.recipeSprite;
            GameObject text = image.transform.GetChild(0).gameObject;
            text.GetComponent<TMP_Text>().SetText("");
        }
        else
        {
            GameObject image = craftingSlot.transform.GetChild(0).gameObject;
            image.SetActive(false);
        }
        craftingSlot.GetComponent<CraftingSlot>().recipeID = recipe.recipeID;
    }

    public void SetLinkedInventory(Inventory inventory)
    {
        this.inventory = inventory;
        if (inventory == null)
        {
            MenuSlideOut menuSlideOut = null;
            if (transform.parent.TryGetComponent<MenuSlideOut>(out menuSlideOut))
            {
                menuSlideOut.TogglePanel(MenuSlideOut.PanelState.panelIn);
            }
        }
        else
        {
            MenuSlideOut menuSlideOut = null;
            if (transform.parent.TryGetComponent<MenuSlideOut>(out menuSlideOut))
            {
                menuSlideOut.TogglePanel(MenuSlideOut.PanelState.panelOut);
            }
        }
        recalculateUI = true;
    }
}
