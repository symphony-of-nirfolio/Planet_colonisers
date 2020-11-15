using TMPro;
using UnityEngine;

public class SmallPlanetController : MonoBehaviour
{
    public GameObject planet;
    public Canvas canvas;
    public TMP_Text planetName;
    public TMP_Text select;

    [HideInInspector]
    public bool isSelected = false;
    

    public void SetMaterial(Material material)
    {
        planet.GetComponent<Renderer>().material = material;
    }

    public void SetCamera(Camera camera)
    {
        canvas.worldCamera = camera;
    }

    public void SetName(string name)
    {
        planetName.text = name;
    }

    public void Select()
    {
        isSelected = true;
        select.text = "Selected";
    }

    public void Unselect()
    {
        isSelected = false;
        select.text = "";
    }


    private void Start()
    {
        Debug.Assert(planet, "Planet doesn't set");
        Debug.Assert(canvas, "Canvas doesn't set");
        Debug.Assert(planetName, "Planet Name doesn't set");
        Debug.Assert(select, "Select doesn't set");
    }
}
