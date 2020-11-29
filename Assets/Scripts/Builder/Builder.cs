﻿using UnityEngine;
using UnityEngine.UIElements;

public class Builder : MonoBehaviour
{
    public Freezer freezer;
    public Camera mainCamera;
    public WorldGenerator worldGenerator;

    public Transform buildingsTransform;

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
            GameObject building = Instantiate(currentBuildingPrefab, position, Quaternion.identity, buildingsTransform);

            if (isCurrentBuildResourceExtractor)
            {
                ResourceDeposit resourceDeposit = worldGenerator.GetResourceDeposit(position);
                Debug.Assert(resourceDeposit, "Resource Deposit is null");

                ResourceExtractor resourceExtractor = building.GetComponent<ResourceExtractor>();
                resourceExtractor.SetDeposit(resourceDeposit);
            }

            worldGenerator.AddBuildingToHexCell(position, building);
        }
    }


    private void Start()
    {
        if (!freezer)
        {
            Debug.LogError("Freezer doesn't set");
            freezer = FindObjectOfType<Freezer>();
        }
        if (!mainCamera)
        {
            Debug.LogError("Main Camera doesn't set");
            mainCamera = Camera.main;
        }
        if (!worldGenerator)
        {
            Debug.LogError("World Generator doesn't set");
            worldGenerator = FindObjectOfType<WorldGenerator>();
        }

        if (!buildingsTransform)
        {
            Debug.LogWarning("buildings Transform is null, so new buildings will be attached to the root transform");
        }
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

            if (!currentBuilding)
            {
                UpdateCurrentBuilding();
            }

            if (currentBuilding)
            {
                Vector3 hexCenter = worldGenerator.GetHexCenterPosition(enter);
                bool isHexContainsResource = worldGenerator.IsHexContainsResource(enter);

                isValidPlace = worldGenerator.IsHexAvailableForBuilding(enter);
                isValidPlace &= isCurrentBuildResourceExtractor == isHexContainsResource;

                currentBuilding.transform.position = hexCenter + Vector3.up * preViewOffset;
                currentBuildHelper.SetMaterialColor(isValidPlace ? validPositionColor : invalidPositionColor);

                if (isValidPlace && Input.GetMouseButtonDown((int) MouseButton.LeftMouse))
                {
                    AddCurrentBuildingToMap();
                }
            }
        }
        else
        {
            Destroy(currentBuilding);
        }
    }
}
