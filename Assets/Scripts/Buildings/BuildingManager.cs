using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] string miningTag = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool CheckPlacement(Collider2D[] collisionList, out Mineable mineable)
    {
        mineable = null;
        if(collisionList.Length == 1 && collisionList[0] != null)
        {
            if(collisionList[0].gameObject.TryGetComponent(out mineable))
            {
                if(mineable.miningtag == miningTag)
                {
                    return true;
                }
                else
                {
                    mineable = null;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
