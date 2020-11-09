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
    [SerializeField] LayerMask layerMaskMining;
    ContactFilter2D filter;
    ContactFilter2D filterMining;

    List<Collider2D> collidersList = new List<Collider2D>(10);
    Collider2D[] colliders = new Collider2D[1];

    //Placement components
    [SerializeField] Grid placeGrid = null;
    [SerializeField] Inventory mouseInventory = null;
    [SerializeField] GameObject placeSprite = null;
    [SerializeField] SpriteRenderer placeSpriteRenderer;

    //Mining components/variables
    public Mineable currentMineable { get; private set; } = null;
    Inventory playerInventory = null;
    public int miningProgress {get; private set; } = 0;
    // Start is called before the first frame update
    void Start()
    {
        filterMining = new ContactFilter2D();
        filterMining.useLayerMask = true;
        filterMining.layerMask = layerMaskMining;
        filterMining.useTriggers = true;

        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = layerMask;
        filter.useTriggers = false;

        playerInventory = GetComponent<Inventory>();
        placeSpriteRenderer = placeSprite.GetComponent<SpriteRenderer>();
        //Debug code
        Definitions definitions = Definitions.Instance;
        foreach (KeyValuePair<int,Item> item in Definitions.Instance.ItemDictionary)
        {
            playerInventory.InsertStack(new ItemStack(item.Value, 5));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D[] hits = new RaycastHit2D[1];
            Physics2D.Raycast(mousePos2D, Vector2.zero, filterMining, hits);

            if (mouseInventory.inventoryReadOnly[0] != null)
            {
                ItemThrowable throwable = mouseInventory.inventoryReadOnly[0].item as ItemThrowable;
                if (throwable != null && Input.GetMouseButtonDown(0))
                {
                    Vector3 place = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    ThrownItem thrown = Instantiate(throwable.thrownResult, this.transform.position, Quaternion.identity).GetComponent<ThrownItem>();
                    thrown.SetTarget(place);
                    mouseInventory.DecrementStack(0);
                }
                else if (mouseInventory.inventoryReadOnly[0].item.placeableResult != null)
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
                    placeSpriteRenderer.sprite = mouseInventory.inventoryReadOnly[0].item.placeableResult.GetComponent<SpriteRenderer>().sprite;

                    
                    BuildingManager buildingManager;

                    if (mouseInventory.inventoryReadOnly[0].item.placeableResult.TryGetComponent(out buildingManager))
                    {
                        Physics2D.OverlapBox(placeVector - offset, size / 2.0f, 0.0f, filterMining, collidersList);
                        List<Mineable> mineables;
                        if (buildingManager.CheckPlacement(collidersList, out mineables))
                        {
                            placeSpriteRenderer.color = new Color(0, 1, 0, 0.25f);
                            if (Input.GetMouseButton(0))
                            {
                                GameObject building = Instantiate(mouseInventory.inventoryReadOnly[0].item.placeableResult, placeVector - offset, Quaternion.identity);
                                building.GetComponent<CraftingManagerFixed>().SetMineables(mineables);
                                mouseInventory.DecrementStack(0);
                            }
                        }
                        else
                        {
                            placeSpriteRenderer.color = new Color(1, 0, 0, 0.25f);
                        }
                    }
                    else if (Physics2D.OverlapBox(placeVector - offset, size / 2.0f, 0.0f, filter, colliders) == 0)
                    {
                        placeSpriteRenderer.color = new Color(0, 1, 0, 0.25f);
                        if (Input.GetMouseButton(0))
                        {
                            Instantiate(mouseInventory.inventoryReadOnly[0].item.placeableResult, placeVector - offset, Quaternion.identity);
                            mouseInventory.DecrementStack(0);
                        }
                    }
                    else
                    {
                        placeSpriteRenderer.color = new Color(1, 0, 0, 0.25f);
                    }
                }
                else if (mouseInventory.inventoryReadOnly[0].item.placeableResult == null)
                {
                    placeSpriteRenderer.color = new Color(1, 1, 1, 0);
                }
            }
            else
            {
                placeSpriteRenderer.color = new Color(1, 1, 1, 0);
            }

            if (hits[0].collider != null)
            {
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
        else
        {
            placeSpriteRenderer.color = new Color(1, 1, 1, 0);
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
