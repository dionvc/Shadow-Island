using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineable : MonoBehaviour
{
    [SerializeField] int itemIDresult;
    [SerializeField] int amountOnMine;
    [SerializeField] int totalAmount;
    [SerializeField] public int miningTime;

    public void OnMine(Inventory inventory)
    {
        if (amountOnMine >= totalAmount)
        {
            Destroy(this.gameObject);
            inventory.InsertStack(new ItemStack(Definitions.Instance.ItemDefinitions[itemIDresult], totalAmount));
        }
        else
        {
            totalAmount -= amountOnMine;
            inventory.InsertStack(new ItemStack(Definitions.Instance.ItemDefinitions[itemIDresult], amountOnMine));
        }
    }
}
