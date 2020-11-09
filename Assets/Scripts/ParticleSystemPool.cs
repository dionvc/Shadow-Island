using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPool : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion = null;
    ParticleSystem.EmitParams explosionParams = new ParticleSystem.EmitParams();
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

    public void EmitExplosion(Vector2 location)
    {
        explosionParams.position = location;
        explosion.Emit(explosionParams, 1);
    }
}
