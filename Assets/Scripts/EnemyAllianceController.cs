using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAllianceController : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject aiFormationPrefab;
    [SerializeField] LayerMask spawningSearchMask;
    ContactFilter2D filter;
    public int startingTimeForNewWave { get; private set; } = 18000; //6 minutes until first wave
    public int timeUntilNewWave { get; private set; } = 18000;
    public int minTimePerWave { get; private set; } = 3000;
    int enemyCountPerWave = 2; //start with one per wave
    int maxEnemyCountPerWave = 25;
    public int wave { get; private set; } = 0;
    GameObject target;
    Alliance enemyAlliance;
    // Start is called before the first frame update
    void Start()
    {
        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = spawningSearchMask;
    }

    public void SetAlliance(Alliance alliance)
    {
        this.enemyAlliance = alliance;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            timeUntilNewWave = 1;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        timeUntilNewWave--;
        if(timeUntilNewWave == 0)
        {
            wave++;
            int[] coords = CalculateSpawnSquare();
            List<Vector2> spawnLocations = new List<Vector2>();
            for(int i = 0; i < 32; i ++)
            {
                for(int j = 0; j < 32; j++)
                {
                    if(Pathing.Instance.GetLocationPathValue(coords[0] * 32 + i, coords[1] * 32 + j) >= 3)
                    {
                        spawnLocations.Add(new Vector2(coords[0] * 32 + i + 0.5f, coords[1] * 32 + j + 0.5f));
                    }
                    if(spawnLocations.Count >= enemyCountPerWave)
                    {
                        break;
                    }
                }
            }
            GameObject formationObject = Instantiate(aiFormationPrefab, Vector3.zero, Quaternion.identity);
            AIFormation formation = formationObject.GetComponent<AIFormation>();
            for(int i = 0; i < spawnLocations.Count; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnLocations[i], Quaternion.identity);
                GenericAI enemyAI = enemy.GetComponent<GenericAI>();
                enemyAI.SetAlliance(enemyAlliance);
                formation.RegisterUnit(enemyAI);
            }
            formation.SetGroupTarget(target);
            enemyCountPerWave++;
            startingTimeForNewWave = startingTimeForNewWave - 300;
            if(startingTimeForNewWave < minTimePerWave)
            {
                startingTimeForNewWave = minTimePerWave;
            }
            timeUntilNewWave = startingTimeForNewWave;
        }
    }

    /// <summary>
    /// Selects an area to spawn a new wave.  Based off of minimal number of objects in square and distance
    /// </summary>
    public int[] CalculateSpawnSquare()
    {
        Generator.LandNode[][] broadMap = Pathing.Instance.GetBroadScaleMap();
        Collider2D[] results = new Collider2D[32];
        int countOfObjects = 0;
        int bestCount = 1000;
        float bestDistance = 1000;
        int biX = 0;
        int biY = 0;
        for(int i = 0; i < broadMap.Length; i++)
        {
            for(int j = 0; j < broadMap[i].Length; j++)
            {
                if (broadMap[i][j].landNodeType != Generator.LandNode.LandNodeType.empty)
                {
                    countOfObjects = Physics2D.OverlapBox(new Vector2(i * 32 + 16, j * 32 + 16), new Vector2(32, 32), 0, filter, results);
                    if(countOfObjects < bestCount)
                    {
                        bestCount = countOfObjects;
                        biX = i;
                        biY = j;
                    }
                }
            }
        }
        Debug.Log("Best place: " + biX + "," + biY + "\n Count: " + bestCount);
        return new int[] { biX, biY };
    }

    public void SetTarget(GameObject target)
    {
        //sets the target of the enemy alliance to the beacon in this case, but could also be player
        this.target = target;
    }
}
