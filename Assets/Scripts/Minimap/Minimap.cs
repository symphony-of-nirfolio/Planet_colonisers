using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameParameters gameParameters;
    public Camera minimapCamera;
    public Camera mainCamera;

    public LineRenderer mainCameraViewLineRenderer;


    private Vector3[] lineRendererPositions;


    private void UpdateMainCameraViewLine()
    {
        int cornerCount = 4;
        Vector3[] screenPositions = new Vector3[cornerCount];
        screenPositions[0] = Vector3.zero;
        screenPositions[1] = Vector3.up * Screen.height;
        screenPositions[2] = Vector3.up * Screen.height + Vector3.right * Screen.width;
        screenPositions[3] = Vector3.right * Screen.width;

        for (int i = 0; i < cornerCount; ++i)
        {
            if (Utils.IntersectionScreenPointRayWithXOZPlane(screenPositions[i], mainCamera, out Vector3 intersectPoint))
            {
                lineRendererPositions[i] = intersectPoint;
            }
            else
            {
                Ray ray = mainCamera.ScreenPointToRay(screenPositions[i]);
                lineRendererPositions[i] = ray.origin +
                    ray.direction * gameParameters.mapSize.width * gameParameters.mapSize.height;
                lineRendererPositions[i].y = 0f;
            }
        }

        mainCameraViewLineRenderer.SetPositions(lineRendererPositions);
    }

    private void Start()
    {
        Debug.Assert(gameParameters, "Game Parameters doesn't set");
        Debug.Assert(minimapCamera, "Minimap Camera doesn't set");
        Debug.Assert(mainCamera, "Main Camera doesn't set");
        Debug.Assert(mainCameraViewLineRenderer, "Main Camera View Line Renderer doesn't set");

        minimapCamera.orthographicSize = gameParameters.mapSize.height / 2f;

        lineRendererPositions = new Vector3[mainCameraViewLineRenderer.positionCount];
        UpdateMainCameraViewLine();
    }


    private void LateUpdate()
    {
        UpdateMainCameraViewLine();
    }
}
