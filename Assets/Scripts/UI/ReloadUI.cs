using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadUI : MonoBehaviour, ICameraSubscriber
{
    [SerializeField] Image fillImage;
    ObjectInteraction objectInteraction;
    [SerializeField] MenuSlideOut menuSlideOut;
    int waitToHide = 180;
    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.GetComponent<CameraFollow>().SubscribeToTargetChange(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (objectInteraction != null && objectInteraction.gameObject != null && objectInteraction.GetReloadProgress() < 0.99f)
        {
            counter = 0;
            menuSlideOut.TogglePanel(MenuSlideOut.PanelState.panelOut);
            fillImage.fillAmount = objectInteraction.GetReloadProgress();
        }
        else
        {
            counter++;
            if (counter > waitToHide)
            {
                menuSlideOut.TogglePanel(MenuSlideOut.PanelState.panelIn);
                counter = 0;
            }
            fillImage.fillAmount = 1.0f;
        }
    }

    public void UpdatedCameraTarget(GameObject newTarget)
    {
        newTarget.TryGetComponent(out objectInteraction);
    }
}
