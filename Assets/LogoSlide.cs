using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoSlide : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MenuSlideOut>().TogglePanel(MenuSlideOut.PanelState.panelOut);
    }
}
