using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GeneratorObjects : MonoBehaviour
{
    Generator generator;
    [SerializeField] Grid grid;
    [SerializeField] Tilemap water;
    [SerializeField] Tilemap cliff;
    [SerializeField] Tilemap tallGrass;
    [SerializeField] List<float> treeValues;
    [SerializeField] List<GameObject> trees;
    [SerializeField] List<GameObject> deadTrees;
    [SerializeField] List<float> oreValues;
    [SerializeField] List<GameObject> ores;

    [SerializeField] GameObject oilPatch;
    [SerializeField] float oilPatchDensity = 4;
    [SerializeField] float oilValue = 0.7f;
    [SerializeField] float oilSeed = 0;

    float[] oreSeeds;
    [SerializeField] float treeDensity;
    ContactFilter2D filter;
    [SerializeField] LayerMask layerMask = 0;
    Collider2D[] colliders;

    Alliance neutralAlliance;

    // Start is called before the first frame update
    void Start()
    {
        generator = GetComponent<Generator>();
        filter = new ContactFilter2D();
        filter.layerMask = layerMask;
        filter.useLayerMask = true;
        filter.useTriggers = false;
        colliders = new Collider2D[1];
        oreSeeds = new float[ores.Count];

        
    }

    // Update is called once per frame
    public void GenerateObjects()
    {
        neutralAlliance = new Alliance("Neutral");
        for (int x = 0; x < generator.sizeX * 32; x++)
        {
            for (int y = 0; y < generator.sizeY * 32; y++)
            {
                for (int i = 0; i < ores.Count; i++)
                {
                    float perlinValue = SamplePerlinOre(i, x, y, generator.sizeX, generator.sizeY);
                    if (!cliff.HasTile(new Vector3Int(x,y,0)) && !water.HasTile(new Vector3Int(x,y,0)) && perlinValue > oreValues[i])
                    {
                        GameObject ore = Instantiate(ores[i], new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity);
                        ore.GetComponent<Mineable>().SetAmount(SamplePerlinOreAmount(i, x, y, generator.sizeX, generator.sizeY, perlinValue));
                        break;
                    }
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
                float value = Mathf.Abs(treeValues[i] - generator.SamplePerlinLerp((int)rounded.x / 32, (int)rounded.y / 32, (int)rounded.x % 32, (int) rounded.y % 32));
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
                        GameObject tree = Instantiate(trees[index], rounded + collider.offset - offset, Quaternion.identity);
                        tree.GetComponent<Health>().alliance = neutralAlliance.allianceCode;
                        tallGrass.SetTile(grid.WorldToCell(rounded + collider.offset - offset), null);
                    }
                    else if (min < 0.3)
                    {
                        GameObject tree = Instantiate(deadTrees[index], rounded + collider.offset - offset, Quaternion.identity);
                        tree.GetComponent<Health>().alliance = neutralAlliance.allianceCode;
                        tallGrass.SetTile(grid.WorldToCell(rounded + collider.offset - offset), null);
                    }
                }
            }
        }
        this.enabled = false;


        PoissonDiscSampler oilSampler = new PoissonDiscSampler(generator.sizeX * 32, generator.sizeY * 32, oilPatchDensity);
        IEnumerable oilSamples = oilSampler.Samples();
        Vector2 oilPatchSize = oilPatch.GetComponent<BoxCollider2D>().size;
        foreach(Vector2 oilSample in oilSamples)
        {
            if(SamplePerlinOil((int)oilSample.x, (int)oilSample.y, generator.sizeX, generator.sizeY) > oilValue)
            {
                Vector2 oilSampleRounded = new Vector2((int)oilSample.x, (int)oilSample.y);
                if(Physics2D.OverlapBox(oilSampleRounded, oilPatchSize, 0) == null)
                {
                    Instantiate(oilPatch, oilSampleRounded, Quaternion.identity);
                    Vector3Int initPos = grid.WorldToCell(oilSampleRounded);
                    for(int i = initPos.x - 1; i < initPos.x + oilPatchSize.x - 1; i++)
                    {
                        for(int j = initPos.y - 1; j < initPos.y + oilPatchSize.y -1; j++)
                        {
                            tallGrass.SetTile(new Vector3Int(i, j, 0), null);
                        }
                    }
                }
            }
        }
    }

    private float SamplePerlinOre(int index, int x, int y, int sizeX, int sizeY)
    {
        if(oreSeeds[index] == 0)
        {
            oreSeeds[index] = Random.value + Random.Range(0, 1024);
        }
        //float attenuation = Mathf.Clamp( 0.5f * (Mathf.Pow((sizeX / 2.0f - x / 32.0f), 2) + Mathf.Pow((sizeY / 2.0f - y / 32.0f), 2)), 0.1f, 1.0f);
        return Mathf.Clamp(Mathf.PerlinNoise(
            (x / (12.0f)) + oreSeeds[index], 
            (y / (12.0f)) + oreSeeds[index])
            , 0, 1.0f);
    }

    private float SamplePerlinOil(int x, int y, int sizeX, int sizeY)
    {
        if(oilSeed == 0)
        {
            oilSeed = Random.value + Random.Range(0, 1024);
        }
        return Mathf.Clamp(Mathf.PerlinNoise(
            x/12.0f + oilSeed,
            y/12.0f + oilSeed)
            ,0, 1.0f);
    }

    /// <summary>
    /// Returns an amount of ore which increases the further from the center of the map
    /// </summary>
    /// <param name="index"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="sizeX"></param>
    /// <param name="sizeY"></param>
    /// <returns></returns>
    private int SamplePerlinOreAmount(int index, int x, int y, int sizeX, int sizeY, float perlinValue)
    {
        float diff = perlinValue - oreValues[index];
        int amount = (int) (diff * 500);
        return (int)Mathf.Pow(amount, 1.5f) + 1;
    }
}
