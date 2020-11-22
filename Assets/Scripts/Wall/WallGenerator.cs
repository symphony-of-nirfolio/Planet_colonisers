using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public GameParameters gameParameters;

    public GameObject wallPrefab;

    public float wallHeight = 2f;


    private const int wallCount = 4;


    private void InitForwardWall(Vector3[] positions, Vector3[] scales, Quaternion[] rotations, float width, float height, int index = 0)
    {
        positions[index] = new Vector3(0f, wallHeight / 2f, height / 2f);
        scales[index] = new Vector3(width, 1f, wallHeight);
        rotations[index] = Quaternion.Euler(-90f, 0f, 0f);
    }

    private void InitRightWall(Vector3[] positions, Vector3[] scales, Quaternion[] rotations, float width, float height, int index = 1)
    {
        positions[index] = new Vector3(width / 2f, wallHeight / 2f, 0f);
        scales[index] = new Vector3(height, 1f, wallHeight);
        rotations[index] = Quaternion.Euler(-90f, 0f, 90f);
    }

    private void InitBackwardWall(Vector3[] positions, Vector3[] scales, Quaternion[] rotations, float width, float height, int index = 2)
    {
        positions[index] = new Vector3(0f, wallHeight / 2f, -height / 2f);
        scales[index] = new Vector3(width, 1f, wallHeight);
        rotations[index] = Quaternion.Euler(-90f, 0f, 180f);
    }

    private void InitLeftWall(Vector3[] positions, Vector3[] scales, Quaternion[] rotations, float width, float height, int index = 3)
    {
        positions[index] = new Vector3(-width / 2f, wallHeight / 2f, 0f);
        scales[index] = new Vector3(height, 1f, wallHeight);
        rotations[index] = Quaternion.Euler(-90f, 0f, -90f);
    }

    private void Start()
    {
        Debug.Assert(gameParameters, "Game Parameters doesn't set");
        Debug.Assert(wallPrefab, "Wall Prefab doesn't set");

        float width = gameParameters.mapSize.width;
        float height = gameParameters.mapSize.height;

        Vector3[] positions = new Vector3[wallCount];
        Vector3[] scales = new Vector3[wallCount];
        Quaternion[] rotations = new Quaternion[wallCount];

        InitForwardWall(positions, scales, rotations, width, height);
        InitRightWall(positions, scales, rotations, width, height);
        InitBackwardWall(positions, scales, rotations, width, height);
        InitLeftWall(positions, scales, rotations, width, height);

        GameObject walls = new GameObject("Walls");

        for (int index = 0; index < wallCount; ++index)
        {
            GameObject wall = Instantiate(wallPrefab, positions[index], rotations[index], walls.transform);
            Mesh mesh = wall.GetComponentInChildren<MeshFilter>().mesh;

            Vector3 localScale = Vector3.one;
            localScale.x = scales[index].x / mesh.bounds.size.x;
            localScale.z = scales[index].z / mesh.bounds.size.z;

            wall.transform.localScale = localScale;
        }
    }
}
