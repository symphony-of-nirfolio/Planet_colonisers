using UnityEngine;

public class AvailableHex : MonoBehaviour
{
    public Renderer currentRenderer;

    public Color playerBase = new Color(1f, 1f, 1f, 0.3f);
    public Color playerHighlight = new Color(1f, 1f, 1f, 0.8f);
    public Color npcBase = new Color(1f, 1f, 0f, 0.3f);
    public Color npcHighlight = new Color(1f, 1f, 0f, 0.8f);


    private Color startedMaterialColor;


    public void SetPlayerBaseColor()
    {
        currentRenderer.material.SetColor("Color_Highlight", playerBase);
    }
    
    public void SetPlayerHighlightColor()
    {
        currentRenderer.material.SetColor("Color_Highlight", playerHighlight);
    }

    public void SetNpcBaseColor()
    {
        currentRenderer.material.SetColor("Color_Highlight", npcBase);
    }
    
    public void SetNpcHighlightColor()
    {
        currentRenderer.material.SetColor("Color_Highlight", npcHighlight);
    }

    private void Start()
    {
        startedMaterialColor = currentRenderer.material.GetColor("Color_Highlight");
    }

    private void OnDisable()
    {
        currentRenderer.material.SetColor("Color_Highlight", startedMaterialColor);
    }
}
