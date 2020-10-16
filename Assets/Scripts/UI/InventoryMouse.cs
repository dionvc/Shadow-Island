using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryMouse : MonoBehaviour
{
    [SerializeField] Inventory inventory = null;
    // Start is called before the first frame update
    void Start()
    {

    }

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
}
