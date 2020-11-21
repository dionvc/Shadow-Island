using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineable : MonoBehaviour
{
    [SerializeField] int itemIDresult;
    [SerializeField] int amountOnMine;
    [SerializeField] int totalAmount;
    [SerializeField] public int miningTime;
    [SerializeField] public string miningtag = null;
    [SerializeField] int[] values;
    [SerializeField] Sprite[] sprites;
    int currentProgression = 0;
    SpriteRenderer spriteRenderer;

    /// <summary>
    /// Returns true if the mining results could be inserted
    /// </summary>
    /// <param name="inventory"></param>
    /// <returns></returns>
    public bool OnMine(Inventory inventory)
    {
        if(values != null && currentProgression < values.Length - 1 && totalAmount - amountOnMine < values[currentProgression+1])
        {
            currentProgression++;
            spriteRenderer.sprite = sprites[currentProgression];
        }
        if (amountOnMine >= totalAmount)
        {
            if (inventory.InsertStack(new ItemStack(Definitions.Instance.ItemDictionary[itemIDresult], totalAmount)) == null)
            {
                Destroy(this.gameObject);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (inventory.InsertStack(new ItemStack(Definitions.Instance.ItemDictionary[itemIDresult], amountOnMine)) == null)
            {
                totalAmount -= amountOnMine;
                return true;
            }
            return false;
        }
    }

    public void SetAmount(int amount)
    {
        this.totalAmount = amount;
        if (values != null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > amount)
                {
                    currentProgression = i;
                    spriteRenderer.sprite = sprites[currentProgression];
                }
            }
        }
    }
}
