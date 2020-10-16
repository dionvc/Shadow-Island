using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStack
{
    public int size;
    public Item item;

    public ItemStack(Item item, int size)
    {
        this.item = item;
        this.size = size;
    }
}
