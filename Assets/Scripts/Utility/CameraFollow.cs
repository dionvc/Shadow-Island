using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject cameraTarget = null;
    List<ICameraSubscriber> subscribers;

    // Update is called once per frame
    void Update()
    {
        if (cameraTarget != null)
        {
            this.transform.position = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y, -10);
        }
    }

    public void SetCameraTarget(GameObject target)
    {
        this.cameraTarget = target;
        for(int i = 0; i < subscribers.Count; i++)
        {
            //Remove subscribers if they were deleted
            if (subscribers[i] == null)
            {
                subscribers.RemoveAt(i);
                i--;
            }
            else
            {
                //update its target if its still around
                subscribers[i].UpdatedCameraTarget(target);
            }
        }
    }

    public GameObject GetCameraTarget()
    {
        return cameraTarget;
    }

    /// <summary>
    /// Subscribes a subscriber to camera target changes.  This is mostly for UI components, so that they know if the player just spawned, or perhaps died (in which case it is null),
    /// or potentially in the case of spectating something else.
    /// </summary>
    /// <param name="subscriber"></param>
    public void SubscribeToTargetChange(ICameraSubscriber subscriber)
    {
        if(subscribers == null)
        {
            subscribers = new List<ICameraSubscriber>();
        }
        subscribers.Add(subscriber);
    }
}
