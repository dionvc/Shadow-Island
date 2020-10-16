using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchUpdater : MonoBehaviour
{
    [SerializeField] RectTransform hourHand;
    [SerializeField] RectTransform minuteHand;
    TimeKeeper timekeeper;
    // Start is called before the first frame update
    void Start()
    {
        timekeeper = GameObject.FindGameObjectWithTag("Time").GetComponent<TimeKeeper>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hourHand.eulerAngles = new Vector3(0, 0, -timekeeper.GetTimeNormalized() * 360 * 2);
        minuteHand.eulerAngles = new Vector3(0, 0, -timekeeper.GetTimeNormalized() * 12 * 360 * 2);
    }
}
