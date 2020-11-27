using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public enum HexType : byte
    {
        Land,
        Water,
        None,
        CountOf,
    }

    public class HexCell
    {
        public float resourceAmount = 0f;
        public int indexInResourceArray = -1;
        public HexType hexType = HexType.None;
    }

    public class WorldAreaInfo
    {
        public HexCell[,] area;
    }

    public GameParameters gameParameters;
    public Camera mainCamera;
    public Camera resourceCamera;
    public Renderer resourceRenderer;
    public Transform resourcesTransform;
    public GameObject resourceDepositPrefab;

    public int seed = 1;

    public float hexMinRadius = 1;


    private readonly float sqrtOfThee = Mathf.Sqrt(3);

    private RenderTexture renderTexture;
    private Material resourceMaterial;

    private WorldAreaInfo worldAreaInfo;
    private readonly List<GameObject> resourceDepositArray = new List<GameObject>();

    private float hexSideSize;

    private int chankSize;
    private int width;
    private int height;


    public Vector2Int GetHexIndices(Vector3 position)
    {
        int y = Mathf.RoundToInt(2f / 3f * position.z / hexSideSize);
        int x = Mathf.RoundToInt((1f / sqrtOfThee * position.x - 1f / 3f * position.z) / hexSideSize) + y / 2;

        return new Vector2Int(x, y);
    }

    public Vector3 GetHexPosition(Vector2Int indices)
    {
        float x = hexSideSize * (sqrtOfThee * (indices.x - indices.y / 2) + sqrtOfThee * 0.5f * indices.y);
        float z = hexSideSize * (1.5f * indices.y);

        Vector3 offset = new Vector3(width / 2f, 0f, height / 2f);
        return new Vector3(x, 0f, z) - offset;
    }

    public Vector3 GetHexCenterPosition(Vector3 position)
    {
        Vector3 offset = new Vector3(width / 2f, 0f, height / 2f);
        return GetHexPosition(GetHexIndices(position + offset));
    }


    private bool IsValidHexIndices(Vector2Int indices)
    {
        return indices.x >= 0 &&
            indices.y >= 0 &&
            indices.x < worldAreaInfo.area.GetLength(0) &&
            indices.y < worldAreaInfo.area.GetLength(1);
    }

    private Color32[] TakeResourceSnapshot(int xOffset, int yOffset)
    {
        resourceCamera.gameObject.SetActive(true);

        resourceMaterial.SetVector("Vector2_Offset", new Vector4(xOffset, yOffset, 0f, 0f));
        Texture2D snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        resourceCamera.Render();
        RenderTexture.active = renderTexture;
        snapshot.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);

        resourceCamera.gameObject.SetActive(false);

        return snapshot.GetPixels32();
    }


    private void InitLand()
    {
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
            {
                HexCell hexCell = new HexCell
                {
                    hexType = HexType.Land
                };
                worldAreaInfo.area[x, y] = hexCell;
            }
    }

    private void InitResourceChank(HexType hexType, int chankX, int chankY)
    {
        int currentXOffset = chankX * chankSize;
        int currentYOffset = chankY * chankSize;

        Color32[] colors = TakeResourceSnapshot(chankX, chankY);

        for (int pixelX = 0; pixelX < renderTexture.width; ++pixelX)
            for (int pixelY = 0; pixelY < renderTexture.height; ++pixelY)
            {
                Color32 color = colors[pixelX + pixelY * renderTexture.width];

                Vector3 position = new Vector3(pixelX + currentXOffset, 0f, pixelY + currentYOffset);
                Vector2Int indices = GetHexIndices(position);

                if (IsValidHexIndices(indices))
                {
                    HexCell hexCell = worldAreaInfo.area[indices.x, indices.y];

                    if (color.r != 0)
                    {
                        if (hexCell.hexType == hexType)
                        {
                            hexCell.resourceAmount += color.r;

                            worldAreaInfo.area[indices.x, indices.y] = hexCell;
                        }
                        else if (hexCell.hexType == HexType.Land)
                        {
                            hexCell.hexType = hexType;
                            hexCell.resourceAmount += color.r;

                            worldAreaInfo.area[indices.x, indices.y] = hexCell;
                        }
                    }
                }
            }
    }

    private void InitResource(HexType hexType, int resourceSeed)
    {
        resourceMaterial.SetFloat("Vector1_Seed", resourceSeed);

        for (int chankX = 0; chankX < width / chankSize; ++chankX)
            for (int chankY = 0; chankY < height / chankSize; ++chankY)
                InitResourceChank(hexType, chankX, chankY);

        resourceMaterial.SetVector("Vector2_Offset", Vector4.zero);
    }

    private void SetResourcePrefabs()
    {
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
            {
                HexCell hexCell = worldAreaInfo.area[x, y];
                if (hexCell.hexType == HexType.Water)
                {
                    GameObject resourceDeposit = Instantiate(resourceDepositPrefab,
                        GetHexPosition(new Vector2Int(x, y)),
                        Quaternion.identity,
                        resourcesTransform);
                    ResourceName resourceName = resourceDeposit.GetComponent<ResourceName>();
                    resourceName.resourceNameText.text = "Water";
                    resourceDeposit.GetComponentInChildren<Canvas>().worldCamera = mainCamera;

                    hexCell.indexInResourceArray = resourceDepositArray.Count;
                    resourceDepositArray.Add(resourceDeposit);
                }
            }
    }

    private void InitWorld()
    {
        worldAreaInfo = new WorldAreaInfo
        {
            area = new HexCell[width, height]
        };

        InitLand();

        InitResource(HexType.Water, seed + 1);

        SetResourcePrefabs();
    }


    private void Start()
    {
        Debug.Assert(gameParameters, "Game Parameters doesn't set");
        Debug.Assert(mainCamera, "Main Camera doesn't set");
        Debug.Assert(resourceCamera, "Resource Camera doesn't set");
        Debug.Assert(resourceRenderer, "Resource Renderer doesn't set");
        Debug.Assert(resourcesTransform, "Resources Transform doesn't set");
        Debug.Assert(resourceDepositPrefab, "Resource Deposit Prefab doesn't set");

        renderTexture = resourceCamera.targetTexture;
        resourceMaterial = resourceRenderer.sharedMaterial;

        chankSize = renderTexture.width / 2;

        int mapSizeToWorldAreaSize(int size) =>
            chankSize * 2 * (1 + (int) (size / 2f / chankSize + 1f));

        width = mapSizeToWorldAreaSize(Mathf.RoundToInt(gameParameters.mapSize.width));
        height = mapSizeToWorldAreaSize(Mathf.RoundToInt(gameParameters.mapSize.height));

        hexSideSize = hexMinRadius * 2f / sqrtOfThee;

        InitWorld();
    }
}
