using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour, ICameraSubscriber
{
    [SerializeField] MenuSlideOut menuSlideOut;
    Health health;
    [SerializeField] Image healthFill;

    void Start()
    {
        Camera.main.GetComponent<CameraFollow>().SubscribeToTargetChange(this);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(health != null)
        {
            healthFill.fillAmount = health.GetHealthPercentage();
        }
    }

    public void UpdatedCameraTarget(GameObject newTarget)
    {
        newTarget.TryGetComponent(out health);
    }
}
