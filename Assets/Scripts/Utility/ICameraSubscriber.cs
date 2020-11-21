using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraSubscriber
{
    void UpdatedCameraTarget(GameObject newTarget);
}
