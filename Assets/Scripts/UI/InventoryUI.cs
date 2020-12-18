using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.ObjectModel;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory = null;
    Inventory mouseInventory = null;
    [SerializeField] GameObject inventorySlot = null;
    [SerializeField] MenuSlideOut closeWhenNull = null;
    [SerializeField] Image image = null;
    SpriteRenderer spriteRenderer = null;
    List<GameObject> itemSlots;
    // Start is called before the first frame update
    void Start()
    {
        itemSlots = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        if (inventory != null)
        {
            if(spriteRenderer != null && image != null)
            {
                image.sprite = spriteRenderer.sprite;
                
            }
            ReadOnlyCollection<ItemStack> items = inventory.inventoryReadOnly;
            while (itemSlots.Count > items.Count)
            {
                Destroy(itemSlots[itemSlots.Count - 1]);
                itemSlots.RemoveAt(itemSlots.Count - 1);
            }
            for (int i = 0; i < itemSlots.Count; i++)
            {
                UpdateSlot(items[i], itemSlots[i], i);
            }
            while (itemSlots.Count < items.Count)
            {
                GameObject currentSlot = Instantiate(inventorySlot, this.transform);
                itemSlots.Add(currentSlot);
                UpdateSlot(inventory.inventoryReadOnly[itemSlots.Count - 1], currentSlot, itemSlots.Count - 1);
                currentSlot.GetComponent<Button>().onClick.AddListener(delegate { OnClickInventorySlot(currentSlot); });
            }
        }
    }

    void OnClickInventorySlot(GameObject inventorySlot)
    {
        AmmoSlot ammoInv = inventory as AmmoSlot;
        if (ammoInv != null)
        {
            ammoInv.TransferMouse(mouseInventory, inventorySlot.GetComponent<InventorySlot>().slotID);
        }
        else
        {
            inventory.TransferMouse(mouseInventory, inventorySlot.GetComponent<InventorySlot>().slotID);
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
        InventorySlot invSlot = inventorySlot.GetComponent<InventorySlot>();
        invSlot.slotID = slotID;
        if (itemStack != null)
        {
            invSlot.item = itemStack.item;
        }
    }

    public void SetViewedInventory(Inventory inventory)
    {
        this.inventory = inventory;
        if (inventory != null)
        {
            inventory.gameObject.TryGetComponent(out spriteRenderer);
        }
        if(spriteRenderer != null && image != null)
        {
            float width = spriteRenderer.bounds.size.x;
            float height = spriteRenderer.bounds.size.y;
            float factorWidth = 96.0f / width;
            float factorHeight = 96.0f / height;
            float factor = Mathf.Max(factorWidth, factorHeight);
            
            RectTransform transform = image.gameObject.GetComponent<RectTransform>();
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * factor);
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * factor);
        }
        if(inventory == null)
        {

            if(closeWhenNull != null)
            {
                closeWhenNull.TogglePanel(MenuSlideOut.PanelState.panelIn);
            }
        }
        else
        {
            if (closeWhenNull != null)
            {
                closeWhenNull.TogglePanel(MenuSlideOut.PanelState.panelOut);
            }
        }
    }

    public void SetMouseInventory(Inventory mouseInventory)
    {
        this.mouseInventory = mouseInventory;
    }
}
