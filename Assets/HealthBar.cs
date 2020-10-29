using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    MaterialPropertyBlock healthBlock;
    SpriteRenderer srenderer;

    void Start()
    {
        healthBlock = new MaterialPropertyBlock();
        srenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateBar(float value)
    {
        if(value > 0.99f)
        {
            srenderer.enabled = false;
        }
        else
        {
            srenderer.enabled = true;
            srenderer.GetPropertyBlock(healthBlock);
            healthBlock.SetFloat("_Fill", value);
            srenderer.SetPropertyBlock(healthBlock);
        }
    }
}
