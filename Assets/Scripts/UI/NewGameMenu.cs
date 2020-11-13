using UnityEngine;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour
{
    public Camera mainCamera;

    public PlanetInfoList planetInfoList;

    public GameObject smallPlanetRectanglePrefab;
    public GameObject smallPlanetPrefab;

    public RectTransform smallPlanetsRectTransform;


    private GameObject[] smallPlanetRectangles;
    private GameObject[] smallPlanets = new GameObject[0];
    private SmallPlanetController[] smallPlanetControllers;


    private void Awake()
    {
        Debug.Assert(mainCamera, "Main Camera doesn't set");
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
                smallPlanetControllers[i].SetName(planetInfoList.planetInfos[i].planetName);
                smallPlanetControllers[i].SetMaterial(planetInfoList.planetInfos[i].smallPlanetMaterial);
                smallPlanetControllers[i].SetCamera(mainCamera);
            }
            else
            {
                Debug.LogWarning("Ray doesn't intersect with xOz plane");
            }
        }

        smallPlanetControllers[0].Select();
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
