using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// This inventory is designed for "discovered" recipes where there is a fixed number of inputs, and the recipe is determined from at most two inputs,
/// and produces only a single item
/// </summary>
public class InventoryFixed : Inventory
{
    [SerializeField] int inputSize;
    [SerializeField] int outputSize;
    private List<ItemStack> inputInventory;
    private List<ItemStack> outputInventory;
    public ReadOnlyCollection<ItemStack> inputInvReadOnly { get; private set; }
    public ReadOnlyCollection<ItemStack> outputInvReadOnly { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        inputInventory = new List<ItemStack>();
        outputInventory = new List<ItemStack>();
        for(int i = 0; i < inputSize; i++)
        {
            inputInventory.Add(null);
        }
        for(int i = 0; i < outputSize; i++)
        {
            outputInventory.Add(null);
        }
        inputInvReadOnly = inputInventory.AsReadOnly();
        outputInvReadOnly = outputInventory.AsReadOnly();
    }

    public override void TransferMouse(Inventory mouse, int slotID)
    {
        if (mouse.inventoryReadOnly[0] == null)
        {
            if (slotID < inputInventory.Count)
            {
                mouse.InsertAt(0, inputInventory[slotID]);
                inputInventory[slotID] = null;
            }
            else
            {
                mouse.InsertAt(0, outputInventory[slotID - inputInventory.Count]);
                outputInventory[slotID - inputInventory.Count] = null;
            }
        }
        else if (mouse.inventoryReadOnly[0] != null && slotID < inputInventory.Count)
        {
            if (inputInventory[slotID] == null)
            {
                inputInventory[slotID] = new ItemStack(mouse.inventoryReadOnly[0].item, 0);
            }
            inputInventory[slotID].size += mouse.inventoryReadOnly[0].size;
            if (inputInventory[slotID].size > inputInventory[slotID].item.maxStack)
            {
                mouse.ChangeStackCount(0, inputInventory[slotID].size - inputInventory[slotID].item.maxStack);
                inputInventory[slotID].size = inputInventory[slotID].item.maxStack;
            }
            else
            {
                mouse.InsertAt(0, null);
            }
            //Cannot insert anything into results, only take from
        }
    }

    public override bool ConsumeItems(List<Ingredient> ingredients)
    {
        return false;
    }

    public override bool InsertItems(List<Ingredient> results)
    {
        return false;
    }

    public override ItemStack InsertStack(ItemStack itemStack)
    {
        for(int i = 0; i < outputInventory.Count; i++)
        {
            if(outputInventory[i] != null && outputInventory[i].item.id == itemStack.item.id)
            {
                outputInventory[i].size += itemStack.size;
                itemStack.size = outputInventory[i].size - outputInventory[i].item.maxStack;
                if(itemStack.size <= 0)
                {
                    return null;
                }
            }
            else if(outputInventory[i] == null)
            {
                outputInventory[i] = itemStack;
                return null;
            }
        }
        return itemStack;
    }

    public override ItemEntity GetOutputItemEntity(Vector2 position, int filter)
    {
        for (int i = 0; i < outputInventory.Count; i++)
        {
            if (outputInventory[i] != null && (filter == -1 || outputInventory[i].item.id == filter))
            {
                outputInventory[i].size--;
                Item item = outputInventory[i].item;
                if (outputInventory[i].size == 0)
                {
                    outputInventory[i] = null;
                }
                return ItemEntityPool.Instance.CreateItemEntity(position, item);
            }
        }
        return null;
    }

    /// <summary>
    /// TODO: probably broken
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public override bool TryInsertItem(Item item)
    {
        for (int i = 0; i < inputInventory.Count; i++)
        {
            if ((inputInventory[i] == null || (inputInventory[i].size < item.maxStack && inputInventory[i].item.id == item.id)))
            {
                if (inputInventory[i] == null)
                {
                    inputInventory[i] = new ItemStack(item, 0);
                }
                inputInventory[i].size++;
                return true;
            }
        }
        return false;
    }
}
