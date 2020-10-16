using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class LUTswap : MonoBehaviour
{
    [SerializeField] Gradient gradientTest;
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
        /*
        if(currentLut != (int)(time * luts.Length / maxTime)) {
            currentLut++;
            nextLut++;
            if(currentLut >= luts.Length)
            {
                currentLut = 0;
            }
            if(nextLut >= luts.Length)
            {
                nextLut = 0;
            }
            lutEffect.ldrLut.value = luts[currentLut];
            //lutEffect.ldrLut.Interp(lutEffect.ldrLut.value, luts[nextLut], 0.001f);
        }
        */
    }
}
