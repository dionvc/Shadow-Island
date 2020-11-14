using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMapGenerator : MonoBehaviour
{
    [SerializeField] GameObject mapObjectPrefab = null;
    [SerializeField] GameObject previewTilePrefab = null;
    [SerializeField] GameObject previewPane = null;
    GameObject mapObjectInstance = null;
    Generator generator;
    int xSizeBuffer = 24;
    int ySizeBuffer = 24;
    [SerializeField] Sprite[] previewImages = null;
    List<GameObject> previewTiles = null;
    // Start is called before the first frame update
    void Start()
    {
        mapObjectInstance = Instantiate(mapObjectPrefab, new Vector3(0,0,0), Quaternion.identity);
        DontDestroyOnLoad(mapObjectInstance);
        generator = mapObjectInstance.GetComponent<Generator>();
        previewTiles = new List<GameObject>();
    }

    /// <summary>
    /// Generate the preview so that it can rendered in the menu, and swap the sprites of the previewTiles
    /// </summary>
    public void GeneratePreview()
    {
        generator.SetWidth(xSizeBuffer);
        generator.SetHeight(ySizeBuffer);
        Debug.Log(generator.sizeX);
        Debug.Log(generator.sizeY);
        generator.GenerateIslandShape(12);
        for(int i = 0; i < previewTiles.Count; i++)
        {
            Destroy(previewTiles[i]);
        }
        previewTiles.Clear();
        for(int i = 0; i < generator.sizeY; i++)
        {
            for(int j = 0; j < generator.sizeX; j++)
            {
                previewTiles.Add(Instantiate(previewTilePrefab, previewPane.transform));
                Debug.Log(i);
                Debug.Log(j);
                int imageIndex = (int)generator.GetLandNode(j, generator.sizeY - i - 1).landNodeType;
                previewTiles[previewTiles.Count - 1].GetComponent<Image>().sprite = previewImages[imageIndex];
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

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
        generator.GenerateIsland();
        mapObjectInstance.GetComponent<GeneratorObjects>().GenerateObjects();
    }
}
