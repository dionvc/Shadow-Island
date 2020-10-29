using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorObjects : MonoBehaviour
{
    Generator generator;
    [SerializeField] Grid grid;
    [SerializeField] List<float> treeValues;
    [SerializeField] List<GameObject> trees;
    [SerializeField] List<GameObject> deadTrees;
    [SerializeField] List<float> oreValues;
    [SerializeField] List<GameObject> ores;
    int generateDelay = 0;
    [SerializeField] float oreDensity;
    [SerializeField] float treeDensity;
    ContactFilter2D filter;
    [SerializeField] LayerMask layerMask = 0;
    Collider2D[] colliders;

    // Start is called before the first frame update
    void Start()
    {
        generator = GetComponent<Generator>();
        filter = new ContactFilter2D();
        filter.layerMask = layerMask;
        filter.useLayerMask = true;
        filter.useTriggers = false;
        colliders = new Collider2D[1];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        generateDelay++;
        if (generateDelay == 60) {
            PoissonDiscSampler oreSampler = new PoissonDiscSampler(generator.sizeX * 32, generator.sizeY * 32, oreDensity);
            IEnumerable<Vector2> oreSamples = oreSampler.Samples();
            foreach(Vector2 sample in oreSamples)
            {
                Vector2 rounded = new Vector2Int((int)Mathf.Round(sample.x), (int)Mathf.Round(sample.y));
                float min = 1.0f;
                int index = -1;
                for (int i = 0; i < ores.Count; i++)
                {
                    float value = Mathf.Abs(oreValues[i] - generator.SamplePerlin((int)rounded.x / 32, (int)rounded.y / 32, (int)rounded.x % 32, (int)rounded.y % 32));
                    if (value < min)
                    {
                        min = value;
                        index = i;
                    }
                }
                if (index != -1)
                {
                    BoxCollider2D collider = ores[index].GetComponent<BoxCollider2D>();
                    rounded = (Vector3)grid.WorldToCell(rounded);
                    Vector2 offset = new Vector2(0, 0);
                    Vector3 size = new Vector3(Mathf.Ceil(collider.size.x), Mathf.Ceil(collider.size.y));
                    if (Mathf.Round(size.x) % 2 != 0)
                    {
                        offset.x = 0.5f;
                    }
                    if (Mathf.Round(size.y) % 2 != 0)
                    {
                        offset.y = 0.5f;
                    }
                    if (Physics2D.OverlapBox(rounded + collider.offset - offset, size, 0.0f, filter, colliders) == 0)
                    {
                        Instantiate(ores[index], rounded + collider.offset - offset, Quaternion.identity);
                    }
                }
            }

            PoissonDiscSampler treeSampler = new PoissonDiscSampler(generator.sizeX * 32, generator.sizeY * 32, treeDensity);
            IEnumerable<Vector2> treeSamples = treeSampler.Samples();
            foreach (Vector2 sample in treeSamples)
            {
                Vector2 rounded = new Vector2Int((int)Mathf.Round(sample.x), (int)Mathf.Round(sample.y));
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
                    BoxCollider2D collider = trees[index].GetComponent<BoxCollider2D>();
                    rounded = (Vector3)grid.WorldToCell(rounded);
                    Vector2 offset = new Vector2(0, 0);
                    Vector3 size = new Vector3(Mathf.Ceil(collider.size.x), Mathf.Ceil(collider.size.y));
                    if (Mathf.Round(size.x) % 2 != 0)
                    {
                        offset.x = 0.5f;
                    }
                    if (Mathf.Round(size.y) % 2 != 0)
                    {
                        offset.y = 0.5f;
                    }
                    if (Physics2D.OverlapBox(rounded + collider.offset - offset, size, 0.0f, filter, colliders) == 0)
                    {
                        if (min < 0.15)
                        {
                            Instantiate(trees[index], rounded + collider.offset - offset, Quaternion.identity);
                        }
                        else if (min < 0.3)
                        {
                            Instantiate(deadTrees[index], rounded + collider.offset - offset, Quaternion.identity);
                        }
                    }
                }
            }
            this.enabled = false;
        }
    }
}
