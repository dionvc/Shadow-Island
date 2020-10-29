using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class LUTswap : MonoBehaviour
{
    ColorGrading lutEffect;
    PostProcessVolume globalVolume;
    [SerializeField] Texture[] luts;
    int currentLut = 0;
    int nextLut = 1;

    // Start is called before the first frame update
    void Start()
    {
        globalVolume = GetComponent<PostProcessVolume>();
        globalVolume.profile.TryGetSettings(out lutEffect);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
