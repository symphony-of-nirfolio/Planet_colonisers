using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour
{
    public Camera mainCamera;

    public MapSizeList mapSizeList;
    public PlanetInfoList planetInfoList;

    public GameObject smallPlanetRectanglePrefab;
    public GameObject smallPlanetPrefab;

    public RectTransform smallPlanetsRectTransform;

    public TMP_Dropdown colonyTypeDropdown;
    public TMP_Dropdown mapSizeDropdown;


    private GameObject[] smallPlanetRectangles;
    private GameObject[] smallPlanets = new GameObject[0];
    private SmallPlanetController[] smallPlanetControllers;

    private string currentColonyType;
    private MapSize currentMapSize;
    private PlanetInfo currentPlanetInfo;


    public void StartGame()
    {
        GameObject gameParametersLoaderGameObject = new GameObject("GameParametersLoader");
        GameParametersLoader gameParametersLoader = gameParametersLoaderGameObject.AddComponent<GameParametersLoader>();
        gameParametersLoader.colonyType = currentColonyType;
        gameParametersLoader.mapSize = currentMapSize;
        gameParametersLoader.planetInfo = currentPlanetInfo;

        SceneManager.LoadScene("GameScene");
    }

    public void SetMapSize(int index)
    {
        Debug.Assert(
            index >= 0 && index < mapSizeList.mapSizes.Length,
            "Index have to be in range [0, size of mapSizeList]");
        currentMapSize = mapSizeList.mapSizes[index];
    }


    private void InitColonyTypes()
    {
        colonyTypeDropdown.ClearOptions();

        List<string> options = new List<string>
        {
            "Default"
        };

        colonyTypeDropdown.AddOptions(options);
        colonyTypeDropdown.value = 0;
        colonyTypeDropdown.RefreshShownValue();

        currentColonyType = options[0];
    }

    private void InitMapSizes()
    {
        mapSizeDropdown.ClearOptions();

        List<string> options = new List<string>();

        foreach (MapSize mapSize in mapSizeList.mapSizes)
        {
            options.Add(mapSize.name);
        }

        mapSizeDropdown.AddOptions(options);
        mapSizeDropdown.value = 0;
        mapSizeDropdown.RefreshShownValue();

        currentMapSize = mapSizeList.mapSizes[0];
    }

    private void InitPlanetInfos()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(smallPlanetsRectTransform);

        smallPlanets = new GameObject[planetInfoList.planetInfos.Length];
        smallPlanetControllers = new SmallPlanetController[planetInfoList.planetInfos.Length];

        for (int i = 0; i < smallPlanetRectangles.Length; ++i)
        {
            Ray ray = mainCamera.ScreenPointToRay(smallPlanetRectangles[i].transform.GetChild(0).transform.position);

            Plane plane = new Plane(Vector3.back, Vector3.zero);

            if (plane.Raycast(ray, out float enter))
            {
                Vector3 intersectPoint = ray.GetPoint(enter);

                smallPlanets[i] = Instantiate(smallPlanetPrefab, intersectPoint, Quaternion.identity);
                smallPlanetControllers[i] = smallPlanets[i].GetComponent<SmallPlanetController>();
                smallPlanetControllers[i].SetName(planetInfoList.planetInfos[i].name);
                smallPlanetControllers[i].SetMaterial(planetInfoList.planetInfos[i].smallPlanetMaterial);
                smallPlanetControllers[i].SetCamera(mainCamera);
            }
            else
            {
                Debug.LogWarning("Ray doesn't intersect with xOz plane");
            }
        }

        smallPlanetControllers[0].Select();

        currentPlanetInfo = planetInfoList.planetInfos[0];
    }


    private void Awake()
    {
        Debug.Assert(mainCamera, "Main Camera doesn't set");
        Debug.Assert(mapSizeList, "Map Info List doesn't set");
        Debug.Assert(planetInfoList, "Planet Info List doesn't set");
        Debug.Assert(smallPlanetRectanglePrefab, "Small Planet Rectangle Prefab doesn't set");
        Debug.Assert(smallPlanetPrefab, "Small Planet Prefab doesn't set");
        Debug.Assert(smallPlanetsRectTransform, "Small Planets Rect Transform doesn't set");

        smallPlanetRectangles = new GameObject[planetInfoList.planetInfos.Length];
        
        for (int i = 0; i < planetInfoList.planetInfos.Length; ++i)
        {
            smallPlanetRectangles[i] = Instantiate(smallPlanetRectanglePrefab, smallPlanetsRectTransform);
        }
    }

    private void OnEnable()
    {
        foreach (GameObject smallPlanet in smallPlanets)
        {
            smallPlanet.SetActive(true);
        }
    }

    private void Start()
    {
        InitColonyTypes();
        InitMapSizes();
        InitPlanetInfos();
    }

    private void OnDisable()
    {
        foreach (GameObject smallPlanet in smallPlanets)
        {
            if (smallPlanet)
            {
                smallPlanet.SetActive(false);
            }
        }
    }
}
