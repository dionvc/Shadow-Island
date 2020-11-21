using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiningProgressUI : MonoBehaviour, ICameraSubscriber
{
    ObjectInteraction objectInteraction;
    [SerializeField] Image fillImage;
    [SerializeField] MenuSlideOut menuSlideOut;
    int waitToHide = 100;
    int counter = 0;

    void Start()
    {
        Camera.main.GetComponent<CameraFollow>().SubscribeToTargetChange(this);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (objectInteraction != null && objectInteraction.currentMineable != null)
        {
            counter = 0;
            menuSlideOut.TogglePanel(MenuSlideOut.PanelState.panelOut);
            fillImage.fillAmount = objectInteraction.miningProgress * 1.0f / objectInteraction.currentMineable.miningTime;
        }
        else
        {
            counter++;
            if (counter > waitToHide)
            {
                menuSlideOut.TogglePanel(MenuSlideOut.PanelState.panelIn);
                counter = 0;
            }
            fillImage.fillAmount = 0;
        }
    }

    public void UpdatedCameraTarget(GameObject newTarget)
    {
        newTarget.TryGetComponent<ObjectInteraction>(out objectInteraction);
    }
}
