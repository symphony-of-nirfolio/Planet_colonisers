using UnityEngine;

public class BuildHelper : MonoBehaviour
{
    public Renderer currentRenderer;

    public bool IsCollideWithOtherBuildings
    {
        get => collideWithOtherBuildingsCount > 0;
    }

    
    private int collideWithOtherBuildingsCount = 0;

    public void SetMaterialColor(Color color)
    {
        currentRenderer.material.color = color;
    }


    private void Start()
    {
        Debug.Assert(currentRenderer, "Current Renderer doesn't set");
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
}
