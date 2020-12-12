using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntityPool : MonoBehaviour
{
    Queue<ItemEntity> disabledItemEntities;
    [SerializeField] int initQueueSize = 1000;
    [SerializeField] GameObject itemEntityPrefab;
    public static ItemEntityPool Instance { get; private set; }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        disabledItemEntities = new Queue<ItemEntity>(initQueueSize);
    }

    public void CreateItemEntity(Vector2 position, Item item)
    {
        if(disabledItemEntities.Count == 0)
        {
            Instantiate(itemEntityPrefab, position, Quaternion.identity).GetComponent<ItemEntity>().UpdateItem(item);
            
        }
        else
        {
            ItemEntity itemEntity = disabledItemEntities.Dequeue();
            itemEntity.gameObject.SetActive(true);
            itemEntity.UpdateItem(item);
            itemEntity.transform.position = position;
        }
    }

    public void StashItemEntity(ItemEntity item)
    {
        disabledItemEntities.Enqueue(item);
        item.gameObject.SetActive(false);
    }
}
