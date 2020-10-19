using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorObjects : MonoBehaviour
{
    Generator generator;
    [SerializeField] Tilemap cliffMap;
    [SerializeField] Tilemap waterMap;
    [SerializeField] List<float> treeValues;
    [SerializeField] List<GameObject> trees;
    [SerializeField] List<GameObject> deadTrees;
    [SerializeField] List<float> oreValues;
    [SerializeField] List<GameObject> ores;
    int generateDelay = 0;
    [SerializeField] float oreDensity;
    [SerializeField] float treeDensity;

    // Start is called before the first frame update
    void Start()
    {
        generator = GetComponent<Generator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        generateDelay++;
        if (generateDelay == 60) {
            PoissonDiscSampler treeSampler = new PoissonDiscSampler(generator.sizeX * 32, generator.sizeY * 32, treeDensity);
            IEnumerable<Vector2> treeSamples = treeSampler.Samples();
            foreach (Vector2 sample in treeSamples)
            {
                Vector2 rounded = new Vector2Int((int)Mathf.Round(sample.x), (int)Mathf.Round(sample.y));
                if (!waterMap.HasTile(new Vector3Int((int)rounded.x, (int)rounded.y, 0)) && !cliffMap.HasTile(new Vector3Int((int)rounded.x, (int)rounded.y, 0)))
                {


                    float min = 1.0f;
                    int index = -1;
                    for (int i = 0; i < trees.Count; i++)
                    {
                        float value = Mathf.Abs(treeValues[i] - generator.SamplePerlin((int)rounded.x / 32, (int)rounded.y / 32, (int)rounded.x % 32, (int) rounded.y % 32));
                        if (value < min)
                        {
                            min = value;
                            index = i;
                        }
                    }
                    if (index != -1)
                    {
                        if (min < 0.15)
                        {
                            Instantiate(trees[index], (Vector3)rounded, Quaternion.identity);
                        }
                        else if (min < 0.3)
                        {
                            Instantiate(deadTrees[index], (Vector3)rounded, Quaternion.identity);
                        }
                    }
                }
            }
            this.enabled = false;
        }
    }
}
