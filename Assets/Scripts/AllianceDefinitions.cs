using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alliance
{
    public int allianceCode; //the number identifying the alliance (same as the number that is stored in health)
    public List<int> hostileTowards; //an array of alliance codes the alliance will attack
    public string allianceName; //the name of the alliance

    public Alliance(string allianceName)
    {
        this.allianceName = allianceName;
        this.allianceCode = AllianceDefinitions.Instance.GetNewAllianceCode();
        AllianceDefinitions.Instance.AddAlliance(this);
    }

    public void SetHostileTowards(int code)
    {
        hostileTowards.Add(code);
    }
}

public class AllianceDefinitions : MonoBehaviour
{

    #region Singleton Code
    private static AllianceDefinitions instance;
    public static AllianceDefinitions Instance
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
    public Dictionary<int, Alliance> AllianceDefintionsIndexed;
    public Dictionary<string, Alliance> AllianceDefinitionsName;
    int currentAllianceCode = 0;

    void Start()
    {
        AllianceDefinitionsName = new Dictionary<string, Alliance>();
        AllianceDefintionsIndexed = new Dictionary<int, Alliance>();
    }
    /// <summary>
    /// Returns a currently unreserved alliancecode.  Calling this will reserve the alliance code for the caller.
    /// </summary>
    /// <returns></returns>
    public int GetNewAllianceCode()
    {
        currentAllianceCode++;
        return currentAllianceCode - 1;
    }

    public void AddAlliance(Alliance alliance)
    {
        AllianceDefintionsIndexed[alliance.allianceCode] = alliance;
        AllianceDefinitionsName[alliance.allianceName] = alliance;
    }

    /// <summary>
    /// Returns an alliance corresponding to the code.  May return null.
    /// </summary>
    /// <param name="alliance"></param>
    /// <returns></returns>
    public Alliance GetAlliance(int allianceCode)
    {
        Alliance alliance;
        if(AllianceDefintionsIndexed.TryGetValue(allianceCode, out alliance))
        {
            return alliance;
        }
        return null;
    }

    /// <summary>
    /// Returns the alliance corresponding to the name.  May return null.
    /// </summary>
    /// <param name="name"></param>
    public Alliance GetAlliance(string name)
    {
        Alliance alliance;
        if(AllianceDefinitionsName.TryGetValue(name, out alliance))
        {
            return alliance;
        }
        return null;
    }
}
