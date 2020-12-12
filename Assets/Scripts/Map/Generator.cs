using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Generator : MonoBehaviour
{
    public struct LandNode
    {
        public enum LandNodeType
        {
            topLeftCorner = 0,
            top = 1,
            topRightCorner = 2,

            left = 3,
            fill = 4,
            right = 5,

            bottomLeftCorner = 6,
            bottom = 7,
            bottomRightCorner = 8,

            bottomLeftInsideCorner = 9,
            topLeftInsideCorner = 10,
            bottomRightInsideCorner = 11,
            topRightInsideCorner = 12,
            empty = 13
        }
        public LandNodeType landNodeType;
        public float topLeft;
        public float top;
        public float topRight;
        public float left;
        public float right;
        public float bottomLeft;
        public float bottom;
        public float bottomRight;
        public float center;
        public LandNode(int topLeftTile, int topTile, int topRightTile, int leftTile, int rightTile, int bottomLeftTile, int bottomTile, int bottomRightTile)
        {
            #region corners
            if (topLeftTile == 0 || topTile == 0 || leftTile == 0)
            {
                topLeft = 0;
            }
            else
            {
                topLeft = 1;
            }
            if (topRightTile == 0 || topTile == 0 || rightTile == 0)
            {
                topRight = 0;
            }
            else
            {
                topRight = 1;
            }
            if (bottomLeftTile == 0 || bottomTile == 0 || leftTile == 0)
            {
                bottomLeft = 0;
            }
            else
            {
                bottomLeft = 1;
            }
            if (bottomRightTile == 0 || bottomTile == 0 || rightTile == 0)
            {
                bottomRight = 0;
            }
            else
            {
                bottomRight = 1;
            }
            #endregion corners
            #region mids
            if(topLeft == 0 || topRight == 0)
            {
                top = 0.5f * topTile;
            }
            else
            {
                top = topTile;
            }

            if(bottomLeft == 0 || bottomRight == 0)
            {
                bottom = 0.5f * bottomTile;
            }
            else
            {
                bottom = bottomTile;
            }

            if(bottomLeft == 0 || topLeft == 0)
            {
                left = 0.5f * leftTile;
            }
            else
            {
                left = leftTile;
            }

            if(bottomRight == 0 || topRight == 0)
            {
                right = 0.5f * rightTile;
            }
            else
            {
                right = rightTile;
            }

            center = (top + bottom + left + right) / 4.0f;
            #endregion mids

            #region type determination
            if(topLeftTile == 1 && topTile == 1 && topRightTile == 1 && 
                leftTile == 1 && rightTile == 1 && 
                bottomLeftTile == 1 && bottomTile == 1 && bottomRightTile == 1)
            {
                landNodeType = LandNodeType.fill;
            }
            else if(topLeftTile == 0 && topTile == 0 && topRightTile == 0 &&
                leftTile == 0 && rightTile == 0 &&
                bottomLeftTile == 0 && bottomTile == 0 && bottomRightTile == 0)
            {
                landNodeType = LandNodeType.empty;
            }
            else if (topTile == 1 &&
                leftTile == 0 && rightTile == 1 &&
                bottomTile == 1)
            {
                landNodeType = LandNodeType.left;
            }
            else if (topTile == 1 &&
                leftTile == 1 && rightTile == 0 &&
                bottomTile == 1)
            {
                landNodeType = LandNodeType.right;
            }
            else if (topTile == 0 &&
                leftTile == 1 && rightTile == 1 &&
                bottomTile == 1)
            {
                landNodeType = LandNodeType.top;
            }
            else if (topTile == 1 &&
                leftTile == 1 && rightTile == 1 &&
                bottomTile == 0)
            {
                landNodeType = LandNodeType.bottom;
            }
            else if (topLeftTile == 0 && topTile == 0 &&
                leftTile == 0 && rightTile == 1 &&
                bottomTile == 1)
            {
                landNodeType = LandNodeType.topLeftCorner;
            }
            else if (topTile == 0 && topRightTile == 0 &&
                leftTile == 1 && rightTile == 0 &&
                bottomTile == 1)
            {
                landNodeType = LandNodeType.topRightCorner;
            }
            else if (topTile == 1 &&
                leftTile == 0 && rightTile == 1 &&
                bottomLeftTile == 0 && bottomTile == 0)
            {
                landNodeType = LandNodeType.bottomLeftCorner;
            }
            else if (topTile == 1 &&
                leftTile == 1 && rightTile == 0 &&
                 bottomTile == 0 && bottomRightTile == 0)
            {
                landNodeType = LandNodeType.bottomRightCorner;
            }
            else if (bottomLeftTile == 0 &&
                leftTile == 1 &&
                 bottomTile == 1)
            {
                landNodeType = LandNodeType.bottomLeftInsideCorner;
            }
            else if (rightTile == 1 &&
                 bottomTile == 1 && bottomRightTile == 0)
            {
                landNodeType = LandNodeType.bottomRightInsideCorner;
            }
            else if (topTile == 1 &&
                leftTile == 1 && topLeftTile == 0)
            {
                landNodeType = LandNodeType.topLeftInsideCorner;
            }
            else if (topTile == 1 &&
                rightTile == 1 && topRightTile == 0)
            {
                landNodeType = LandNodeType.topRightInsideCorner;
            }
            else
            {
                landNodeType = LandNodeType.empty;
            }
            #endregion type determination
        }
    }
    [SerializeField] Grid tileGrid;
    [SerializeField] Tilemap[] maps;
    [SerializeField] RuleTile[] tiles;
    [SerializeField] Tilemap cliffMap;
    [SerializeField] RuleTile cliffTile;
    [SerializeField] Tile tallGrass;
    [SerializeField] Tilemap tallGrassMap;
    float seed = 0;
    int[][] isLand;
    LandNode[][] landNodes;
    [SerializeField] public int sizeX { get; private set; } = 12;
    [SerializeField] public int sizeY { get; private set; } = 12;
    public float progress = 0.0f;
    public float colliderProgress = 0.0f;

    // Start is called before the first frame update

    public void GenerateIsland()
    {
        if (isLand == null)
        {
            GenerateIslandShape(12);
        }
        StartCoroutine("GenerateChunks");
    }

    IEnumerator GenerateChunks()
    {
        seed = Random.value;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                GenerateTerrain(x, y);
                GenerateCliffs(x, y);
                progress = (x * sizeY + y) / (sizeX * sizeY + 10.0f);
                yield return new WaitForFixedUpdate();
            }
        }
        CoverEdges();
        progress = (sizeX * sizeY + 1.0f) / (sizeX * sizeY + 10.0f);
        yield return new WaitForFixedUpdate();
        progress = (sizeX * sizeY + 2.0f) / (sizeX * sizeY + 10.0f);
        GenerateGrass();
        yield return new WaitForFixedUpdate();
        progress = 1.0f;
        yield break;
    }

    public void GenerateIslandShape(int solveIterations)
    {
        //Intialize Array
        isLand = new int[sizeX][];
        for(int i = 0; i < sizeX; i++)
        {
            isLand[i] = new int[sizeY];
        }
        //Seed array
        for(int i = 1; i < sizeX - 1; i++)
        {
            for(int j = 1; j < sizeY - 1; j++)
            {
                if(Random.value > 0.425f)
                {
                    isLand[i][j] = 1;
                }
                else
                {
                    isLand[i][j] = 0;
                }
            }
        }
        for (int solve = 0; solve < solveIterations; solve++)
        {
            for (int i = 1; i < sizeX - 1; i++)
            {
                for (int j = 1; j < sizeY - 1; j++)
                {
                    if(isLand[i][j] == 0)
                    {
                        if(isLand[i - 1][j - 1] + isLand[i - 1][j] + isLand[i - 1][j + 1] +
                            isLand[i + 1][j - 1] + isLand[i + 1][j] + isLand[i + 1][j + 1] +
                            isLand[i][j - 1] + isLand[i][j + 1] >= 5)
                        {
                            isLand[i][j] = 1;
                        }
                    }
                    else
                    {
                        if(isLand[i - 1][j - 1] + isLand[i - 1][j] + isLand[i - 1][j + 1] +
                            isLand[i + 1][j - 1] + isLand[i + 1][j] + isLand[i + 1][j + 1] +
                            isLand[i][j - 1] + isLand[i][j + 1] <= 3)
                        {
                            isLand[i][j] = 0;
                        }
                    }
                }
            }
        }
        landNodes = new LandNode[sizeX][];
        for(int i = 0; i < sizeX; i++)
        {
            landNodes[i] = new LandNode[sizeY];
            for(int j = 0; j < sizeY; j++)
            {
                if (i != 0 && j != 0 && isLand[i][j] != 0)
                {
                    landNodes[i][j] = new LandNode(isLand[i - 1][j + 1], isLand[i][j + 1], isLand[i + 1][j + 1], isLand[i - 1][j], isLand[i + 1][j], isLand[i - 1][j - 1], isLand[i][j - 1], isLand[i + 1][j - 1]);
                }
                else
                {
                    landNodes[i][j] = new LandNode(0, 0, 0, 0, 0, 0, 0, 0);
                }
            }
        }
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
                int num = (int)SamplePerlinLerp(x, y, i, j);
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
                float perlin = SamplePerlinLerp(i / 32, j / 32, i % 32, j % 32);
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
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public float SamplePerlinLerp(int x, int y, int i, int j)
    {
        //wat da hell why can't i find a better way
        if (x < 0 || y < 0 || x >= sizeX || y >= sizeY || isLand[x][y] == 0)
        {
            return 0;
        }
        float factor = 1;
        LandNode node = landNodes[x][y];
        //Split into quadrants
        if(i < 16 && j < 16)
        {
            //bottom left
            factor = 0.5f * (j / 15.0f) *                   Mathf.Lerp(node.left, node.center, i / 15.0f) + //Lerp from left to center
                     0.5f * (1 - (j / 15.0f)) *             Mathf.Lerp(node.bottomLeft, node.bottom, i / 15.0f) + //Lerp from bottomLeft to bottom
                     0.5f * (i / 15.0f) *                   Mathf.Lerp(node.bottom, node.center, j / 15.0f) + //Lerp from bottom to center
                     0.5f * (1 - (i / 15.0f)) *             Mathf.Lerp(node.bottomLeft, node.left, j / 15.0f); //Lerp from bottomLeft to left
        }
        else if(i >= 16 && j < 16)
        {
            //Bottom right
            factor = 0.5f * (j / 15.0f) *                   Mathf.Lerp(node.center, node.right, (i - 16.0f) / 15.0f) + //lerp from center to right
                     0.5f * (1 - (j / 15.0f)) *             Mathf.Lerp(node.bottom, node.bottomRight, (i - 16.0f) / 15.0f) + //Lerp from bottom to right
                     0.5f * ((i - 16.0f) / 15.0f) *         Mathf.Lerp(node.bottomRight, node.right, j / 15.0f) + //Lerp from bottom to center
                     0.5f * (1 - ((i - 16.0f) / 15.0f)) *   Mathf.Lerp(node.bottom, node.center, j / 15.0f); //Lerp from bottomLeft to left
        }
        else if(i < 16 && j >= 16)
        {
            //Top left
            factor = 0.5f * ((j - 16.0f) / 15.0f) *         Mathf.Lerp(node.topLeft, node.top, i / 15.0f) + //Lerp from topLeft to top
                     0.5f * (1 - ((j - 16.0f) / 15.0f)) *   Mathf.Lerp(node.left, node.center, i / 15.0f) + //Lerp from left to center
                     0.5f * (i / 15.0f) *                   Mathf.Lerp(node.center, node.top, (j - 16.0f) / 15.0f) + //Lerp from center to top
                     0.5f * (1 - (i / 15.0f)) *             Mathf.Lerp(node.left, node.topLeft, (j - 16.0f) / 15.0f); //Lerp from left to topleft
        }
        else
        {
            //Top right
            factor = 0.5f * ((j - 16.0f) / 15.0f) *         Mathf.Lerp(node.top, node.topRight, (i - 16.0f) / 15.0f) + //Lerp from top to topRight
                     0.5f * (1 - ((j - 16.0f) / 15.0f)) *   Mathf.Lerp(node.center, node.right, (i - 16.0f) / 15.0f) + //Lerp from center to right
                     0.5f * ((i - 16.0f) / 15.0f) *         Mathf.Lerp(node.right, node.topRight, (j - 16.0f) / 15.0f) + //Lerp from right to topRight
                     0.5f * (1 - ((i - 16.0f) / 15.0f)) *   Mathf.Lerp(node.center, node.top, (j - 16.0f) / 15.0f); //Lerp from center to top
        }

        float perlin = Mathf.Clamp(0.33f * Mathf.PerlinNoise(((x * 32 + i) / 2.0f) + seed, ((y * 32 + j) / 2.0f) + seed) + 0.67f * Mathf.PerlinNoise(((x * 32 + i) / 4.0f) + seed, ((y * 32 + j) / 4.0f) + seed), 0, 0.99f);
        perlin = Mathf.Pow(perlin, 0.55f);
        return Mathf.Clamp01(factor) * maps.Length * perlin;
    }
    
    

    /// <summary>
    /// Initializes the pathing grid with unwalkable areas according to generated map
    /// </summary>
    public void InitializePathing()
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
        Pathing.Instance.CalculatePathablity(sizeX * 32, sizeY * 32);
    }

    public LandNode GetLandNode(int x, int y)
    {
        return landNodes[x][y];
    }

    public void SetWidth(int sizeX)
    {
        this.sizeX = sizeX;
    }

    public void SetHeight(int sizeY)
    {
        this.sizeY = sizeY;
    }

    public void GenerateColliders()
    {
        TilemapCollider2D[] colliders = this.gameObject.GetComponentsInChildren<TilemapCollider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }
        StartCoroutine("BuildCompositeColliders");
    }

    IEnumerator BuildCompositeColliders()
    {

        CompositeCollider2D[] colliders = this.gameObject.GetComponentsInChildren<CompositeCollider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GenerateGeometry();
            colliders[i].generationType = CompositeCollider2D.GenerationType.Synchronous;
            colliderProgress = (i * 1.0f) / (i + 1);
            yield return new WaitForFixedUpdate();
        }
        colliderProgress = 1.0f;
        yield break;
    }

    public void DisableColliders()
    {
        CompositeCollider2D[] compositeColliders = this.gameObject.GetComponentsInChildren<CompositeCollider2D>();
        TilemapCollider2D[] colliders = this.gameObject.GetComponentsInChildren<TilemapCollider2D>();
        for (int i = 0; i < compositeColliders.Length; i++)
        {
            compositeColliders[i].generationType = CompositeCollider2D.GenerationType.Manual;
        }
        for(int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

    public void RemoveFromDontDestroyOnLoad()
    {
        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
    }
}
