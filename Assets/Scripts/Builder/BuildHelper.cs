using UnityEngine;

public class BuildHelper : MonoBehaviour
{
    public Renderer currentRenderer;

    public bool IsCollideWithOtherBuildings
    {
        get => collideWithOtherBuildingsCount > 0;
    }

    
    private int collideWithOtherBuildingsCount = 0;

    private Color startedRoadMaterialColor;
    private bool isRoad;

    public void SetMaterialColor(Color color)
    {
        if (isRoad)
        {
            currentRenderer.material.SetColor("Color_Highlight", color);
        }
        else
        {
            currentRenderer.material.color = color;
        }
    }


    private void Start()
    {
        Debug.Assert(currentRenderer, "Current Renderer doesn't set");
        isRoad = TryGetComponent(out RoadUpdater _);

        if (isRoad)
        {
            startedRoadMaterialColor = currentRenderer.material.GetColor("Color_Highlight");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == gameObject.layer)
        {
            ++collideWithOtherBuildingsCount;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == gameObject.layer)
        {
            --collideWithOtherBuildingsCount;
        }
    }

    private void OnDisable()
    {
        if (isRoad)
        {
            currentRenderer.material.SetColor("Color_Highlight", startedRoadMaterialColor);
        }
    }
}
