using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFormation : MonoBehaviour
{
    List<GenericAI> units;
    GameObject formationTarget;
    PathNode pathToTarget;

    public void SetGroupTarget(GameObject target)
    {
        formationTarget = target;
        if (units.Count > 0) {
            Vector3 combinedPosition = Vector3.zero;
            for(int i = 0; i < units.Count; i++)
            {
                combinedPosition += units[i].transform.position;
            }
            combinedPosition = (combinedPosition / units.Count);
            if (Pathing.Instance.GetLocationPathValue((int)combinedPosition.x, (int)combinedPosition.y) == -1)
            {
                combinedPosition = units[0].transform.position;
            }
            pathToTarget = Pathing.Instance.GetPath(combinedPosition, target.transform.position, 1000, units[0].GetAgentSize());
        }
        for(int i = 0; i < units.Count; i++)
        {
            units[i].SetFormationPath(pathToTarget);
        }
    }

    public void RegisterUnit(GenericAI unit)
    {
        if(units == null)
        {
            units = new List<GenericAI>();
        }
        units.Add(unit);
    }

}
