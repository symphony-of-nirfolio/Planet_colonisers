using UnityEngine;
using UnityEngine.UIElements;

public class Builder : MonoBehaviour
{
    public Freezer freezer;

    public Camera mainCamera;

    public Color validPositionColor;
    public Color invalidPositionColor;

    public float preViewOffset = 0.01f;

    private GameObject currentBuildingPrefab = null;
    private GameObject currentBuilding = null;
    private BuildHelper currentBuildHelper = null;

    private bool isActive = false;


    public bool IsActiveBuilder()
    {
        return isActive;
    }

    public void DeactivateBuilder()
    {
        isActive = false;
        Destroy(currentBuilding);
    }

    public void SetCurrentBuilding(GameObject buildingPrefab)
    {
        isActive = true;
        currentBuildingPrefab = buildingPrefab;

        UpdateCurrentBuilding();
    }


    private bool IntersectionRayFromMouseWithXOZPlane(out Vector3 intersectPoint)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float enter))
        {
            intersectPoint = ray.GetPoint(enter);
            return true;
        }

        intersectPoint = Vector3.zero;
        return false;
    }

    private void UpdateCurrentBuilding()
    {
        if (currentBuilding)
        {
            Destroy(currentBuilding);
        }

        if (IntersectionRayFromMouseWithXOZPlane(out Vector3 intersectPoint))
        {
            currentBuilding = Instantiate(
                currentBuildingPrefab,
                intersectPoint + Vector3.up * preViewOffset,
                Quaternion.identity);
            currentBuildHelper = currentBuilding.GetComponent<BuildHelper>();
        }
    }

    private void AddCurrentBuildingToMap()
    {
        if (currentBuilding)
        {
            Instantiate(
                currentBuildingPrefab,
                currentBuilding.transform.position - Vector3.up * preViewOffset,
                Quaternion.identity);
        }
    }


    private void Start()
    {
        Debug.Assert(freezer, "Freezer doesn't set");
        Debug.Assert(mainCamera, "Main Camera doesn't set");
    }

    private void Update()
    {
        if (!freezer.IsInteractionFreeze && 
            Input.GetMouseButtonDown((int) MouseButton.RightMouse))
        {
            DeactivateBuilder();
        }

        if (!isActive)
        {
            return;
        }

        if (!freezer.IsInteractionFreeze &&
            IntersectionRayFromMouseWithXOZPlane(out Vector3 intersectPoint))
        {
            if (currentBuilding)
            {
                currentBuilding.transform.position = intersectPoint + Vector3.up * preViewOffset;
            }
            else
            {
                UpdateCurrentBuilding();
            }

            if (currentBuildHelper.IsCollideWithOtherBuildings)
            {
                currentBuildHelper.SetMaterialColor(invalidPositionColor);
            }
            else
            {
                currentBuildHelper.SetMaterialColor(validPositionColor);
            }
        }
        else
        {
            Destroy(currentBuilding);
        }
    }

    private void LateUpdate()
    {
        if (!freezer.IsInteractionFreeze &&
            currentBuilding)
        {
            if (!currentBuildHelper.IsCollideWithOtherBuildings &&
                Input.GetMouseButtonDown((int) MouseButton.LeftMouse))
            {
                AddCurrentBuildingToMap();
            }
        }
    }
}
