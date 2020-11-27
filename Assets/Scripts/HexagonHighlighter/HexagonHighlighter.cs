using System.Collections.Generic;
using UnityEngine;

public class HexagonHighlighter : MonoBehaviour
{
    public GameParameters gameParameters;
    public Camera mainCamera;
    public WorldGenerator worldGenerator;

    public GameObject highlighter;
    public Renderer currentRenderer;

    public float smoothTime = 0.1f;

    public float radius = 0.1f;


    private Vector3 currentVelocity;


    private Vector3 debugPoint1;
    private Vector3 debugPoint2;

    private List<Vector3> debugPoints = new List<Vector3>();


    private bool IntersectionRayWithXOZPlane(Ray ray, out Vector3 intersectPoint)
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

    private bool IntersectionRayFromMouseWithXOZPlane(Vector3 mousePosition, out Vector3 intersectPoint)
    {
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        return IntersectionRayWithXOZPlane(ray, out intersectPoint);
    }

    private bool IntersectionRayFromMouseWithXOZPlane(out Vector3 intersectPoint)
    {
        Vector3 mousePosition = Input.mousePosition;
        return IntersectionRayFromMouseWithXOZPlane(mousePosition, out intersectPoint);
    }


    private void Start()
    {
        Debug.Assert(gameParameters, "Game Parameters doesn't set");
        Debug.Assert(mainCamera, "Game Parameters doesn't set");
        Debug.Assert(worldGenerator, "World Generator doesn't set");
        Debug.Assert(highlighter, "Highlighter doesn't set");
        Debug.Assert(currentRenderer, "Current Renderer doesn't set");
    }

    private void Update()
    {
        if (IntersectionRayFromMouseWithXOZPlane(out Vector3 enter))
        {
            highlighter.SetActive(true);

            debugPoint1 = enter;

            if (Input.GetKeyDown(KeyCode.A))
            {
                debugPoints.Add(enter);
            }
            Vector3 hexCenter = worldGenerator.GetHexCenterPosition(enter);

            debugPoint2 = hexCenter;
            transform.position = Vector3.SmoothDamp(transform.position, hexCenter, ref currentVelocity, smoothTime);
        }
        else
        {
            highlighter.SetActive(false);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(debugPoint1, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(debugPoint2, radius);

        Gizmos.color = Color.yellow;
        foreach (Vector3 point in debugPoints)
        {
            Gizmos.DrawSphere(point, radius);
        }
    }
}
