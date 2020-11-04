using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour
{
    int time = 0;
    int timeMax = 15000; //full time of day in ticks (50 ticks per second)
    float timeCurve = 0;
    float sinTimeCurve = 0;
    float sunAngle = 0;
    [SerializeField] Gradient ambientGradient;
    Light globalLight;
    #region Singleton code
    private static TimeKeeper instance;
    public static TimeKeeper Instance
    {
        get
        {
            if (instance == null) Debug.LogError("No Instance of Definitions in the Scene!");
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Just one Instance of Definitions allowed!");
        }
    }
#endregion
    void Start()
    {
        globalLight = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time++;
        if(time > timeMax)
        {
            time = 0;
        }
        timeCurve = (Mathf.Sin(-Mathf.PI / 2 + 2 * Mathf.PI * time / timeMax) + 1)/2;
        globalLight.color = ambientGradient.Evaluate(time * 1.0f / timeMax);
        if (time > timeMax / 5.0f && time < timeMax * 4.0f / 5.0f)
        {
            globalLight.transform.eulerAngles = new Vector3(30, -50.0f + 100.0f * (((time - timeMax/5.0f) * (1.25f) / timeMax)), 0);
        }
        else if(time <= timeMax / 5.0f)
        {
            globalLight.transform.eulerAngles = new Vector3(30, -50.0f * (5.0f * time / timeMax), 0);
        }
        else
        {
            globalLight.transform.eulerAngles = new Vector3(30, 50 - 50.0f * (1.0f * time / (4.0f * timeMax)), 0);
        }
        sunAngle = Mathf.Atan2(globalLight.transform.forward.y, globalLight.transform.forward.x) - Mathf.PI/2;
    }

    public float GetAlphaPoint()
    {
        return 0.9f - 0.9f * timeCurve;
    }

    public float GetAlphaSun()
    {
        return timeCurve;
    }

    public float GetSunAngle()
    {
        return sunAngle;
    }

    public float GetTimeNormalized()
    {
        return time * 1.0f / timeMax;
    }

    /// <summary>
    /// If the number is between 0 and 1 it is day, if it is between 1 and 2 it is night
    /// </summary>
    /// <returns></returns>
    public float GetTimeDayNormalized()
    {
        if (time >= timeMax / 4)
        {
            return (time - timeMax / 4.0f) * 2.0f / timeMax;
        }
        else
        {
            return 1.5f + time * 2.0f / timeMax;
        }
    }
}
