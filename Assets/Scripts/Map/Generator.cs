using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Generator : MonoBehaviour
{
    [SerializeField] Grid tileGrid;
    [SerializeField] Tilemap[] maps;
    [SerializeField] RuleTile[] tiles;
    [SerializeField] Tilemap cliffMap;
    [SerializeField] RuleTile cliffTile;
    [SerializeField] Tile tallGrass;
    [SerializeField] Tilemap tallGrassMap;
    float seed = 0;
    public int sizeX = 12;
    public int sizeY = 12;

    // Start is called before the first frame update

    void Start()
    {
        seed = Random.value;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                GenerateTerrain(x, y);
                GenerateCliffs(x, y);
            }
        }
        CoverEdges();
        GenerateGrass();
        InitializePathing();
    }

    void GenerateCliffs(int x, int y)
    {
        for(int j = 0; j < 32; j++)
        {
            if (j < 12 || j > 18)
            {
                if (!maps[0].HasTile(new Vector3Int(x * 32, j + y * 32, 0))) {
                    cliffMap.SetTile(new Vector3Int(0 + x * 32, j + y * 32, 0), cliffTile);
                }
                if (!maps[0].HasTile(new Vector3Int(31 + x * 32, j + y * 32, 0)))
                {
                    cliffMap.SetTile(new Vector3Int(31 + x * 32, j + y * 32, 0), cliffTile);
                }
                if (!maps[0].HasTile(new Vector3Int(j + x * 32, 0 + y * 32, 0)))
                {
                    cliffMap.SetTile(new Vector3Int(j + x * 32, 0 + y * 32, 0), cliffTile);
                }
                if (!maps[0].HasTile(new Vector3Int(j + x * 32, 31 + y * 32, 0)))
                {
                    cliffMap.SetTile(new Vector3Int(j + x * 32, 31 + y * 32, 0), cliffTile);
                }
            }

        }
    }

    void GenerateTerrain(int x, int y)
    {
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                int num = (int)SamplePerlin(x, y, i, j);
                maps[num].SetTile(new Vector3Int(x * 32 + i, y * 32 + j, 0), tiles[num]);
            }
        }
    }

    /// <summary>
    /// This function literally covers the edges in the tilemap.  Due to the design of perlin noise and unity rule tiles, there are gaps where the tiles do
    /// not overlap, this function fixes that in two passes
    /// 1. it finds areas where there is a tile that has a lower level tile as a neighbor, and places both the lower level and higher level tile in a 3x3 box at that spot
    /// 2. it goes over and finds areas where water is now overlapped by 9 upper level tiles, and deletes the water (as it is no longer visible)
    /// </summary>
    void CoverEdges()
    {
        //First pass, fixing gaps between layers
        List<KeyValuePair<int, Vector3Int>> update = new List<KeyValuePair<int, Vector3Int>>();
        Vector3Int[] locations = new Vector3Int[9];
        for (int i = 0; i < 32 * sizeX; i++)
        {
            for (int j = 0; j < 32 * sizeY; j++)
            {

                locations[0] = new Vector3Int(i - 1, j - 1, 0);
                locations[1] = new Vector3Int(i - 1, j, 0);
                locations[2] = new Vector3Int(i - 1, j + 1, 0);
                locations[3] = new Vector3Int(i, j - 1, 0);
                locations[4] = new Vector3Int(i, j, 0);
                locations[5] = new Vector3Int(i, j + 1, 0);
                locations[6] = new Vector3Int(i + 1, j - 1, 0);
                locations[7] = new Vector3Int(i + 1, j, 0);
                locations[8] = new Vector3Int(i + 1, j + 1, 0);


                for (int type = 1; type < maps.Length; type++)
                {
                    if (maps[type].HasTile(locations[4]) && CheckNeighborsOne(maps[type - 1], locations))
                    {
                        for (int s = 0; s < 9; s++)
                        {
                            update.Add(new KeyValuePair<int, Vector3Int>(type, locations[s]));
                        }
                        if(type - 1 != 0)
                        {
                            for (int s = 0; s < 9; s++)
                            {
                                update.Add(new KeyValuePair<int, Vector3Int>(type - 1, locations[s]));
                            }
                        }
                    }
                }
            }
        }
        

        foreach (KeyValuePair<int, Vector3Int> kvp in update)
        {
            maps[kvp.Key].SetTile(kvp.Value, tiles[kvp.Key]);
        }

        //Second pass, clearing out water that is covered
        for (int i = 0; i < 32 * sizeX; i++)
        {
            for (int j = 0; j < 32 * sizeY; j++)
            {
                locations[0] = new Vector3Int(i - 1, j - 1, 0);
                locations[1] = new Vector3Int(i - 1, j, 0);
                locations[2] = new Vector3Int(i - 1, j + 1, 0);
                locations[3] = new Vector3Int(i, j - 1, 0);
                locations[4] = new Vector3Int(i, j, 0);
                locations[5] = new Vector3Int(i, j + 1, 0);
                locations[6] = new Vector3Int(i + 1, j - 1, 0);
                locations[7] = new Vector3Int(i + 1, j, 0);
                locations[8] = new Vector3Int(i + 1, j + 1, 0);

                if (maps[0].HasTile(locations[4]) && GetNeighborsCount(maps[1], locations) == 9)
                {
                    maps[0].SetTile(locations[4], null);
                }
                if (maps[0].HasTile(locations[4]))
                {
                    maps[1].SetTile(locations[4], tiles[1]);
                }
            }
        }
    }

    void GenerateGrass()
    {
        Vector3Int[] locations = new Vector3Int[9];
        for (int i = 0; i < 32 * sizeX; i++)
        {
            for (int j = 0; j < 32 * sizeY; j++)
            {
                locations[0] = new Vector3Int(i - 1, j - 1, 0);
                locations[1] = new Vector3Int(i - 1, j, 0);
                locations[2] = new Vector3Int(i - 1, j + 1, 0);
                locations[3] = new Vector3Int(i, j - 1, 0);
                locations[4] = new Vector3Int(i, j, 0);
                locations[5] = new Vector3Int(i, j + 1, 0);
                locations[6] = new Vector3Int(i + 1, j - 1, 0);
                locations[7] = new Vector3Int(i + 1, j, 0);
                locations[8] = new Vector3Int(i + 1, j + 1, 0);
                float perlin = SamplePerlin(i / 32, j / 32, i % 32, j % 32);
                if (!cliffMap.HasTile(locations[4]) && GetNeighborsCount(maps[3], locations) == 9 && !maps[4].HasTile(locations[4])
                    && perlin < 3.6 && perlin > 3.2)
                {
                    tallGrassMap.SetTile(locations[4], tallGrass);
                }
            }
        }
    }

    bool CheckNeighborsOne(Tilemap map, Vector3Int[] locations)
    {
        if (map.HasTile(locations[0]) ||
            map.HasTile(locations[1]) ||
            map.HasTile(locations[2]) ||
            map.HasTile(locations[3]) ||
            map.HasTile(locations[5]) ||
            map.HasTile(locations[6]) ||
            map.HasTile(locations[7]) ||
            map.HasTile(locations[8])
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    int GetNeighborsCount(Tilemap map, Vector3Int[] locations)
    {
        int neighbors = 0;
        for(int i = 0; i < 9; i++)
        {
            if(map.HasTile(locations[i]))
            {
                neighbors++;
            }
        }
        return neighbors;
    }

    /// <summary>
    /// Returns a float between 0 and the number of tile variants
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public float SamplePerlin(int x, int y, int i, int j)
    {
        if (x == 0 || y == 0 || x == sizeX - 1 || y == sizeY - 1)
        {
            return 0;
        }
        int x0 = 0;
        int y0 = 0;
        int yM = 0;
        int xM = 0;
        int xy0 = 0;
        int x0yM = 0;
        int xMy0 = 0;
        int xMyM = 0;
        int xyR = 0;
        if (x == 1 && y == 1)
        {
            xy0 = 1;
        }
        else if (x == 1 && y == sizeY - 2)
        {
            x0yM = 1;
        }
        else if (x == sizeX - 2 && y == 1)
        {
            xMy0 = 1;
        }
        else if (x == sizeX - 2 && y == sizeY - 2)
        {
            xMyM = 1;
        }
        else if (x == 1)
        {
            x0 = 1;
        }
        else if (y == 1)
        {
            y0 = 1;
        }
        else if (x == sizeX - 2)
        {
            xM = 1;
        }
        else if (y == sizeY - 2)
        {
            yM = 1;
        }
        else
        {
            xyR = 1;
        }
        float perlin = Mathf.Clamp(0.33f * Mathf.PerlinNoise(((x * 32 + i) / 4.0f) + seed, ((y * 32 + j) / 4.0f) + seed) + 0.67f * Mathf.PerlinNoise(((x * 32 + i) / 8.0f) + seed, ((y * 32 + j) / 8.0f) + seed), 0, 0.99f);
        perlin = Mathf.Pow(perlin, 0.5f);
        float num = ((maps.Length) * perlin *
                        (
                        Mathf.Clamp(
                            x0 * (i / 18.0f) + y0 * (j / 18.0f) + xM * ((31 - i) / 18.0f) + yM * ((31 - j) / 18.0f) +
                            xy0 * (3 * (i * j) / (2 * 961.0f)) +
                            x0yM * (3 * (i * (31 - j) / (2 * 961.0f))) +
                            xMy0 * (3 * ((31 - i) * j / (2 * 961.0f))) +
                            xMyM * (3 * ((31 - i) * (31 - j) / (2 * 961.0f))) +
                            xyR
                            , 0, 0.99f)
                        ));
        return num;
    }

    /// <summary>
    /// Initializes the pathing grid with unwalkable areas according to generated map
    /// </summary>
    private void InitializePathing()
    {
        Pathing.Instance.InitializePathingGrid(sizeX * 32, sizeY * 32);
        for(int i = 0; i < sizeX * 32; i++)
        {
            for(int j = 0; j < sizeY * 32; j++)
            {
                Vector3Int location = new Vector3Int(i, j, 0);
                if(cliffMap.HasTile(location) || maps[0].HasTile(location))
                {
                    Pathing.Instance.SetUnpathableLocation(i, j);
                }
            }
        }
    }
}
