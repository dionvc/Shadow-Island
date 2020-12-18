using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFilterItem : MonoBehaviour
{
    [SerializeField] GameObject filterItemPrefab;
    [SerializeField] GameObject itemPanel;
    [SerializeField] GameObject filterOutputTogglePrefab;
    [SerializeField] GameObject filterStatePanel;
    [SerializeField] ToggleGroup filterToggleGroup;
    [SerializeField] GameObject setFilterHighlightPrefab;
    GameObject filterHighlight;
    List<GameObject> filterItemList;
    List<Toggle> filterStateList;
    IItemFilter filterEntity;
    int itemId;
    // Start is called before the first frame update
    void Start()
    {
        filterHighlight = Instantiate(setFilterHighlightPrefab, this.transform, false);
        filterHighlight.SetActive(false);
        filterItemList = new List<GameObject>();
        filterStateList = new List<Toggle>();
        foreach (KeyValuePair<int, Item> kvp in Definitions.Instance.ItemDictionary)
        {
            GameObject filterItem = Instantiate(filterItemPrefab, itemPanel.transform, false);
            filterItem.transform.GetChild(0).GetComponent<Image>().sprite = kvp.Value.itemSprite;
            InventorySlot invSlot = filterItem.GetComponent<InventorySlot>();
            invSlot.item = kvp.Value;
            invSlot.slotID = filterItemList.Count;
            filterItemList.Add(filterItem);
            filterItem.GetComponent<Button>().onClick.AddListener(delegate { OnClickFilterSlot(filterItem); } );
        }
    }

    public void SetFilterFocus(IItemFilter filterEntity)
    {
        filterHighlight.SetActive(false);
        this.filterEntity = filterEntity;
        if (filterEntity != null)
        {
            itemId = filterEntity.GetFilterItem();
            string[] filterNames = filterEntity.GetFilterNames();
            for (int i = 0; i < filterStateList.Count; i++)
            {
                filterToggleGroup.UnregisterToggle(filterStateList[i]);
                Destroy(filterStateList[i].gameObject);
            }
            filterStateList.Clear();
            if (filterNames.Length > 1)
            {
                
                for (int i = 0; i < filterNames.Length; i++)
                {
                    GameObject filterStateButton = Instantiate(filterOutputTogglePrefab, filterStatePanel.transform, false);
                    Toggle toggle = filterStateButton.GetComponent<Toggle>();
                    filterStateList.Add(toggle);
                    filterStateButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = filterNames[i];
                    toggle.group = filterToggleGroup;
                    if(filterEntity.GetFilter() == filterNames[i])
                    {
                        toggle.isOn = true;
                    }
                    else
                    {
                        toggle.isOn = false;
                    }
                }
                for(int i = 0; i < filterStateList.Count; i++)
                {
                    filterStateList[i].onValueChanged.AddListener(delegate { OnClickToggle(); });
                }
            }
            else
            {
                GameObject toggleOnOff = Instantiate(filterOutputTogglePrefab, filterStatePanel.transform, false);
                Toggle temp = toggleOnOff.GetComponent<Toggle>();
                filterStateList.Add(temp);
                toggleOnOff.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Enabled";
                temp.group = filterToggleGroup;
                string filterString = filterEntity.GetFilter();
                if (filterString == "Enabled")
                {
                    temp.isOn = true;
                }
                else
                {
                    temp.isOn = false;
                }
                temp.onValueChanged.AddListener(delegate { OnClickToggle(); });
            }
            if(itemId != -1)
            {
                for(int i = 0; i < filterItemList.Count; i++)
                {
                    if(filterItemList[i].GetComponent<InventorySlot>().item.id == itemId)
                    {
                        filterHighlight.SetActive(true);
                        filterHighlight.transform.SetParent(filterItemList[i].transform, false);
                        filterHighlight.transform.position = filterItemList[i].transform.position;
                    }
                }
            }
            
            GetComponent<MenuSlideOut>().TogglePanel(MenuSlideOut.PanelState.panelOut);
        }
        else
        {
            GetComponent<MenuSlideOut>().TogglePanel(MenuSlideOut.PanelState.panelIn);
        }
    }

    void OnClickFilterSlot(GameObject filterSlot)
    {
        if(filterEntity != null)
        {
            itemId = filterSlot.GetComponent<InventorySlot>().item.id;
            if (!filterToggleGroup.AnyTogglesOn() || itemId == -1)
            {
                filterEntity.SetFilter("None", -1);
                filterHighlight.SetActive(false);
            }
            else
            {
                foreach(Toggle toggle in filterToggleGroup.ActiveToggles())
                {
                    filterEntity.SetFilter(toggle.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text, itemId);
                }
                filterHighlight.SetActive(true);
                filterHighlight.transform.SetParent(filterSlot.transform, false);
                filterHighlight.transform.position = filterSlot.transform.position;
            }
        }
    }

    void OnClickToggle()
    {
        if(filterEntity != null)
        {
            if (!filterToggleGroup.AnyTogglesOn() || itemId == -1)
            {
                filterEntity.SetFilter("None", -1);
                filterHighlight.SetActive(false);
            }
            else
            {
                if(itemId == -1)
                {
                    itemId = 0;
                    filterHighlight.SetActive(true);
                    filterHighlight.transform.SetParent(filterItemList[0].transform, false);
                    filterHighlight.transform.position = filterItemList[0].transform.position;
                }
                foreach (Toggle toggle in filterToggleGroup.ActiveToggles())
                {
                    filterEntity.SetFilter(toggle.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text, itemId);
                }
            }
        }
    }
}
