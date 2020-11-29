﻿using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static bool IntersectionRayWithXOZPlane(Ray ray, out Vector3 intersectPoint)
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float enter))
        {
            intersectPoint = ray.GetPoint(enter);
            return true;
        }

        intersectPoint = Vector3.zero;
        return false;
    }

    public static bool IntersectionScreenPointRayWithXOZPlane(Vector3 mousePosition, Camera mainCamera, out Vector3 intersectPoint)
    {
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        return IntersectionRayWithXOZPlane(ray, out intersectPoint);
    }

    public static bool IntersectionMouseRayWithXOZPlane(Camera mainCamera, out Vector3 intersectPoint)
    {
        Vector3 mousePosition = Input.mousePosition;
        return IntersectionScreenPointRayWithXOZPlane(mousePosition, mainCamera, out intersectPoint);
    }

    public static void Shuffle<T>(IList<T> list, int seed)
    {
        Random.InitState(seed);

        for (int i = 0; i < list.Count - 1; ++i)
        {
            int index = Random.Range(i + 1, list.Count);
            T temp = list[index];
            list[index] = list[i];
            list[i] = temp;
        }
    }
}
