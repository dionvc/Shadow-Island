using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeProgressBar : MonoBehaviour
{
    [SerializeField] Sprite dayBackground = null;
    [SerializeField] Sprite nightBackground = null;
    [SerializeField] Image dayFill = null;
    [SerializeField] Image nightFill = null;
    Image back = null;
    // Start is called before the first frame update
    void Start()
    {
        back = GetComponent<Image>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float normTime = TimeKeeper.Instance.GetTimeDayNormalized();
        if (normTime < 1.0f)
        {
            back.sprite = dayBackground;
            dayFill.fillAmount = normTime;
            nightFill.fillAmount = 0.0f;
        }
        else
        {
            back.sprite = nightBackground;
            dayFill.fillAmount = 0.0f;
            nightFill.fillAmount = normTime - 1.0f;
        }
    }
}
