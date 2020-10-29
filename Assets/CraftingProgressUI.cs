using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingProgressUI : MonoBehaviour
{
    [SerializeField] CraftingManagerQueue craftingManager = null;
    [SerializeField] Image fillImage = null;
    [SerializeField] Image currentCraft = null;
    [SerializeField] Inventory inventory = null;
    [SerializeField] GameObject queuedCraftPrefab = null;
    [SerializeField] GameObject queuedCraftPanel = null;
    GameObject[] queuedCraftObjects = new GameObject[4];
    MenuSlideOut menuSlideOut = null;
    // Start is called before the first frame update
    void Start()
    {
        menuSlideOut = GetComponent<MenuSlideOut>();
        for(int i = 0; i < 4; i++)
        {
            queuedCraftObjects[i] = Instantiate(queuedCraftPrefab, queuedCraftPanel.transform);
            queuedCraftObjects[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (craftingManager.GetRecipe() != null)
        {
            menuSlideOut.TogglePanel(MenuSlideOut.PanelState.panelOut);
            currentCraft.enabled = true;
            fillImage.fillAmount = craftingManager.recipeProgress / craftingManager.GetRecipe().craftingTime;
            TMP_Text text = currentCraft.gameObject.GetComponentInChildren<TMP_Text>();
            if (craftingManager.GetRecipeAmount() > 1)
            {
                text.SetText(craftingManager.GetRecipeAmount().ToString());
            }
            else {
                text.SetText("");
            }
            currentCraft.sprite = craftingManager.GetRecipe().recipeSprite;
            int recipeCount = craftingManager.GetQueuedRecipeCount();
            for (int i = 0; i < 4; i++)
            {
                if (i >= recipeCount)
                {
                    queuedCraftObjects[i].SetActive(false);
                }
                else
                {
                    KeyValuePair<Recipe, int> recipePair = craftingManager.GetRecipeByIndex(i);
                    queuedCraftObjects[i].SetActive(true);
                    queuedCraftObjects[i].transform.GetChild(0).GetComponent<Image>().sprite = recipePair.Key.recipeSprite;
                    queuedCraftObjects[i].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().SetText(recipePair.Value.ToString());
                }
            }
        }
        else
        {
            menuSlideOut.TogglePanel(MenuSlideOut.PanelState.panelIn);
            fillImage.fillAmount = 0;
            currentCraft.enabled = false;
        }
    }

    public void OnClickCurrent()
    {
        int reduceAmount = 1;
        if(Input.GetKey(KeyCode.LeftShift)) {
            reduceAmount = 5;
        }
        if(Input.GetKey(KeyCode.LeftControl))
        {
            reduceAmount = craftingManager.GetRecipeAmount();
        }
        craftingManager.ReduceCurrentAmountBy(reduceAmount);
    }
}
