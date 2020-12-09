using UnityEngine;
using UnityEngine.UIElements;

public class HexagonHighlighter : MonoBehaviour
{
    public GameParameters gameParameters;
    public Freezer freezer;
    public Camera mainCamera;
    public WorldGenerator worldGenerator;
    public HexCellInfoMenu hexCellInfoMenu;

    public GameObject highlighter;
    public Renderer currentRenderer;

    public Color emptyHexColor = Color.white;
    public Color resourceHexColor = Color.green;
    public Color noneHexColor = Color.red;

    public float moveSmoothTime = 0.05f;
    public float colorChangeSpeed = 7f;

    public float holdingDurationToShowInfo = 0.4f;


    private Material currentMaterial;

    private Color previousColor = Color.white;

    private Vector3 currentVelocity;

    private Vector3 previousMousePosition = Vector3.zero;
    private float holdingTime = 0f;
    private bool isShowingInfo = false;


    private bool IsValideMouseInput()
    {
        if (previousMousePosition != Input.mousePosition)
        {
            return false;
        }

        if (Input.GetMouseButton((int) MouseButton.LeftMouse) ||
            Input.GetMouseButton((int) MouseButton.MiddleMouse) ||
            Input.GetMouseButton((int) MouseButton.RightMouse) ||
            Input.mouseScrollDelta.sqrMagnitude > Mathf.Epsilon)
        {
            return false;
        }

        return true;
    }

    private void HandlingMouseInput(Vector3 enter)
    {
        if (IsValideMouseInput())
        {
            if (Time.time - holdingTime >= holdingDurationToShowInfo)
            {
                if (!isShowingInfo)
                {
                    isShowingInfo = true;

                    hexCellInfoMenu.Show(worldGenerator.worldMap.GetHexCellInfo(enter));
                }
            }
            else
            {
                if (isShowingInfo)
                {
                    isShowingInfo = false;
                    hexCellInfoMenu.Hide();
                }
            }
        }
        else
        {
            previousMousePosition = Input.mousePosition;
            holdingTime = Time.time;
            if (isShowingInfo)
            {
                isShowingInfo = false;
                hexCellInfoMenu.Hide();
            }
        }
    }


    private void Start()
    {
        if (!freezer)
        {
            Debug.LogError("Freezer doesn't set");
            freezer = FindObjectOfType<Freezer>();
        }
        if (!hexCellInfoMenu)
        {
            Debug.LogError("Hex Cell Info Menu doesn't set");
            hexCellInfoMenu = FindObjectOfType<HexCellInfoMenu>();
        }

        Debug.Assert(gameParameters, "Game Parameters doesn't set");
        Debug.Assert(mainCamera, "Game Parameters doesn't set");
        Debug.Assert(worldGenerator, "World Generator doesn't set");
        Debug.Assert(highlighter, "Highlighter doesn't set");
        Debug.Assert(currentRenderer, "Current Renderer doesn't set");

        currentMaterial = currentRenderer.sharedMaterial;
    }

    private void Update()
    {
        if (!freezer.IsInteractionFreeze)
        {
            if (Utils.IntersectionMouseRayWithXOZPlane(mainCamera, out Vector3 enter))
            {
                highlighter.SetActive(true);

                Vector3 hexCenter = worldGenerator.worldMap.GetHexCenterPosition(enter);
                HandlingMouseInput(enter);

                bool isHexContainsLand = worldGenerator.worldMap.IsHexContainsLand(enter);
                bool isHexContainsResource = worldGenerator.worldMap.IsHexContainsResource(enter);

                Color hexColor;
                if (isHexContainsLand)
                    hexColor = emptyHexColor;
                else if (isHexContainsResource)
                    hexColor = resourceHexColor;
                else
                    hexColor = noneHexColor;

                previousColor = Color.Lerp(previousColor, hexColor, Time.deltaTime * colorChangeSpeed);
                currentMaterial.SetColor("Color_Highlight", previousColor);

                transform.position = Vector3.SmoothDamp(transform.position, hexCenter, ref currentVelocity, moveSmoothTime);
            }
            else
            {
                if (isShowingInfo)
                {
                    isShowingInfo = false;
                    hexCellInfoMenu.Hide();
                }
                highlighter.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        currentMaterial.SetColor("Color_Highlight", Color.white);
    }
}
