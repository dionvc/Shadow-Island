using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    //UI components
    [SerializeField] GameObject otherInventoryPanel;
    [SerializeField] GameObject machinePanel;

    //Raycast variables
    [SerializeField] LayerMask layerMask;
    ContactFilter2D filter;

    //Placement components
    [SerializeField] Grid placeGrid = null;
    [SerializeField] Inventory mouseInventory = null;
    [SerializeField] GameObject placeSprite = null;

    //Mining components/variables
    public Mineable currentMineable { get; private set; } = null;
    Inventory playerInventory = null;
    public int miningProgress {get; private set; } = 0;
    // Start is called before the first frame update
    void Start()
    {
        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = layerMask;
        playerInventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D[] hits = new RaycastHit2D[1];
            Physics2D.Raycast(mousePos2D, Vector2.zero, filter, hits);
            if (mouseInventory.inventoryReadOnly[0] != null && mouseInventory.inventoryReadOnly[0].item.placeableResult != null)
            {
                Vector3 place = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 placeRounded = new Vector3(Mathf.Round(place.x), Mathf.Round(place.y), 0);
                Vector3 placeVector = placeGrid.WorldToCell(placeRounded);
                Vector3 size = mouseInventory.inventoryReadOnly[0].item.placeableResult.GetComponent<BoxCollider2D>().size;
                size = new Vector3(Mathf.Ceil(size.x), Mathf.Ceil(size.y));
                Vector3 offset = new Vector3(0, 0);
                if (Mathf.Round(size.x) % 2 != 0)
                {
                    offset.x = 0.5f;
                }
                if (Mathf.Round(size.y) % 2 != 0)
                {
                    offset.y = 0.5f;
                }
                placeSprite.transform.position = placeVector - offset;
                placeSprite.GetComponent<SpriteRenderer>().sprite = mouseInventory.inventoryReadOnly[0].item.placeableResult.GetComponent<SpriteRenderer>().sprite;
                placeSprite.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.25f);
                if (Input.GetMouseButton(0))
                {
                    
                    if (Physics2D.OverlapBoxAll(placeVector - offset, size / 2.0f, 0.0f).Length == 0)
                    {
                        Instantiate(mouseInventory.inventoryReadOnly[0].item.placeableResult, placeVector - offset, Quaternion.identity);
                    }
                }
            }
            else if(mouseInventory.inventoryReadOnly[0] == null || mouseInventory.inventoryReadOnly[0].item.placeableResult == null)
            {
                placeSprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            }

            if (hits[0].collider != null)
            {
                Debug.Log(hits[0].collider.name);
                #region Left Click Interaction
                if (Input.GetMouseButtonDown(0))
                {
                    Inventory inventory = null;
                    CraftingManagerRecipe craftingManagerRecipe = null;
                    CraftingManagerFixed craftingManagerFixed = null;
                    if (hits[0].collider.gameObject.TryGetComponent<CraftingManagerRecipe>(out craftingManagerRecipe))
                    {
                        machinePanel.GetComponent<MachineUI>().SetLinkedMachine(hits[0].collider.gameObject);
                        otherInventoryPanel.GetComponentInChildren<InventoryUI>().SetViewedInventory(null);
                    }
                    else if (hits[0].collider.gameObject.TryGetComponent<CraftingManagerFixed>(out craftingManagerFixed))
                    {
                        machinePanel.GetComponent<MachineUI>().SetLinkedMachine(hits[0].collider.gameObject);
                        otherInventoryPanel.GetComponentInChildren<InventoryUI>().SetViewedInventory(null);
                    }
                    else if (hits[0].collider.gameObject.TryGetComponent<Inventory>(out inventory))
                    {
                        otherInventoryPanel.GetComponentInChildren<InventoryUI>().SetViewedInventory(inventory);
                        machinePanel.GetComponent<MachineUI>().SetLinkedMachine(null);
                    }
                    else
                    {
                        otherInventoryPanel.GetComponentInChildren<InventoryUI>().SetViewedInventory(null);
                        machinePanel.GetComponent<MachineUI>().SetLinkedMachine(null);
                    }
                }
                #endregion Left Click Interaction

                #region Right Click Interaction
                if(Input.GetMouseButtonDown(1))
                {
                    Mineable mineable;
                    if(hits[0].collider.TryGetComponent(out mineable))
                    {
                        currentMineable = mineable;
                    }
                }

                if (currentMineable != null) {
                    Mineable mineable;
                    if (hits[0].collider.TryGetComponent(out mineable))
                    {
                        if(!ReferenceEquals(currentMineable, mineable))
                        {
                            miningProgress = 0;
                            currentMineable = mineable;
                        }
                    }
                }
                #endregion Right Click Interaction
            }
            else
            {
                currentMineable = null;
            }
        }

        if(currentMineable != null)
        {
            miningProgress++;
            if(miningProgress > currentMineable.miningTime)
            {
                currentMineable.OnMine(playerInventory);
                miningProgress = 0;
            }
        }
    }
}
