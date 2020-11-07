﻿using UnityEngine;
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

    private bool isNeedToShow = false;


    public void SetCurrentBuilding(GameObject buildingPrefab)
    {
        isNeedToShow = true;
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
        Debug.Assert(mainCamera, "Main Camera doesn't set");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetMouseButtonDown((int) MouseButton.RightMouse))
        {
            isNeedToShow = false;
            Destroy(currentBuilding);
        }

        if (isNeedToShow)
        {
            if (IntersectionRayFromMouseWithXOZPlane(out Vector3 intersectPoint))
            {
                if (!currentBuilding)
                {
                    UpdateCurrentBuilding();
                }
                else
                {
                    currentBuilding.transform.position = intersectPoint + Vector3.up * preViewOffset;
                }

                if (!currentBuildHelper.IsCollideWithOtherBuildings)
                {
                    currentBuildHelper.SetMaterialColor(validPositionColor);
                }
                else
                {
                    currentBuildHelper.SetMaterialColor(invalidPositionColor);
                }
            }
            else
            {
                Destroy(currentBuilding);
            }
        }
    }

    private void LateUpdate()
    {
        if (currentBuilding)
        {
            if (!currentBuildHelper.IsCollideWithOtherBuildings &&
                Input.GetMouseButtonDown((int) MouseButton.LeftMouse))
            {
                AddCurrentBuildingToMap();
            }
        }
    }
}
