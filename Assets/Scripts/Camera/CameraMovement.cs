using UnityEngine;
using UnityEngine.UIElements;

public class CameraMovement : MonoBehaviour
{
    public Freezer freezer;

    public GameParameters gameParameters;

    public Camera mainCamera = null;

    public int countOfZoomSteps = 20;

    public float minFieldOfView = 40f;
    public float maxFieldOfView = 100f;

    public float minCameraHeight = 5f;
    public float maxCameraHeight = 25f;

    public float minCameraRollAngle = 40f;
    public float maxCameraRollAngle = 60f;

    public bool needDrawGizmos = false;
    public float gizmosSphereSize = 0.1f;

    [HideInInspector]
    public Vector3 playerMainBasePosition = Vector3.zero;


    private Vector3 clickedPointOnPlaneFromMouse = Vector3.zero;
    private Vector3 clickedPointOnPlaneFromCenterPoint = Vector3.zero;

    private Vector3 clickedMousePosition = Vector3.zero;

    private Vector3 clickedCameraPosition = Vector3.zero;
    private Vector3 clickedCameraRotation = Vector3.zero;

    private float clickedAngleOfScreenCenterToMouse = 0f;

    private float halfWidth;
    private float halfHeight;

    private bool isAltKeyPressed = false;
    private bool isLeftMouseButtonPressed = false;

    private int currentZoomStep = -1;


    public void SetCameraViewToMainBase()
    {
        if (IntersectionCenterPointRayWithXOZPlane(out Vector3 enter))
        {
            transform.position += playerMainBasePosition - enter;
        }
        else
        {
            Debug.LogError("Center Point isn't intersected with xOz plane");
        }
    }


    private void Move()
    {
        Vector3 cameraPosition = transform.position;

        Vector3 mouseWorldPoint = mainCamera.ScreenPointToRay(Input.mousePosition).origin;
        Vector3 fixedMouseWorldPoint = mainCamera.ScreenPointToRay(clickedMousePosition).origin;

        float height = cameraPosition.y;

        float mouseHeight = height - mouseWorldPoint.y;
        float fixedMouseHeight = height - fixedMouseWorldPoint.y;

        Vector3 mouseDelta = mouseWorldPoint - cameraPosition;
        Vector3 fixedMouseDelta = fixedMouseWorldPoint - cameraPosition;

        Vector3 delta = mouseDelta * height / mouseHeight - fixedMouseDelta * height / fixedMouseHeight;

        Vector3 offset = new Vector3(delta.x, 0f, delta.z);

        Vector3 position = clickedCameraPosition - offset;
        position.x = Mathf.Clamp(position.x, -halfWidth, halfWidth);
        position.z = Mathf.Clamp(position.z, -halfHeight, halfHeight);

        transform.position = position;
    }

    private void SetRotationAround(Vector3 point, float angle)
    {
        Vector3 centerProjectionToCamera = clickedCameraPosition - point;
        centerProjectionToCamera = Quaternion.AngleAxis(angle, Vector3.up) * centerProjectionToCamera;

        transform.position = point + centerProjectionToCamera;
        transform.eulerAngles = new Vector3(clickedCameraRotation.x, clickedCameraRotation.y + angle, 0f);
    }

    private void Rotate()
    {
        float angleOfScreenCenterToMouse = AngleOfScreenCenterToMouse();

        SetRotationAround(
            clickedPointOnPlaneFromCenterPoint,
            angleOfScreenCenterToMouse - clickedAngleOfScreenCenterToMouse);
    }

    private void SetCameraHeightByZoomStep()
    {
        float zoomCoefficient = currentZoomStep / ((float) countOfZoomSteps);

        float currentHeight = zoomCoefficient * (maxCameraHeight - minCameraHeight) + minCameraHeight;

        Vector3 position = transform.position;
        position.y = currentHeight;
        transform.position = position;
    }

    private void SetFieldOfViewByZoomStep()
    {
        float zoomCoefficient = currentZoomStep / ((float) countOfZoomSteps);

        float currentFieldOfView = zoomCoefficient * (maxFieldOfView - minFieldOfView) + minFieldOfView;

        mainCamera.fieldOfView = currentFieldOfView;
    }

    private void SetCameraRollByZoomStep()
    {
        float zoomCoefficient = currentZoomStep / ((float) countOfZoomSteps);
        float inverseZoomCoefficient = 1f - zoomCoefficient;

        float currentAngle = inverseZoomCoefficient * (maxCameraRollAngle - minCameraRollAngle) + minCameraRollAngle;

        Vector3 rotation = transform.eulerAngles;
        rotation.x = currentAngle;
        transform.eulerAngles = rotation;
    }

    private void InitZoom()
    {
        float currentFieldOfView = mainCamera.fieldOfView;

        currentZoomStep = Mathf.RoundToInt(countOfZoomSteps * (currentFieldOfView - minFieldOfView) / (maxFieldOfView - minFieldOfView));
        currentZoomStep = Mathf.Clamp(currentZoomStep, 0, countOfZoomSteps);
        
        SetCameraHeightByZoomStep();
        SetFieldOfViewByZoomStep();
        SetCameraRollByZoomStep();
    }

    private void Zoom()
    {
        float mouseWheelDelta = Input.mouseScrollDelta.y;

        if (Mathf.Abs(mouseWheelDelta) < Mathf.Epsilon)
        {
            return;
        }

        currentZoomStep -= Mathf.RoundToInt(mouseWheelDelta);
        currentZoomStep = Mathf.Clamp(currentZoomStep, 0, countOfZoomSteps);

        SetCameraHeightByZoomStep();
        SetFieldOfViewByZoomStep();
        SetCameraRollByZoomStep();
    }

    private Vector2 ScreenCenterToPointVector(Vector2 point)
    {
        return point - new Vector2(Screen.width, Screen.height) * 0.5f;
    }

    private Vector2 ScreenCenterToPointVector(Vector3 point)
    {
        return ScreenCenterToPointVector(new Vector2(point.x, point.y));
    }

    private float AngleOfScreenCenterToPointVector(Vector2 screenCenterToPointVector)
    {
        return Mathf.Atan2(screenCenterToPointVector.y, screenCenterToPointVector.x) * Mathf.Rad2Deg;
    }

    private float AngleOfScreenCenterToMouse()
    {
        return AngleOfScreenCenterToPointVector(ScreenCenterToPointVector(Input.mousePosition));
    }


    private bool IntersectionCenterPointRayWithXOZPlane(out Vector3 intersectPoint)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        return Utils.IntersectionRayWithXOZPlane(ray, out intersectPoint);
    }


    private void Awake()
    {
        Debug.Assert(freezer, "Freezer doesn't set");
        Debug.Assert(gameParameters, "Game Parameters doesn't set");

        if (!mainCamera)
        {
            mainCamera = GetComponent<Camera>();
        }

        InitZoom();
    }

    private void Start()
    {
        halfWidth = gameParameters.mapSize.width / 2f;
        halfHeight = gameParameters.mapSize.height / 2f;
    }

    private void Update()
    {
        if (freezer.IsInteractionFreeze)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            SetCameraViewToMainBase();
        }

        if (Input.GetMouseButtonDown((int) MouseButton.MiddleMouse) ||
            (Input.GetMouseButton((int) MouseButton.MiddleMouse) &&
                (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.LeftAlt))))
        {
            if (Utils.IntersectionMouseRayWithXOZPlane(mainCamera, out Vector3 intersectPointFromMouse))
            {
                clickedPointOnPlaneFromMouse = intersectPointFromMouse;
            }

            if (IntersectionCenterPointRayWithXOZPlane(out Vector3 intersectPointFromCenterPoint))
            {
                clickedPointOnPlaneFromCenterPoint = intersectPointFromCenterPoint;
            }

            clickedMousePosition = Input.mousePosition;

            clickedCameraPosition = transform.position;
            clickedCameraRotation = transform.eulerAngles;

            clickedAngleOfScreenCenterToMouse = AngleOfScreenCenterToMouse();
        }

        isLeftMouseButtonPressed = Input.GetMouseButton((int) MouseButton.MiddleMouse);
        isAltKeyPressed = Input.GetKey(KeyCode.LeftAlt);

        if (isLeftMouseButtonPressed && !isAltKeyPressed)
        {
            Move();
        }
        else if (isLeftMouseButtonPressed)
        {
            Rotate();
        }

        Zoom();
    }

    private void OnDrawGizmos()
    {
        if (!needDrawGizmos)
        {
            return;
        }

        if (Utils.IntersectionMouseRayWithXOZPlane(mainCamera, out Vector3 intersectPointFromMouse))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(intersectPointFromMouse, gizmosSphereSize);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(clickedPointOnPlaneFromMouse, gizmosSphereSize);

        if (IntersectionCenterPointRayWithXOZPlane(out Vector3 intersectPointFromCenterPoint))
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(intersectPointFromCenterPoint, gizmosSphereSize);
        }
    }
}
