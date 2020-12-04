using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class MaterialFontApplier : MonoBehaviour
{
    public Material fontMaterial;


    private void Start()
    {
        if (!fontMaterial)
        {
            Debug.LogError("Font Material doesn't set");
            return;
        }

        GetComponent<TMP_Text>().fontSharedMaterial = fontMaterial;
    }
}
