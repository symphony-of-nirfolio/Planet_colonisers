using UnityEngine;
using UnityEngine.SceneManagement;

public class GameParametersLoader : MonoBehaviour
{
    public ColonyType colonyType;
    public MapSize mapSize;
    public int colonyAmount;
    public PlanetInfo planetInfo;


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameScene")
        {
            Debug.LogWarning("Wrong scene");
        }
        else
        {
            Debug.Assert(colonyType, "Colony Type doesn't set");
            Debug.Assert(mapSize, "Map Size doesn't set");
            Debug.Assert(colonyAmount > 0, "Colony Amount have to be more than zero");
            Debug.Assert(planetInfo, "Planet Info doesn't set");

            GameParameters gameParameters = FindObjectOfType<GameParameters>();
            gameParameters.colonyType = colonyType;
            gameParameters.mapSize = mapSize;
            gameParameters.colonyAmount = colonyAmount;
            gameParameters.planetInfo = planetInfo;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
}
