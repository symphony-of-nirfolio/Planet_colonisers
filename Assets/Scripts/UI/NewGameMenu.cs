using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour
{
    public Camera mainCamera;
    public MainMenuLauncher mainMenuLauncher;

    public ColonyTypeList colonyTypeList;
    public MapSizeList mapSizeList;
    public PlanetInfoList planetInfoList;

    public GameObject smallPlanetRectanglePrefab;
    public GameObject smallPlanetPrefab;

    public RectTransform smallPlanetsRectTransform;

    public TMP_Dropdown colonyTypeDropdown;
    public TMP_Dropdown mapSizeDropdown;

    public Slider colonyAmountSlider;
    public TMP_Text colonyAmountSliderValueText;

    public int defaultColonyAmount = 4;
    public int maxColonyAmount = 8;


    private GameObject[] smallPlanetRectangles;
    private GameObject[] smallPlanets = new GameObject[0];
    private SmallPlanetController[] smallPlanetControllers;

    private ColonyType currentColonyType;
    private MapSize currentMapSize;
    private int currentColonyAmount;
    private PlanetInfo currentPlanetInfo;


    public void StartGame()
    {
        GameObject gameParametersLoaderGameObject = new GameObject("GameParametersLoader");
        GameParametersLoader gameParametersLoader = gameParametersLoaderGameObject.AddComponent<GameParametersLoader>();
        gameParametersLoader.colonyType = currentColonyType;
        gameParametersLoader.mapSize = currentMapSize;
        gameParametersLoader.colonyAmount = currentColonyAmount;
        gameParametersLoader.planetInfo = currentPlanetInfo;

        mainMenuLauncher.StartGameScene();
    }

    public void SetColonyType(int index)
    {
        Debug.Assert(
            index >= 0 && index < colonyTypeList.colonyTypes.Length,
            "Index have to be in range [0, size of colonyTypeList]");
        currentColonyType = colonyTypeList.colonyTypes[index];
    }

    public void SetMapSize(int index)
    {
        Debug.Assert(
            index >= 0 && index < mapSizeList.mapSizes.Length,
            "Index have to be in range [0, size of mapSizeList]");
        currentMapSize = mapSizeList.mapSizes[index];
    }

    public void SetColonyAmount(float amount)
    {
        currentColonyAmount = Mathf.RoundToInt(amount);
        colonyAmountSliderValueText.text = currentColonyAmount.ToString();
    }


    private void InitColonyTypes()
    {
        colonyTypeDropdown.ClearOptions();

        List<string> options = new List<string>();

        foreach (ColonyType colonyType in colonyTypeList.colonyTypes)
        {
            options.Add(colonyType.name);
        }

        colonyTypeDropdown.AddOptions(options);
        colonyTypeDropdown.value = 0;
        colonyTypeDropdown.RefreshShownValue();

        currentColonyType = colonyTypeList.colonyTypes[0];
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

    private void InitColonyAmountSlider()
    {
        colonyAmountSliderValueText.text = defaultColonyAmount.ToString();

        colonyAmountSlider.minValue = 1;
        colonyAmountSlider.maxValue = maxColonyAmount;
        colonyAmountSlider.value = defaultColonyAmount;

        currentColonyAmount = defaultColonyAmount;
    }


    private void Awake()
    {
        Utils.CheckMainCameraNotNullAndTryToSet(ref mainCamera);
        Utils.CheckFieldNotNullAndTryToSet(ref mainMenuLauncher, "Main Menu Launcher");
        Utils.CheckFieldNotNull(colonyTypeList, "Colony Type List");
        Utils.CheckFieldNotNull(mapSizeList, "Map Info List");
        Utils.CheckFieldNotNull(planetInfoList, "Planet Info List");
        Utils.CheckFieldNotNull(smallPlanetRectanglePrefab, "Small Planet Rectangle Prefab");
        Utils.CheckFieldNotNull(smallPlanetPrefab, "Small Planet Prefab");
        Utils.CheckFieldNotNull(smallPlanetsRectTransform, "Small Planets Rect Transform");
        Utils.CheckFieldNotNull(colonyTypeDropdown, "Colony Type Dropdown");
        Utils.CheckFieldNotNull(mapSizeDropdown, "Map Size Dropdown");
        Utils.CheckFieldNotNull(colonyAmountSlider, "Colony Amount Slider");
        Utils.CheckFieldNotNull(colonyAmountSliderValueText, "Colony Amount Slider Value Text");

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
        InitColonyAmountSlider();
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
