using UnityEngine;

public class PreparerTo3DSnapshot : MonoBehaviour
{
    public Renderer currentRenderer;
    public Color backgroundColor;

    private Color startedRoadMaterialColor;
    private bool isRoad;


    public void Prepare()
    {
        if (isRoad)
        {
            currentRenderer.material.SetColor("Color_Highlight", backgroundColor);
        }
    }

    public void ResetToPrevious()
    {
        if (isRoad)
        {
            currentRenderer.material.SetColor("Color_Highlight", startedRoadMaterialColor);
        }
    }


    private void Awake()
    {
        if (currentRenderer)
        {
            RoadUpdater roadUpdater = GetComponentInChildren<RoadUpdater>();
            isRoad = roadUpdater != null;

            if (isRoad)
            {
                startedRoadMaterialColor = currentRenderer.material.GetColor("Color_Highlight");
            }
        }
    }
}
