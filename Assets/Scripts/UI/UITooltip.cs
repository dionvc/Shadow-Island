using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UITooltip : MonoBehaviour
{
    PointerEventData pointerData;
    List<RaycastResult> results;
    CraftingSlot recipeSlot;
    InventorySlot itemSlot;
    [SerializeField] GameObject tooltipPanel;
    [SerializeField] GameObject mainHeader;
    [SerializeField] GameObject description;
    [SerializeField] GameObject ingredientsPanel;
    [SerializeField] GameObject resultsPanel;
    [SerializeField] GameObject craftingTime;
    [SerializeField] GameObject damageHeader;
    [SerializeField] GameObject damageDescriptionAmmo;
    [SerializeField] GameObject damageDescriptionThrown;
    List<GameObject> ingredientsItemElements; //stores all the item listing prefabs
    List<GameObject> resultsItemElements;

    [SerializeField] GameObject ingredientsItemPrefab;
    

    bool changed = false;
    float adjustXY = 8;
    RectTransform tooltipPanelTransform;
    Canvas canvas;

    void Start()
    {
        results = new List<RaycastResult>(16);
        tooltipPanelTransform = tooltipPanel.GetComponent<RectTransform>();
        canvas = this.transform.parent.GetComponent<Canvas>();
        ingredientsItemElements = new List<GameObject>();
        resultsItemElements = new List<GameObject>();
        for (int i = 0; i < 9; i++)
        {
            ingredientsItemElements.Add(null);
            resultsItemElements.Add(null);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        results.Clear();
        EventSystem.current.RaycastAll(pointerData, results);
        InventorySlot tempItem = null;
        CraftingSlot tempCraft = null;
        for(int i = 0; i < results.Count; i++)
        {
            if(results[i].gameObject.TryGetComponent(out tempItem))
            {
                break;
            }
            else if(results[i].gameObject.TryGetComponent(out tempCraft))
            {
                itemSlot = null;
                break;
            }
        }
        if((tempCraft != null && recipeSlot != null && tempCraft.recipeID != recipeSlot.recipeID)
            || tempCraft == null)
        {
            changed = true;
        }
        recipeSlot = tempCraft;
        if((tempItem != null && tempItem.item != null && itemSlot != null && itemSlot.item != null && tempItem.item.id != itemSlot.item.id)
            || tempItem == null || tempItem.item == null)
        {
            changed = true;
        }
        itemSlot = tempItem;
        
        

        if(itemSlot != null && itemSlot.item != null && changed)
        {
            tooltipPanel.SetActive(true);
            mainHeader.GetComponentInChildren<TextMeshProUGUI>().SetText(itemSlot.item.name);
            if (itemSlot.item.description != null && itemSlot.item.description != "")
            {
                description.SetActive(true);
                description.GetComponentInChildren<TextMeshProUGUI>().text = itemSlot.item.description;
            }
            else
            {
                description.SetActive(false);
            }
            //if the item is ammo or thrown list damage.
            ingredientsPanel.SetActive(false);
            resultsPanel.SetActive(false);
            craftingTime.SetActive(false);
            ItemAmmo itemAmmo = itemSlot.item as ItemAmmo;
            if(itemAmmo != null)
            {
                damageHeader.SetActive(true);
                damageDescriptionAmmo.SetActive(true);
                damageDescriptionAmmo.GetComponentInChildren<TextMeshProUGUI>().SetText(System.Enum.GetName(typeof(Health.DamageType), itemAmmo.damageType) + " Damage " + itemAmmo.damage +
                                                                              "\nFiring Rate {0:1}/s", (50.0f / itemAmmo.firingRate));
            }
            else
            {
                damageHeader.SetActive(false);
                damageDescriptionAmmo.SetActive(false);
            }
            changed = false;
        }
        else if(recipeSlot != null && changed)
        {
            tooltipPanel.SetActive(true);
            Recipe recipe = Definitions.Instance.RecipeDictionaryIndexed[recipeSlot.recipeID];
            if(recipe.Results.Count > 1)
            {
                description.SetActive(false);
                damageHeader.SetActive(false);
                damageDescriptionAmmo.SetActive(false);
                resultsPanel.SetActive(true);
            }
            else
            {
                if (recipe.Results[0].item.description != null && recipe.Results[0].item.description != "")
                {
                    description.SetActive(true);
                    description.GetComponentInChildren<TextMeshProUGUI>().text = recipe.Results[0].item.description;
                }
                else
                {
                    description.SetActive(false);
                }
                resultsPanel.SetActive(false);
                ItemAmmo itemAmmo = recipe.Results[0].item as ItemAmmo;
                if (itemAmmo != null)
                {
                    damageHeader.SetActive(true);
                    damageDescriptionAmmo.SetActive(true);
                    damageDescriptionAmmo.GetComponentInChildren<TextMeshProUGUI>().SetText(System.Enum.GetName(typeof(Health.DamageType), itemAmmo.damageType) + " Damage " + itemAmmo.damage +
                                                                              "\nFiring Rate {0:1}/s", (50.0f / itemAmmo.firingRate));
                }
                else
                {
                    damageHeader.SetActive(false);
                    damageDescriptionAmmo.SetActive(false);
                }
            }
            mainHeader.GetComponentInChildren<TextMeshProUGUI>().SetText(recipe.name);
            ingredientsPanel.SetActive(true);
            craftingTime.SetActive(true);
            craftingTime.GetComponentInChildren<TextMeshProUGUI>().SetText("{0:1}s Crafting Time", (recipe.craftingTime / 50.0f));

            for (int i = 0; i < ingredientsItemElements.Count; i++)
            {
                if(i < recipe.Ingredients.Count)
                {
                    if(ingredientsItemElements[i] == null)
                    {
                        ingredientsItemElements[i] = Instantiate(ingredientsItemPrefab, ingredientsPanel.transform, false);
                    }
                    ingredientsItemElements[i].SetActive(true);
                    ingredientsItemElements[i].transform.GetComponentInChildren<Image>().sprite = recipe.Ingredients[i].item.itemSprite;
                    ingredientsItemElements[i].transform.GetComponentInChildren<TextMeshProUGUI>().SetText("x " + recipe.Ingredients[i].Amount + " " + recipe.Ingredients[i].item.name);
                }
                else
                {
                    if (ingredientsItemElements[i] != null)
                    {
                        ingredientsItemElements[i].SetActive(false);
                    }
                }
            }

            if(recipe.Results.Count > 1)
            {
                for (int i = 0; i < resultsItemElements.Count; i++)
                {
                    if (i < recipe.Results.Count)
                    {
                        if (resultsItemElements[i] == null)
                        {
                            resultsItemElements[i] = Instantiate(ingredientsItemPrefab, resultsPanel.transform, false);
                        }
                        resultsItemElements[i].SetActive(true);
                        resultsItemElements[i].transform.GetComponentInChildren<Image>().sprite = recipe.Results[i].item.itemSprite;
                        resultsItemElements[i].transform.GetComponentInChildren<TextMeshProUGUI>().SetText("x " + recipe.Results[i].Amount + " " + recipe.Results[i].item.name);
                    }
                    else
                    {
                        if (resultsItemElements[i] != null)
                        {
                            resultsItemElements[i].SetActive(false);
                        }
                    }
                }
            }

            
            changed = false;
        }
        else if(changed == true)
        {
            tooltipPanel.SetActive(false);
            changed = false;
        }

        float width = tooltipPanelTransform.rect.width * canvas.scaleFactor;
        float height = tooltipPanelTransform.rect.height * canvas.scaleFactor;
        Vector3 position = Input.mousePosition;
        if(Input.mousePosition.x + width > Screen.width)
        {
            position.x -= width;
            position.x -= adjustXY;
        }
        else
        {
            position.x += adjustXY;
        }
        if(Input.mousePosition.y - height < 0)
        {
            position.y += height;
            position.y += adjustXY;
        }
        else
        {
            position.y -= adjustXY;
        }
        this.transform.position = position;
    }

    
}
