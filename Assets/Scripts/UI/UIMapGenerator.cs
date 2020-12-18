using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIMapGenerator : MonoBehaviour
{
    //Really important things (not sure if its best to have a UI script handling this, but \_(._.)_/ )
    [SerializeField] GameObject mapObjectPrefab = null;
    [SerializeField] GameObject playerPrefab = null;
    [SerializeField] GameObject beaconPrefab = null;
    [SerializeField] GameObject enemyAllianceControllerPrefab = null;
    [SerializeField] GameObject waveUIPrefab = null;
    //Actual UI stuff
    [SerializeField] GameObject previewTilePrefab = null;
    [SerializeField] GameObject mapPreviewSelectionPrefab = null;
    [SerializeField] GameObject previewPane = null;
    [SerializeField] Button playButton = null;
    [SerializeField] GameObject loadingPanel = null;
    [SerializeField] Image loadingPanelBar = null;
    [SerializeField] GameObject parentCanvas = null;
    GameObject mapObjectInstance = null;
    GameObject mapPreviewSelection = null;
    Generator generator;
    int xSizeBuffer = 24;
    int ySizeBuffer = 24;
    int density = 12;
    int startingX = -1;
    int startingY = -1;
    [SerializeField] Sprite[] previewImages = null;
    [SerializeField] Sprite[] fillVariants = null;
    [SerializeField] MenuSlideOut[] menusToHideOnStart;
    List<GameObject> previewTiles = null;
    AsyncOperation loading = null;
    // Start is called before the first frame update
    void Start()
    {
        mapObjectInstance = Instantiate(mapObjectPrefab, new Vector3(0,0,0), Quaternion.identity);
        DontDestroyOnLoad(mapObjectInstance);
        generator = mapObjectInstance.GetComponent<Generator>();
        previewTiles = new List<GameObject>();
        playButton.interactable = false;
        mapPreviewSelection = Instantiate(mapPreviewSelectionPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        mapPreviewSelection.SetActive(false);
        DontDestroyOnLoad(parentCanvas);
    }

    /// <summary>
    /// Generate the preview so that it can rendered in the menu, and swap the sprites of the previewTiles
    /// </summary>
    public void GeneratePreview()
    {
        startingX = -1;
        startingY = -1;
        playButton.interactable = false;
        generator.SetWidth(xSizeBuffer);
        generator.SetHeight(ySizeBuffer);
        generator.GenerateIslandShape(density);
        mapPreviewSelection.transform.SetParent(null, false);
        mapPreviewSelection.SetActive(false);
        for(int i = 0; i < previewTiles.Count; i++)
        {
            Destroy(previewTiles[i]);
        }
        previewTiles.Clear();
        for(int i = 0; i < generator.sizeY; i++)
        {
            for(int j = 0; j < generator.sizeX; j++)
            {
                GameObject tile = Instantiate(previewTilePrefab, previewPane.transform);
                previewTiles.Add(tile);
                Generator.LandNode.LandNodeType type = generator.GetLandNode(j, generator.sizeY - i - 1).landNodeType;
                int imageIndex = (int)type;
                if (type == Generator.LandNode.LandNodeType.fill)
                {
                    previewTiles[previewTiles.Count - 1].GetComponent<Image>().sprite = fillVariants[Random.Range(0, fillVariants.Length)];
                }
                else
                {
                    previewTiles[previewTiles.Count - 1].GetComponent<Image>().sprite = previewImages[imageIndex];
                }
                previewTiles[previewTiles.Count - 1].GetComponent<MapPreviewLocation>().SetXY(j, generator.sizeY - i - 1);
                Button tileButton = previewTiles[previewTiles.Count - 1].GetComponent<Button>();
                if (type == Generator.LandNode.LandNodeType.fill)
                {
                    tileButton.onClick.AddListener(delegate { SetStartingLocation(tile); });
                }
                else
                {
                    tileButton.enabled = false;
                }
                
            }
        }
        previewPane.GetComponent<GridLayoutGroup>().constraintCount = generator.sizeX;
    }

    public void BufferWidth(System.Single sizeX)
    {
        xSizeBuffer = (int)sizeX;
    }

    public void BufferHeight(System.Single sizeY)
    {
        ySizeBuffer = (int)sizeY;
    }

    public void SetLandDensity(System.Single landDensity)
    {
        density = (int)landDensity;
    }

    public void SetStartingLocation(GameObject previewMapSlot)
    {
        MapPreviewLocation location = previewMapSlot.GetComponent<MapPreviewLocation>();
        startingX = location.xPos;
        startingY = location.yPos;
        mapPreviewSelection.SetActive(true);
        mapPreviewSelection.transform.SetParent(previewMapSlot.transform, false);
        mapPreviewSelection.transform.localPosition = new Vector3(0, 0, 0);
        playButton.interactable = true;
    }

    public void StartGame()
    {
        loadingPanel.SetActive(true);
        for(int i = 0; i < menusToHideOnStart.Length; i++)
        {
            menusToHideOnStart[i].TogglePanelOff();
        }
        StartCoroutine("Load");
    }

    /// <summary>
    /// Basically responsible for getting the game working.  In a UI script.  Whatever.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Load()
    {
        TMP_Text loadingText = loadingPanel.GetComponentInChildren<TMP_Text>();
        loading = SceneManager.LoadSceneAsync("MainScene");
        loadingText.SetText("Loading Assets");
        while (loading.progress < 1.0f)
        {
            loadingPanelBar.fillAmount = loading.progress;
            yield return new WaitForFixedUpdate();
        }
        //Generate terrain
        generator.DisableColliders();
        generator.GenerateIsland();
        loadingText.SetText("Generating Terrain");
        while (generator.progress < 1.0f)
        {
            loadingPanelBar.fillAmount = generator.progress;
            yield return new WaitForFixedUpdate();
        }
        //Build collison map
        loadingText.SetText("Generating Colliders");
        generator.GenerateColliders();
        while(generator.colliderProgress < 1.0f)
        {
            loadingPanelBar.fillAmount = generator.colliderProgress;
            yield return new WaitForFixedUpdate();
        }
        //Initialize Pathing
        loadingPanel.GetComponentInChildren<TMP_Text>().SetText("Initializing Pathing");
        loadingPanelBar.fillAmount = 0.33f;
        generator.InitializePathing();
        yield return new WaitForFixedUpdate();
        //Generate ore and trees
        loadingPanel.GetComponentInChildren<TMP_Text>().SetText("Generating Objects");
        loadingPanelBar.fillAmount = 0.66f;
        mapObjectInstance.GetComponent<GeneratorObjects>().GenerateObjects();
        yield return new WaitForFixedUpdate();
        //Instantiate player
        loadingPanel.GetComponentInChildren<TMP_Text>().SetText("Finishing Up");
        loadingPanelBar.fillAmount = 1;
        GameObject player = Instantiate(playerPrefab, new Vector3(startingX * 32 + 16, startingY * 32 + 16), Quaternion.identity);
        GameObject beacon = Instantiate(beaconPrefab, new Vector3(startingX * 32 + 16.5f, startingY * 32 + 17.5f), Quaternion.identity);
        Alliance characterAlliance = new Alliance("Character");
        characterAlliance.spawnPoint = player.transform.position;
        beacon.GetComponent<Health>().alliance = characterAlliance.allianceCode;
        Alliance enemyAlliance = new Alliance("Enemy");
        GameObject enemyAllianceController = Instantiate(enemyAllianceControllerPrefab, Vector2.zero, Quaternion.identity);
        enemyAllianceController.GetComponent<EnemyAllianceController>().SetTarget(beacon);
        enemyAllianceController.GetComponent<EnemyAllianceController>().SetAlliance(enemyAlliance);
        characterAlliance.SetHostileTowards(enemyAlliance.allianceCode);
        enemyAlliance.SetHostileTowards(characterAlliance.allianceCode);
        GameObject waveUI = Instantiate(waveUIPrefab, GameObject.Find("Canvas").transform);
        waveUI.GetComponent<DefendTheBeaconUI>().SetBeacon(beacon);
        waveUI.GetComponent<DefendTheBeaconUI>().SetEnemyAllianceController(enemyAllianceController.GetComponent<EnemyAllianceController>());
        yield return new WaitForFixedUpdate();
        //Set generator be destroyed when returning to main menu.
        generator.RemoveFromDontDestroyOnLoad();
        Destroy(parentCanvas);
        yield break;
    }
}
