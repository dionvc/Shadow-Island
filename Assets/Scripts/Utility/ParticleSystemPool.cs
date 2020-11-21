using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPool : MonoBehaviour
{
    public enum ParticleType
    {
        none,
        smallExplosion,
        mediumExplosion,
        iceCrystal,
        poisonCloud,
        wood,
        leaf,
        grass,
        dust,
        blood
    }

    [System.Serializable]
    class ParticleSystemPair {
        public ParticleType type;
        public ParticleSystem system;
    }

    ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
    [SerializeField] List<ParticleSystemPair> pairList;
    Dictionary<ParticleType, ParticleSystem> particleSystemDictionary;
    #region Singleton Code
    private static ParticleSystemPool instance;
    public static ParticleSystemPool Instance
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

    public void Start()
    {
        particleSystemDictionary = new Dictionary<ParticleType, ParticleSystem>();
        foreach(ParticleSystemPair pair in pairList)
        {
            particleSystemDictionary[pair.type] = pair.system;
        }
    }

    public void EmitParticle(ParticleType type, Vector2 location, int quantity)
    {
        if(quantity <= 0)
        {
            return;
        }
        if(type == ParticleType.none)
        {
            return;
        }
        emitParams.position = location;
        particleSystemDictionary[type].Emit(emitParams, quantity);
    }
}
