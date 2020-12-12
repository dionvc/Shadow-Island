using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightSlotUI : MonoBehaviour
{
    [SerializeField] Color highlightColor;
    [SerializeField] Color regularColor;
    int previousindex = 0;
    public void SetHighlightedSlot(int index)
    {
        this.transform.GetChild(previousindex).gameObject.GetComponent<Image>().color = regularColor;
        int children = this.transform.childCount;
        if(index >= children)
        {
            return;
        }
        else
        {
            this.transform.GetChild(index).gameObject.GetComponent<Image>().color = highlightColor;
            previousindex = index;
        }
    }
}
