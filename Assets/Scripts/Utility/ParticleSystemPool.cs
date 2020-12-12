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
        acidCloud, //unimplemented
        wood, //unimplemented
        leaf, //unimplemented
        grass,
        dust, //unimplemented
        blood, //unimplemented
        machine,
        gunFlare,
        bulletCase, //unimplemented
        bulletHit //unimplemented
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
            particleSystemDictionary[pair.type] = Instantiate(pair.system, Vector3.zero, Quaternion.identity);
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
        emitParams.position = new Vector3(location.x, location.y, 0);
        particleSystemDictionary[type].Emit(emitParams, quantity);
    }

    public void EmitParticle(ParticleType type, Vector2 location, int quantity, bool retainShape)
    {
        if (quantity <= 0)
        {
            return;
        }
        if (type == ParticleType.none)
        {
            return;
        }
        emitParams.position = new Vector3(location.x, location.y, 0);
        emitParams.applyShapeToPosition = retainShape;
        particleSystemDictionary[type].Emit(emitParams, quantity);
        emitParams.applyShapeToPosition = false;
    }

    public void EmitParticle(ParticleType type, Vector2 location, int quantity, bool retainShape, Color color)
    {
        if (quantity <= 0)
        {
            return;
        }
        if (type == ParticleType.none)
        {
            return;
        }
        emitParams.position = new Vector3(location.x, location.y, 0);
        emitParams.startColor = color;
        emitParams.applyShapeToPosition = retainShape;
        particleSystemDictionary[type].Emit(emitParams, quantity);
        emitParams.applyShapeToPosition = false;
        emitParams.startColor = Color.white;
    }

    public void EmitParticle(ParticleType type, Vector2 location, float rotation, int quantity)
    {
        if (quantity <= 0)
        {
            return;
        }
        if (type == ParticleType.none)
        {
            return;
        }
        emitParams.position = location;
        emitParams.rotation = rotation;
        particleSystemDictionary[type].Emit(emitParams, quantity);
        emitParams.rotation = 0.0f;
    }
}
