using TMPro;
using UnityEngine;

public class ResourceName : MonoBehaviour
{
    public TMP_Text resourceNameText;


    private void Start()
    {
        Debug.Assert(resourceNameText, "Resource Name Text doesn't set");
    }
}
