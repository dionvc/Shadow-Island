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

    public bool CheckPlacement(List<Collider2D> collisionList, out List<Mineable> mineableList)
    {
        mineableList = new List<Mineable>();

        for (int i = 0; i < collisionList.Count; i++) {
            Mineable mineable;
            if (collisionList[i].gameObject.TryGetComponent(out mineable))
            {
                if (mineable.miningtag == miningTag)
                {
                    mineableList.Add(mineable);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        if(mineableList.Count > 0)
        {
            return true;
        }
        return false;
    }
}
