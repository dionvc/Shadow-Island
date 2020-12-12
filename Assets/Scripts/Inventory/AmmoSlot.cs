using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSlot : Inventory
{
    [SerializeField] int ammoSlotCount;
    [SerializeField] ItemAmmo.AmmoType ammoRestriction;
    int currentSlot = 0;
    // Start is called before the first frame update
    void Start()
    {
        inventory = new List<ItemStack>(ammoSlotCount);
        for(int i = 0; i < ammoSlotCount; i++)
        {
            inventory.Add(null);
        }
        inventoryReadOnly = inventory.AsReadOnly();
    }

    public override void TransferMouse(Inventory mouse, int slotID)
    {
        if (mouse.inventoryReadOnly[0] == null)
        {
            base.TransferMouse(mouse, slotID);
        }
        else
        {
            ItemAmmo ammo = mouse.inventoryReadOnly[0].item as ItemAmmo;
            if ((ammo != null) && (ammoRestriction == ItemAmmo.AmmoType.None || ammoRestriction == ammo.ammoType))
            {
                base.TransferMouse(mouse, slotID);
            }
        }
    }

    public ItemAmmo ConsumeAmmo()
    {
        inventory[currentSlot].size--;
        ItemAmmo item = inventory[currentSlot].item as ItemAmmo;
        if(inventory[currentSlot].size == 0)
        {
            inventory[currentSlot] = null;
        }
        return item;
    }
}
