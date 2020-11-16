using UnityEngine;
using UnityEngine.SceneManagement;

public class GameParametersLoader : MonoBehaviour
{
    // TODO: replace with future class
    public string colonyType = "Default";

    public MapSize mapSize;

    public PlanetInfo planetInfo;


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameScene")
        {
            Debug.LogWarning("Wrong scene");
        }
        else
        {
            Debug.Assert(colonyType == "Default", "Colony Type doesn't set");
            Debug.Assert(mapSize, "Map Size doesn't set");
            Debug.Assert(planetInfo, "Planet Info doesn't set");

            GameParameters gameParameters = FindObjectOfType<GameParameters>();
            gameParameters.colonyType = colonyType;
            gameParameters.mapSize = mapSize;
            gameParameters.planetInfo = planetInfo;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
}
