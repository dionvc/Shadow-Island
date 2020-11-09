using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MachineUI : MonoBehaviour
{
    [SerializeField] GameObject inventorySlot;
    [SerializeField] Inventory mouseInventory;
    [SerializeField] GameObject inputPanel;
    [SerializeField] GameObject outputPanel;
    [SerializeField] GameObject recipeSelector;
    [SerializeField] GameObject recipePanel;
    [SerializeField] Image progressBar;
    InventoryFixed inventoryFixed;
    InventoryRecipe inventoryRecipe;
    CraftingManagerFixed craftingManagerFixed;
    CraftingManagerRecipe craftingManagerRecipe;
    List<GameObject> inputInventory;
    List<GameObject> outputInventory;

    // Start is called before the first frame update
    void Start()
    {
        inputInventory = new List<GameObject>();
        outputInventory = new List<GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (inventoryRecipe != null)
        {
            while (inputInventory.Count > inventoryRecipe.inputInvReadOnly.Count)
            {
                Destroy(inputInventory[inputInventory.Count - 1]);
                inputInventory.RemoveAt(inputInventory.Count - 1);
            }
            for (int i = 0; i < inputInventory.Count; i++)
            {
                UpdateSlot(inventoryRecipe.inputInvReadOnly[i], inputInventory[i], i);
            }
            while (inputInventory.Count < inventoryRecipe.inputInvReadOnly.Count)
            {
                GameObject currentSlot = Instantiate(inventorySlot, inputPanel.transform);
                inputInventory.Add(currentSlot);
                UpdateSlot(inventoryRecipe.inputInvReadOnly[inputInventory.Count - 1], currentSlot, inputInventory.Count - 1);
                currentSlot.GetComponent<Button>().onClick.AddListener(delegate { OnClickInventorySlot(currentSlot); } );
            }

            while (outputInventory.Count > inventoryRecipe.outputInvReadOnly.Count)
            {
                Destroy(outputInventory[outputInventory.Count - 1]);
                outputInventory.RemoveAt(outputInventory.Count - 1);
            }
            for (int i = 0; i < outputInventory.Count; i++)
            {
                UpdateSlot(inventoryRecipe.outputInvReadOnly[i], outputInventory[i], i + inventoryRecipe.inputInvReadOnly.Count);
            }
            while (outputInventory.Count < inventoryRecipe.outputInvReadOnly.Count)
            {
                GameObject currentSlot = Instantiate(inventorySlot, outputPanel.transform);
                outputInventory.Add(currentSlot);
                UpdateSlot(inventoryRecipe.outputInvReadOnly[outputInventory.Count - 1], currentSlot, inventoryRecipe.inputInvReadOnly.Count + outputInventory.Count - 1);
                currentSlot.GetComponent<Button>().onClick.AddListener(delegate { OnClickInventorySlot(currentSlot); });
            }
            if (craftingManagerRecipe.GetRecipe() != null)
            {
                recipeSelector.transform.GetChild(0).gameObject.SetActive(true);
                recipeSelector.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = craftingManagerRecipe.GetRecipe().recipeSprite;
            }
            else
            {
                recipeSelector.transform.GetChild(0).gameObject.SetActive(false);
            }
            if (craftingManagerRecipe.GetRecipe() != null)
            {
                progressBar.fillAmount = craftingManagerRecipe.recipeProgress * 1.0f / craftingManagerRecipe.GetRecipe().craftingTime;
            }
            else
            {
                progressBar.fillAmount = 0;
            }
        }

        if(inventoryFixed != null)
        {
            //input
            while(inputInventory.Count > inventoryFixed.inputInvReadOnly.Count)
            {
                Destroy(inputInventory[inputInventory.Count - 1]);
                inputInventory.RemoveAt(inputInventory.Count - 1);
            }
            for(int i = 0; i < inputInventory.Count; i++) 
            {
                UpdateSlot(inventoryFixed.inputInvReadOnly[i], inputInventory[i], i);
            }
            while (inputInventory.Count < inventoryFixed.inputInvReadOnly.Count)
            {
                GameObject currentSlot = Instantiate(inventorySlot, inputPanel.transform);
                inputInventory.Add(currentSlot);
                UpdateSlot(inventoryFixed.inputInvReadOnly[inputInventory.Count - 1], currentSlot, inputInventory.Count - 1);
                currentSlot.GetComponent<Button>().onClick.AddListener(delegate { OnClickInventorySlot(currentSlot); });
            }
            //output
            while (outputInventory.Count > inventoryFixed.outputInvReadOnly.Count)
            {
                Destroy(outputInventory[outputInventory.Count - 1]);
                outputInventory.RemoveAt(outputInventory.Count - 1);
            }
            for (int i = 0; i < outputInventory.Count; i++)
            {
                UpdateSlot(inventoryFixed.outputInvReadOnly[i], outputInventory[i], i);
            }
            while (outputInventory.Count < inventoryFixed.outputInvReadOnly.Count)
            {
                GameObject currentSlot = Instantiate(inventorySlot, outputPanel.transform);
                outputInventory.Add(currentSlot);
                UpdateSlot(inventoryFixed.outputInvReadOnly[outputInventory.Count - 1], currentSlot, inventoryFixed.inputInvReadOnly.Count + outputInventory.Count - 1);
                currentSlot.GetComponent<Button>().onClick.AddListener(delegate { OnClickInventorySlot(currentSlot); });
            }
            if (craftingManagerFixed.GetRemainingTime() != 0)
            {
                progressBar.fillAmount = craftingManagerFixed.recipeProgress * 1.0f / craftingManagerFixed.GetRemainingTime();
            }
            else
            {
                progressBar.fillAmount = 0;
            }
        }
    }

    public void OnClickFuelSlot()
    {
        //transfer fuel
    }

    public void OnClickInventorySlot(GameObject inventorySlot)
    {
        if(inventoryFixed != null)
        {
            inventoryFixed.TransferMouse(mouseInventory, inventorySlot.GetComponent<InventorySlot>().slotID);
        }
        else if (inventoryRecipe != null)
        {
            inventoryRecipe.TransferMouse(mouseInventory, inventorySlot.GetComponent<InventorySlot>().slotID);
        }
    }

    public void SetLinkedMachine(GameObject machine)
    {
        
        CraftingManagerRecipe craftingManagerRecipe;
        CraftingManagerFixed craftingManagerFixed;
        if(machine == null)
        {
            this.craftingManagerFixed = null;
            this.craftingManagerRecipe = null;
            this.inventoryFixed = null;
            this.inventoryRecipe = null;
            GetComponent<MenuSlideOut>().TogglePanel(MenuSlideOut.PanelState.panelIn);
        }
        else if (machine.TryGetComponent(out craftingManagerFixed))
        {
            this.craftingManagerRecipe = null;
            this.craftingManagerFixed = craftingManagerFixed;
            recipeSelector.SetActive(false);
            inventoryFixed = machine.GetComponent<InventoryFixed>();
            inventoryRecipe = null;
            GetComponent<MenuSlideOut>().TogglePanel(MenuSlideOut.PanelState.panelOut);
        }
        else if (machine.TryGetComponent(out craftingManagerRecipe))
        {
            this.craftingManagerFixed = null;
            this.craftingManagerRecipe = craftingManagerRecipe;
            recipeSelector.SetActive(true);
            inventoryRecipe = machine.GetComponent<InventoryRecipe>();
            inventoryFixed = null;
            GetComponent<MenuSlideOut>().TogglePanel(MenuSlideOut.PanelState.panelOut);
            recipePanel.GetComponent<CraftingUI>().SetLinkedInventory(inventoryRecipe);
        }
    }

    void UpdateSlot(ItemStack itemStack, GameObject inventorySlot, int slotID)
    {
        if (itemStack != null)
        {
            GameObject image = inventorySlot.transform.GetChild(0).gameObject;
            image.SetActive(true);
            image.GetComponent<Image>().sprite = itemStack.item.itemSprite;
            GameObject text = image.transform.GetChild(0).gameObject;
            if (itemStack.size > 1)
            {
                text.GetComponent<TMP_Text>().SetText(itemStack.size.ToString());
            }
            else
            {
                text.GetComponent<TMP_Text>().SetText("");
            }
        }
        else
        {
            GameObject image = inventorySlot.transform.GetChild(0).gameObject;
            image.SetActive(false);
        }
        inventorySlot.GetComponent<InventorySlot>().slotID = slotID;
    }
}
