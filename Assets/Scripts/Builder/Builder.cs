using UnityEngine;
using UnityEngine.UIElements;

public class Builder : MonoBehaviour
{
    public Freezer freezer;
    public Camera mainCamera;
    public WorldGenerator worldGenerator;

    public Color validPositionColor;
    public Color invalidPositionColor;

    public float preViewOffset = 0.01f;

    private GameObject currentBuildingPrefab = null;
    private GameObject currentBuilding = null;
    private BuildHelper currentBuildHelper = null;

    private bool isCurrentBuildResourceExtractor = false;

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


    private void UpdateCurrentBuilding()
    {
        if (currentBuilding)
        {
            Destroy(currentBuilding);
        }

        if (Utils.IntersectionMouseRayWithXOZPlane(mainCamera, out Vector3 intersectPoint))
        {
            currentBuilding = Instantiate(
                currentBuildingPrefab,
                intersectPoint + Vector3.up * preViewOffset,
                Quaternion.identity);
            currentBuildHelper = currentBuilding.GetComponent<BuildHelper>();

            isCurrentBuildResourceExtractor = currentBuilding.TryGetComponent(out ResourceExtractor _);
        }
    }

    private void AddCurrentBuildingToMap()
    {
        if (currentBuilding)
        {
            Vector3 position = currentBuilding.transform.position - Vector3.up * preViewOffset;
            GameObject building = Instantiate(currentBuildingPrefab, position, Quaternion.identity);

            if (isCurrentBuildResourceExtractor)
            {
                ResourceDeposit resourceDeposit = worldGenerator.GetResourceDeposit(position);
                Debug.Assert(resourceDeposit, "Resource Deposit is null");

                ResourceExtractor resourceExtractor = building.GetComponent<ResourceExtractor>();
                resourceExtractor.SetDeposit(resourceDeposit);
            }
        }
    }


    private void Start()
    {
        Debug.Assert(freezer, "Freezer doesn't set");
        Debug.Assert(mainCamera, "Main Camera doesn't set");
        Debug.Assert(worldGenerator, "World Generator doesn't set");
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
            Utils.IntersectionMouseRayWithXOZPlane(mainCamera, out Vector3 enter))
        {
            bool isValidPlace = true;

            if (currentBuilding)
            {
                Vector3 hexCenter = worldGenerator.GetHexCenterPosition(enter);
                bool isHexContainsResource = worldGenerator.IsHexContainsResource(enter);

                isValidPlace = isCurrentBuildResourceExtractor == isHexContainsResource;

                currentBuilding.transform.position = hexCenter + Vector3.up * preViewOffset;
            }
            else
            {
                UpdateCurrentBuilding();
            }

            // TODO: replace with world generator function
            isValidPlace &= !currentBuildHelper.IsCollideWithOtherBuildings;

            currentBuildHelper.SetMaterialColor(isValidPlace ? validPositionColor : invalidPositionColor);

            if (isValidPlace && Input.GetMouseButtonDown((int) MouseButton.LeftMouse))
            {
                AddCurrentBuildingToMap();
            }
        }
        else
        {
            Destroy(currentBuilding);
        }
    }
}
