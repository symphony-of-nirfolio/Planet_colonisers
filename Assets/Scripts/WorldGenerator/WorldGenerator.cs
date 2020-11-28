using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Flags]
    public enum HexType : ushort
    {
        None        = 0b_0000_0000_0000_0000,
        Land        = 0b_0000_0000_0000_0001,
        Building    = 0b_0000_0000_0000_0010,
        Water       = 0b_0000_0000_0000_0100,
    }

    public class HexCell
    {
        public float resourceAmount = 0f;
        public int indexInResourceArray = -1;
        public int indexInBuildingArray = -1;
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
    private readonly List<ResourceDeposit> resourceDepositArray = new List<ResourceDeposit>();
    private readonly List<GameObject> buildingArray = new List<GameObject>();

    private Vector3 offsetToCenter;

    private float hexSideSize;

    private int chankSize;
    private int width;
    private int height;


    public Vector2Int GetHexIndices(Vector3 position)
    {
        float y = 2f / 3f * position.z / hexSideSize;
        float x = (1f / sqrtOfThee * position.x - 1f / 3f * position.z) / hexSideSize;
        Vector2 hex = HexRound(new Vector2(x, y));

        int yHex = Mathf.RoundToInt(hex.y);
        int xHex = Mathf.RoundToInt(hex.x) + yHex / 2;

        return new Vector2Int(xHex, yHex);
    }

    public Vector3 GetHexPosition(Vector2Int indices)
    {
        float x = hexSideSize * (sqrtOfThee * (indices.x - indices.y / 2) + sqrtOfThee * 0.5f * indices.y);
        float z = hexSideSize * (1.5f * indices.y);

        return new Vector3(x, 0f, z) - offsetToCenter;
    }

    public Vector3 GetHexCenterPosition(Vector3 position)
    {
        return GetHexPosition(GetHexIndices(position + offsetToCenter));
    }

    public bool IsHexContainsResource(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
            return IsResourceType(worldAreaInfo.area[indices.x, indices.y].hexType);
        else
            return false;
    }

    public bool IsHexAvailableForBuilding(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
        {
            HexType hexType = worldAreaInfo.area[indices.x, indices.y].hexType;

            return hexType == HexType.Land || IsResourceOnlyType(hexType);
        }
        else
            return false;
    }

    public void AddBuildingToHexCell(Vector3 position, GameObject building)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsHexAvailableForBuilding(position))
        {
            HexCell hexCell = worldAreaInfo.area[indices.x, indices.y];
            hexCell.hexType |= HexType.Building;
            hexCell.indexInBuildingArray = buildingArray.Count;
            worldAreaInfo.area[indices.x, indices.y] = hexCell;

            buildingArray.Add(building);
        }
        else
            Debug.LogError("Hex cell unavalible for building");
    }

    public GameObject GetBuilding(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
        {
            int buildingIndex = worldAreaInfo.area[indices.x, indices.y].indexInBuildingArray;

            if (buildingIndex == -1)
            {
                Debug.LogError("Current hex cell doesn\'t contain building");
                return null;
            }
            else
                return buildingArray[buildingIndex];
        }
        else
            return null;
    }

    public GameResourceType GetGameResourceType(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
            return HexTypeToGameResourceType(worldAreaInfo.area[indices.x, indices.y].hexType);
        else
            return HexTypeToGameResourceType(HexType.None);
    }

    public ResourceDeposit GetResourceDeposit(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
        {
            int resourceIndex = worldAreaInfo.area[indices.x, indices.y].indexInResourceArray;

            if (resourceIndex == -1)
            {
                Debug.LogError("Current hex cell doesn\'t contain resource");
                return null;
            }
            else
                return resourceDepositArray[resourceIndex];
        }
        else
            return null;
    }


    private Vector2 CubeToAxial(Vector3 cube)
    {
        return new Vector2(cube.x, cube.z);
    }

    private Vector3 AxialToCube(Vector2 axial)
    {
        float x = axial.x;
        float z = axial.y;
        float y = -x - z;
        return new Vector3(x, y, z);
    }

    private Vector3 CubeRound(Vector3 cube)
    {
        float roundX = Mathf.Round(cube.x);
        float roundY = Mathf.Round(cube.y);
        float roundZ = Mathf.Round(cube.z);

        float xDifference = Mathf.Abs(roundX - cube.x);
        float yDifference = Mathf.Abs(roundY - cube.y);
        float zDifference = Mathf.Abs(roundZ - cube.z);

        if (xDifference > yDifference && xDifference > zDifference)
            roundX = -roundY - roundZ;
        else if (yDifference > zDifference)
            roundY = -roundX - roundZ;
        else
            roundZ = -roundX - roundY;

        return new Vector3(roundX, roundY, roundZ);
    }

    private Vector2 HexRound(Vector2 hex)
    {
        return CubeToAxial(CubeRound(AxialToCube(hex)));
    }

    private bool IsValidHexIndices(Vector2Int indices)
    {
        return indices.x >= 0 &&
            indices.y >= 0 &&
            indices.x < worldAreaInfo.area.GetLength(0) &&
            indices.y < worldAreaInfo.area.GetLength(1);
    }

    private HexType GetResourceOnly(HexType hexType)
    {
        HexType noResourceTypes = HexType.Land | HexType.Building;

        return hexType & ~noResourceTypes;
    }

    private bool IsResourceOnlyType(HexType hexType)
    {
        switch (hexType)
        {
            case HexType.Water:
                return true;
            default:
                return false;
        }
    }

    private bool IsResourceType(HexType hexType)
    {
        HexType resourceOnly = GetResourceOnly(hexType);

        return IsResourceOnlyType(resourceOnly);
    }

    private GameResourceType HexTypeToGameResourceType(HexType hexType)
    {
        switch (hexType)
        {
            case HexType.Water:
                return GameResourceType.Water;
            default:
                Debug.LogWarning("Unresolved hex type");
                return GameResourceType.Dust;
        }
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
                    ResourceDeposit resourceDepositScript = resourceDeposit.GetComponent<ResourceDeposit>();

                    // TODO: add resource amount
                    resourceDepositScript.SetResourceType(HexTypeToGameResourceType(hexCell.hexType));

                    hexCell.indexInResourceArray = resourceDepositArray.Count;
                    resourceDepositArray.Add(resourceDepositScript);
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

        offsetToCenter = new Vector3(width / 2f, 0f, height / 2f);

        hexSideSize = hexMinRadius * 2f / sqrtOfThee;

        InitWorld();
    }
}
