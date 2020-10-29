using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject cameraTarget = null;

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

    }
}
