using UnityEngine;

public class HexagonHighlighter : MonoBehaviour
{
    public GameParameters gameParameters;
    public Camera mainCamera;
    public WorldGenerator worldGenerator;

    public GameObject highlighter;
    public Renderer currentRenderer;

    public Color emptyHexColor = Color.white;
    public Color resourceHexColor = Color.green;

    public float moveSmoothTime = 0.05f;
    public float colorChangeSpeed = 7f;


    private Material currentMaterial;

    private Color previousColor = Color.white;

    private Vector3 currentVelocity;


    private void Start()
    {
        Debug.Assert(gameParameters, "Game Parameters doesn't set");
        Debug.Assert(mainCamera, "Game Parameters doesn't set");
        Debug.Assert(worldGenerator, "World Generator doesn't set");
        Debug.Assert(highlighter, "Highlighter doesn't set");
        Debug.Assert(currentRenderer, "Current Renderer doesn't set");

        currentMaterial = currentRenderer.sharedMaterial;
    }

    private void Update()
    {
        if (Utils.IntersectionMouseRayWithXOZPlane(mainCamera, out Vector3 enter))
        {
            highlighter.SetActive(true);

            Vector3 hexCenter = worldGenerator.GetHexCenterPosition(enter);
            bool isHexContainsResource = worldGenerator.IsHexContainsResource(enter);

            Color hexColor = isHexContainsResource ? resourceHexColor : emptyHexColor;
            previousColor = Color.Lerp(previousColor, hexColor, Time.deltaTime * colorChangeSpeed);
            currentMaterial.SetColor("Color_Highlight", previousColor);

            transform.position = Vector3.SmoothDamp(transform.position, hexCenter, ref currentVelocity, moveSmoothTime);
        }
        else
            highlighter.SetActive(false);
    }
}
