using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSlideOut : MonoBehaviour
{
    public enum PanelState
    {
        panelOut,
        panelIn
    }
    [SerializeField] Vector3 outTarget;
    [SerializeField] Vector3 inTarget;
    [SerializeField] string slideOutKey = null;
    [SerializeField] MenuSlideOut[] otherMenusToHide = null;
    [SerializeField] bool popOutOnStart = false;
    RectTransform panelTransform;
    PanelState panelState = PanelState.panelIn;
    Vector3 currentTarget;
    Vector3 start;
    float percentage = 0.0f;
    float percentageIncrement = 0.1f;
    Coroutine coroutine = null;

    void Start()
    {
        panelTransform = GetComponent<RectTransform>();
        if(popOutOnStart)
        {
            this.TogglePanel(PanelState.panelOut);
        }
    }
    IEnumerator Slide()
    {
        while (percentage < 1.0f)
        {
            percentage += percentageIncrement;
            panelTransform.anchoredPosition = Vector3.Lerp(start, currentTarget, percentage);
            yield return new WaitForFixedUpdate();
        }
        panelTransform.anchoredPosition = currentTarget;
        percentage = 0.0f;
        coroutine = null;
        yield break;
    }

    public void Update()
    {
        if(slideOutKey != null && slideOutKey != "" && Input.GetKeyDown(PlayerData.Instance.keybinds[slideOutKey]))
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        if (panelState == PanelState.panelIn)
        {
            panelState = PanelState.panelOut;
            start = inTarget;
            currentTarget = outTarget;
            coroutine = StartCoroutine("Slide");
            ToggleOtherMenusOff();

        }
        else if (panelState == PanelState.panelOut)
        {
            TogglePanelOff();
        }

    }

    public void TogglePanel(PanelState state)
    {
        if(state == panelState || panelTransform == null)
        {
            return;
        }
        else
        {
            if (state == PanelState.panelOut)
            {
                panelState = PanelState.panelOut;
                start = inTarget;
                currentTarget = outTarget;
                coroutine = StartCoroutine("Slide");
                ToggleOtherMenusOff();
            }
            else if (state == PanelState.panelIn)
            {
                TogglePanelOff();
            }
        }
    }

    public void TogglePanelOff()
    {
        panelState = PanelState.panelIn;
        start = outTarget;
        currentTarget = inTarget;
        coroutine = StartCoroutine("Slide");
        ToggleOtherMenusOff();
    }

    public void ToggleOtherMenusOff()
    {
        if (otherMenusToHide != null)
        {
            for (int i = 0; i < otherMenusToHide.Length; i++)
            {
                otherMenusToHide[i].TogglePanelOff();
            }
        }
    }
}
