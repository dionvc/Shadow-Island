using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGatherer : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer[] renderers;
    TimeKeeper timeKeeper;
    Light sun;
    List<Collider2D> results;
    GameObject tk;
    [SerializeField] LayerMask layerMask;
    ContactFilter2D filter;
    void Start()
    {
        tk = GameObject.FindGameObjectWithTag("Time");
        timeKeeper = tk.GetComponent<TimeKeeper>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
        sun = tk.GetComponentInChildren<Light>();
        filter = new ContactFilter2D();
        filter.layerMask = layerMask;
        filter.useLayerMask = true;
        filter.useDepth = false;
        filter.useTriggers = true;
        results = new List<Collider2D>();
        Debug.Log(renderers[0].bounds);
        renderers[0].allowOcclusionWhenDynamic = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (renderers[0].isVisible) {
            renderers[0].material.SetFloat("_Alpha", timeKeeper.GetAlphaSun());
            renderers[0].material.SetFloat("_Angle", timeKeeper.GetSunAngle());

            if (results.Count > 0)
            {
                Vector3 lightToObject = this.transform.position - results[0].gameObject.transform.position;
                
                renderers[1].material.SetFloat("_Angle", (Mathf.Atan2(lightToObject.y, lightToObject.x) - Mathf.PI / 2));
                renderers[1].material.SetFloat("_Alpha", timeKeeper.GetAlphaPoint());
            }
            if (results.Count > 1)
            {
                Vector3 lightToObject = this.transform.position - results[1].gameObject.transform.position;
                renderers[2].material.SetFloat("_Angle", (Mathf.Atan2(lightToObject.y, lightToObject.x) - Mathf.PI / 2));
                renderers[2].material.SetFloat("_Alpha", timeKeeper.GetAlphaPoint());
            }
        }
    }

    void FixedUpdate()
    {
        results.Clear();
        int count = Physics2D.OverlapCircle(this.transform.position, 15, filter, results);
    }
}
