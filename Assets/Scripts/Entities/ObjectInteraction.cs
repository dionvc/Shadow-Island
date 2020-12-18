using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Basically does everything interaction-wise with the game (interact with objects, communicate with UI, mine items, fire weapons).
/// whoops.  Later I can try to divide functionality.  At least it doesn't also do movement.
/// </summary>
public class ObjectInteraction : MonoBehaviour
{
    //UI components
    [SerializeField] GameObject inventoryPanelInteractPrefab;
    [SerializeField] GameObject machinePanelInteractPrefab;
    [SerializeField] GameObject inventoryPanelPrefab;
    [SerializeField] GameObject craftingPanelPrefab;
    [SerializeField] GameObject mouseInventoryPrefab;
    [SerializeField] GameObject ammoInventoryPrefab;
    [SerializeField] GameObject tooltipPanelPrefab;
    [SerializeField] GameObject respawnPanelPrefab;
    [SerializeField] GameObject filterItemPanelPrefab;
    MachineUI machineUI;
    InventoryUI inventoryInteractUI;
    InventoryUI inventoryUI;
    InventoryUI inventoryAmmoUI;
    CraftingUI craftingUI;
    HighlightSlotUI ammoSlotSelected;
    UIFilterItem filterItemUI;

    GameObject machinePanelInteract;
    GameObject inventoryInteract;
    GameObject inventoryPanel;
    GameObject craftingPanel;
    GameObject inventoryAmmo;
    GameObject mouseInventoryUI;
    GameObject tooltip;
    GameObject filterItemPanel;

    GameObject canvas;

    //Raycast variables
    [SerializeField] LayerMask layerMask;
    [SerializeField] LayerMask layerMaskMining;
    ContactFilter2D filter;
    ContactFilter2D filterMining;

    List<Collider2D> collidersList = new List<Collider2D>(10);
    Collider2D[] colliders = new Collider2D[1];

    //Placement components
    Grid placeGrid = null;
    Tilemap tallGrassMap = null;
    [SerializeField] Inventory mouseInventory = null;
    [SerializeField] GameObject placeSpritePrefab = null;
    GameObject placeSprite = null;
    SpriteRenderer placeSpriteRenderer;
    int placementRotation = 0;

    //Mining components/variables
    public Mineable currentMineable { get; private set; } = null;
    Inventory playerInventory = null;
    AmmoSlot ammoInventory = null;
    int selectedAmmoSlot = 0;
    public int miningProgress {get; private set; } = 0;
    bool fireWeapon = false;
    [SerializeField] int firingSpeed = 10; //number of fixedupdates before player can fire again
    int firingTimer = 0;


    Alliance characterAlliance;
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

        placeSprite = Instantiate(placeSpritePrefab, Vector3.zero, Quaternion.identity);
        Inventory[] inventories = GetComponents<Inventory>();
        for (int i = 0; i < inventories.Length; i++) 
        {
            if (inventories[i] is AmmoSlot)
            {
                ammoInventory = inventories[i] as AmmoSlot;
            }
            else
            {
                playerInventory = inventories[i];
            }
        }

        playerInventory = GetComponent<Inventory>();
        placeSpriteRenderer = placeSprite.GetComponent<SpriteRenderer>();
        Camera.main.GetComponent<CameraFollow>().SetCameraTarget(this.gameObject);

        placeGrid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        canvas = GameObject.Find("Canvas");
        machinePanelInteract = Instantiate(machinePanelInteractPrefab, canvas.transform);
        inventoryInteract = Instantiate(inventoryPanelInteractPrefab, canvas.transform);
        inventoryPanel = Instantiate(inventoryPanelPrefab, canvas.transform);
        craftingPanel = Instantiate(craftingPanelPrefab, canvas.transform);
        inventoryAmmo = Instantiate(ammoInventoryPrefab, canvas.transform);
        mouseInventoryUI = Instantiate(mouseInventoryPrefab, canvas.transform);
        tooltip = Instantiate(tooltipPanelPrefab, canvas.transform);
        filterItemPanel = Instantiate(filterItemPanelPrefab, canvas.transform);

        mouseInventoryUI.GetComponent<InventoryMouse>().SetInventory(mouseInventory);
        machineUI = machinePanelInteract.GetComponentInChildren<MachineUI>();
        inventoryInteractUI = inventoryInteract.GetComponentInChildren<InventoryUI>();
        inventoryUI = inventoryPanel.GetComponentInChildren<InventoryUI>();
        craftingUI = craftingPanel.GetComponentInChildren<CraftingUI>();
        inventoryAmmoUI = inventoryAmmo.GetComponentInChildren<InventoryUI>();
        ammoSlotSelected = inventoryAmmo.GetComponentInChildren<HighlightSlotUI>();
        filterItemUI = filterItemPanel.GetComponent<UIFilterItem>();

        inventoryAmmoUI.SetViewedInventory(ammoInventory);
        inventoryAmmoUI.SetMouseInventory(mouseInventory);
        inventoryUI.SetViewedInventory(playerInventory);
        inventoryUI.SetMouseInventory(mouseInventory);
        inventoryInteractUI.SetMouseInventory(mouseInventory);
        craftingUI.SetLinkedInventory(playerInventory);
        machineUI.SetMouseInventory(mouseInventory);



        characterAlliance = AllianceDefinitions.Instance.GetAlliance("Character");
        this.gameObject.GetComponent<Health>().alliance = characterAlliance.allianceCode;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            DebugInventory();
        }

        //Drop item
        if(Input.GetKeyDown(PlayerData.Instance.keybinds["Drop Item"]) && mouseInventory.inventoryReadOnly[0] != null)
        {
            ItemEntityPool.Instance.CreateItemEntity(Camera.main.ScreenToWorldPoint(Input.mousePosition), mouseInventory.inventoryReadOnly[0].item);
            mouseInventory.DecrementStack(0);
        }

        //Fire weapon
        if(Input.GetKey(PlayerData.Instance.keybinds["Fire Weapon"]))
        {
            fireWeapon = true;
        }

        if(Input.GetKeyDown(PlayerData.Instance.keybinds["Rotate Buildable"]))
        {
            placementRotation--;
            if(placementRotation < 0)
            {
                placementRotation = 7;
            }
        }

        
        if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D[] hits = new RaycastHit2D[1];
            Physics2D.Raycast(mousePos2D, Vector2.zero, filterMining, hits);

            if (mouseInventory.inventoryReadOnly[0] != null)
            {
                ItemThrowable throwable = mouseInventory.inventoryReadOnly[0].item as ItemThrowable;
                if (throwable != null && Input.GetMouseButtonDown(0)) //thrown item code
                {
                    Vector3 place = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    ThrownItem thrown = Instantiate(throwable.thrownResult, this.transform.position, Quaternion.identity).GetComponent<ThrownItem>();
                    thrown.SetTarget(place);
                    mouseInventory.DecrementStack(0);
                }
                else if (mouseInventory.inventoryReadOnly[0].item.placeableResult != null) //place building code
                {
                    Vector2 size;
                    IRotate rotateable;
                    if (mouseInventory.inventoryReadOnly[0].item.placeableResult.TryGetComponent(out rotateable))
                    {
                        placeSpriteRenderer.sprite = rotateable.GetRotatedPlaceSprite(placementRotation);
                        size = rotateable.GetRotatedBoundingBox(placementRotation);
                    }
                    else
                    {
                        placeSpriteRenderer.sprite = mouseInventory.inventoryReadOnly[0].item.placeableResult.GetComponent<SpriteRenderer>().sprite;
                        size = mouseInventory.inventoryReadOnly[0].item.placeableResult.GetComponent<BoxCollider2D>().size;
                    }

                    Vector3 place = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 placeRounded = new Vector3(Mathf.Round(place.x), Mathf.Round(place.y), 0);
                    Vector3 placeVector = placeGrid.WorldToCell(placeRounded);
                    size = new Vector2(Mathf.CeilToInt(size.x), Mathf.CeilToInt(size.y));
                    Vector3 offset = new Vector3(0, 0);
                    if (size.x % 2 != 0)
                    {
                        offset.x = 0.5f;
                    }
                    if (size.y % 2 != 0)
                    {
                        offset.y = 0.5f;
                    }

                    placeSprite.transform.position = placeVector - offset;
                    
                    

                    BuildingManager buildingManager;


                    if (rotateable != null && Physics2D.OverlapBox(placeVector - offset, size - new Vector2(0.1f, 0.1f), 0.0f, filter, colliders) == 0)
                    {
                        rotateable.GetRotatedPlaceSprite(placementRotation);
                        placeSpriteRenderer.color = new Color(0, 1, 0, 0.25f);
                        if (Input.GetMouseButton(0))
                        {
                            GameObject building = Instantiate(mouseInventory.inventoryReadOnly[0].item.placeableResult, placeVector - offset, Quaternion.identity);
                            building.GetComponent<Health>().alliance = characterAlliance.allianceCode;
                            building.GetComponent<IRotate>().InitializeRotated(placementRotation);
                            DestroyGrassAt(placeVector - offset, size);
                            mouseInventory.DecrementStack(0);
                            UpdateLoaders(building.transform.position, size);
                        }
                    }
                    else if (mouseInventory.inventoryReadOnly[0].item.placeableResult.TryGetComponent(out buildingManager))
                    {
                        Physics2D.OverlapBox(placeVector - offset, size - new Vector2(0.1f, 0.1f), 0.0f, filterMining, collidersList);
                        List<Mineable> mineables;
                        if (buildingManager.CheckPlacement(collidersList, out mineables, placeVector - offset))
                        {
                            placeSpriteRenderer.color = new Color(0, 1, 0, 0.25f);
                            if (Input.GetMouseButton(0))
                            {
                                GameObject building = Instantiate(mouseInventory.inventoryReadOnly[0].item.placeableResult, placeVector - offset, Quaternion.identity);
                                building.GetComponent<CraftingManagerFixed>().SetMineables(mineables);
                                building.GetComponent<Health>().alliance = characterAlliance.allianceCode;
                                DestroyGrassAt(placeVector - offset, size);
                                mouseInventory.DecrementStack(0);
                                UpdateLoaders(building.transform.position, size);
                            }
                        }
                        else
                        {
                            placeSpriteRenderer.color = new Color(1, 0, 0, 0.25f);
                        }
                    }
                    else if (Physics2D.OverlapBox(placeVector - offset, size - new Vector2(0.1f, 0.1f), 0.0f, filter, colliders) == 0)
                    {
                        placeSpriteRenderer.color = new Color(0, 1, 0, 0.25f);
                        if (Input.GetMouseButton(0))
                        {
                            GameObject building = Instantiate(mouseInventory.inventoryReadOnly[0].item.placeableResult, placeVector - offset, Quaternion.identity);
                            building.GetComponent<Health>().alliance = characterAlliance.allianceCode;
                            DestroyGrassAt(placeVector - offset, size);
                            mouseInventory.DecrementStack(0);
                            UpdateLoaders(building.transform.position, size);
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
                    IItemFilter itemFilter = null;
                    if (hits[0].collider.gameObject.TryGetComponent<CraftingManagerRecipe>(out craftingManagerRecipe))
                    {
                        filterItemUI.SetFilterFocus(null);
                        machineUI.SetLinkedMachine(hits[0].collider.gameObject);
                        inventoryInteractUI.SetViewedInventory(null);
                    }
                    else if (hits[0].collider.gameObject.TryGetComponent<CraftingManagerFixed>(out craftingManagerFixed))
                    {
                        filterItemUI.SetFilterFocus(null);
                        machineUI.SetLinkedMachine(hits[0].collider.gameObject);
                        inventoryInteractUI.SetViewedInventory(null);
                    }
                    else if (hits[0].collider.gameObject.TryGetComponent<Inventory>(out inventory))
                    {
                        filterItemUI.SetFilterFocus(null);
                        inventoryInteractUI.SetViewedInventory(inventory);
                        machineUI.SetLinkedMachine(null);
                    }
                    else if (hits[0].collider.gameObject.TryGetComponent<IItemFilter>(out itemFilter))
                    {
                        filterItemUI.SetFilterFocus(itemFilter);
                        machineUI.SetLinkedMachine(null);
                        inventoryInteractUI.SetViewedInventory(null);
                    }
                    else
                    {
                        inventoryInteractUI.SetViewedInventory(null);
                        machineUI.SetLinkedMachine(null);
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

        if(Input.GetKeyDown(PlayerData.Instance.keybinds["Change Ammo"]))
        {
            selectedAmmoSlot++;
            if(selectedAmmoSlot >= ammoInventory.inventoryReadOnly.Count)
            {
                selectedAmmoSlot = 0;
            }
            ammoSlotSelected.SetHighlightedSlot(selectedAmmoSlot);
        }
    }

    /// <summary>
    /// Position of new building
    /// size of new building
    /// </summary>
    /// <param name="position"></param>
    /// <param name="size"></param>
    void UpdateLoaders(Vector2 position, Vector2 size)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = 1 << LayerMask.NameToLayer("ConveyorBelt");
        Collider2D[] elements = new Collider2D[(int)(size.x + size.x + size.y + size.y + 4)];//lol fuck if this works
        int loaderCount = Physics2D.OverlapBox(position, size + new Vector2(1, 1), 0, filter, elements);
        Loader temp;
        for(int i = 0; i < loaderCount; i++)
        {
            if(elements[i].TryGetComponent(out temp))
            {
                temp.CheckForInventory();
            }
        }
    }

    void FixedUpdate()
    {
        if (currentMineable != null)
        {
            miningProgress++;
            if (miningProgress > currentMineable.miningTime)
            {
                currentMineable.OnMine(playerInventory);
                miningProgress = 0;
            }
        }

        if (firingTimer < firingSpeed)
        {
            firingTimer++;
        }
        if (fireWeapon == true && firingTimer == firingSpeed)
        {
            if (ammoInventory.inventoryReadOnly[selectedAmmoSlot] != null)
            {
                ItemAmmo ammo = ammoInventory.inventoryReadOnly[selectedAmmoSlot].item as ItemAmmo;
                if (ammo != null)
                {
                    ammo.OnFire(Camera.main.ScreenToWorldPoint(Input.mousePosition), this.gameObject);
                    ammoInventory.DecrementStack(selectedAmmoSlot);
                    firingTimer = 0;
                }
            }
            
        }
        fireWeapon = false;
    }

    void DestroyGrassAt(Vector2 center, Vector2 size) 
    {
        if (tallGrassMap == null)
        {
            int childCount = placeGrid.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (placeGrid.transform.GetChild(i).name == "TallGrassMap")
                {
                    tallGrassMap = placeGrid.transform.GetChild(i).gameObject.GetComponent<Tilemap>();
                }
            }
        }
        int iBound = Mathf.CeilToInt(center.x + (size.x / 2.0f));
        int jBound = Mathf.CeilToInt(center.y + (size.y / 2.0f));
        for (int i = (int)(center.x - (size.x/2.0f)); i < iBound; i++)
        {
            for (int j =  (int)(center.y - (size.x/2.0f)); j < jBound; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                if (tallGrassMap.HasTile(pos))
                {
                    ParticleSystemPool.Instance.EmitParticle(ParticleSystemPool.ParticleType.grass, center, Random.Range(2, 10), true);
                }
                tallGrassMap.SetTile(pos, null);
                
            }
        }
    }

    public void DebugInventory()
    {
        //Debug code
        Definitions definitions = Definitions.Instance;
        foreach (KeyValuePair<int, Item> item in Definitions.Instance.ItemDictionary)
        {
            playerInventory.InsertStack(new ItemStack(item.Value, item.Value.maxStack));
        }
    }

    void OnDestroy()
    {
        Destroy(machinePanelInteract);
        Destroy(inventoryInteract);
        Destroy(inventoryPanel);
        Destroy(craftingPanel);
        Destroy(inventoryAmmo);
        Destroy(mouseInventoryUI);
        Destroy(tooltip);
        Destroy(filterItemPanel);
        if (respawnPanelPrefab != null && canvas != null)
        {
            GameObject respawnPanel = Instantiate(respawnPanelPrefab, canvas.transform, false);
            respawnPanel.GetComponent<DeathPanelUI>().GiveAlliance(characterAlliance);
        }
    }

    public float GetReloadProgress()
    {
        return ((firingTimer * 1.0f) / firingSpeed);
    }
}
