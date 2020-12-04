using TMPro;
using UnityEngine;

public class PersonalResourcesMenu : MonoBehaviour
{
    public TMP_Text[] resourceCountTexts;


    public void UpdateResources(GameResourcesStorage storage)
    {
        for (int i = 0; i < GameResourcesStorage.resourceCount; ++i)
        {
            resourceCountTexts[i].text = storage.getResourceAmount((GameResourceType)i).ToString();
        }
    }

   

    private void Start() { }

    
}
