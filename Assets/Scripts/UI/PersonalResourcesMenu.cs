using TMPro;
using UnityEngine;

public class PersonalResourcesMenu : MonoBehaviour
{
    // TODO: replace with original
    // Temporary class to functional check
    public class GameResources
    {
        public const int resourcesCount = 12;

        public int[] resources = new int[resourcesCount];
    }

    public TMP_Text[] resourceCountTexts;


    public void UpdateResources(GameResources gameResources)
    {
        for (int i = 0; i < GameResources.resourcesCount; ++i)
        {
            resourceCountTexts[i].text = gameResources.resources[i].ToString();
        }
    }

    // TODO: remove it
    // Temporary variable to functional check
    private GameResources gameResources = new GameResources();

    private void Start()
    {
        Debug.Assert(resourceCountTexts.Length == GameResources.resourcesCount,
            "Resource Count Text array have to be same size as Game Resources array");
    }

    // TODO: remove it
    // Temporary function to functional check
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            int index = Random.Range(0, 12);
            int resourceValue = Random.Range(1, 100);

            gameResources.resources[index] += resourceValue;

            UpdateResources(gameResources);
        }
    }
}
