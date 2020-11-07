using UnityEngine;
using UnityEngine.UIElements;

public class Builder : MonoBehaviour
{
    public Camera mainCamera;

    public Color validPositionColor;
    public Color invalidPositionColor;

    public float preViewOffset = 0.01f;

    private GameObject currentBuildingPrefab = null;
    private GameObject currentBuilding = null;
    private BuildHelper currentBuildHelper = null;


    public void SetCurrentBuilding(GameObject buildingPrefab)
    {
        currentBuildingPrefab = buildingPrefab;

        UpdateCurrentBuilding();
    }


    private void UpdateCurrentBuilding()
    {
        if (currentBuilding)
        {
            Destroy(currentBuilding);
        }

        currentBuilding = Instantiate(currentBuildingPrefab);
        currentBuildHelper = currentBuilding.GetComponent<BuildHelper>();
    }

    private void AddCurrentBuildingToMap()
    {
        if (currentBuilding)
        {
            Instantiate(currentBuildingPrefab, currentBuilding.transform.position - Vector3.up * preViewOffset, Quaternion.identity);
        }
    }


    private void Start()
    {
        Debug.Assert(mainCamera, "Main Camera doesn't set");
    }

    private void Update()
    {
        if (currentBuilding)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            Plane plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(ray, out float enter))
            {
                currentBuilding.transform.position = ray.GetPoint(enter) + Vector3.up * preViewOffset;

                if (!currentBuildHelper.IsCollideWithOtherBuildings)
                {
                    currentBuildHelper.SetMaterialColor(validPositionColor);
                }
                else
                {
                    currentBuildHelper.SetMaterialColor(invalidPositionColor);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (currentBuilding)
        {
            if (!currentBuildHelper.IsCollideWithOtherBuildings && Input.GetMouseButtonDown((int) MouseButton.LeftMouse))
            {
                AddCurrentBuildingToMap();
            }
        }
    }
}
