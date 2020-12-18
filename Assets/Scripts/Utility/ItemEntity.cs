using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : MonoBehaviour
{
    public Item item { get; private set; }
    SpriteRenderer spriteRenderer;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void UpdateItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.itemSprite;
    }
    public void ConsumeItem()
    {
        ItemEntityPool.Instance.StashItemEntity(this);
    }
}
