using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorObjects : MonoBehaviour
{
    Generator generator;
    PoissonDiscSampler rockSampler;
    [SerializeField] GameObject generatable = null;
    [SerializeField] KeyValuePair<int, GameObject> generateables;
    [SerializeField] Tilemap dirtMap = null;
    int generateDelay = 0;

    // Start is called before the first frame update
    void Start()
    {
        generator = GetComponent<Generator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //generateDelay++;
        /*if(generateDelay > 60)
        {
            rockSampler = new PoissonDiscSampler(12 * 32, 12 * 32, 10);
            Vector3 size = generatable.GetComponent<BoxCollider2D>().size;
            foreach (Vector2 sample in rockSampler.Samples())
            {
                Vector2 alignedSample = new Vector2(Mathf.RoundToInt(sample.x), Mathf.RoundToInt(sample.y));
                if (Physics2D.OverlapBoxAll(alignedSample, size, 0.0f).Length == 0)
                {
                    Instantiate(generatable, alignedSample, Quaternion.identity);
                }
            }
            this.enabled = false;
        }*/
    }
}
