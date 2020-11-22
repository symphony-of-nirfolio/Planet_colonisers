using UnityEngine;

public class MinimapInitializer : MonoBehaviour
{
    public GameParameters gameParameters;
    public Camera minimapCamera;


    private void Start()
    {
        Debug.Assert(gameParameters, "Game Parameters doesn't set");
        Debug.Assert(minimapCamera, "Minimap Camera doesn't set");

        minimapCamera.orthographicSize = gameParameters.mapSize.height / 2f;
    }
}
