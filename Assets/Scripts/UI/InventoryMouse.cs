using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryMouse : MonoBehaviour
{
    Inventory inventory = null;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (inventory.inventoryReadOnly[0] != null) {

            transform.position = Input.mousePosition;
            GetComponent<Image>().enabled = true;
            GetComponent<Image>().sprite = inventory.inventoryReadOnly[0].item.itemSprite;
            GetComponentInChildren<TMP_Text>().SetText(inventory.inventoryReadOnly[0].size.ToString());
        }
        else
        {
            GetComponent<Image>().enabled = false;
            GetComponentInChildren<TMP_Text>().SetText("");
        }
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }
}
