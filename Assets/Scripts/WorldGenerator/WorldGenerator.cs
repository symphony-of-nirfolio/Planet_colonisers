using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldGenerator : MonoBehaviour
{
    [Flags]
    public enum HexType : ushort
    {
        None            = 0b_0000_0000_0000_0000,
        Land            = 0b_0000_0000_0000_0001,
        Building        = 0b_0000_0000_0000_0010,
        ColonyMainBase  = 0b_0000_0000_0000_0100,

        Water           = 0b_0000_0000_0000_1000,
        Metals          = 0b_0000_0000_0001_0000,
        RareMetals      = 0b_0000_0000_0010_0000,
        Dust            = 0b_0000_0000_0100_0000,
        Radioactive     = 0b_0000_0000_1000_0000,
        
        AllResources    = Water | Metals | RareMetals | Dust | Radioactive
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
    public LimitedMinedResourceInfoList limitedMinedResourceInfoList;
    public Camera resourceCamera;
    public Renderer resourceRenderer;
    public Transform resourcesTransform;
    public Transform mainBaseLocationsTransform;
    public GameObject resourceDepositPrefab;
    public GameObject mainBaseLocationPrefab;

    public int seed = 1;

    public int distanceBetweenColony = 2;

    public float hexMinRadius = 1;

    public float defaultRiddling = 0.001f;
    public float mainBaseRiddling = 0.0003f;


    private readonly float sqrtOfThee = Mathf.Sqrt(3);
    private readonly int waterResourceOffset = 1;
    private readonly int coloniesOffset = 10;

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
    private int colonyAmount;


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

    public bool IsHexNone(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
            return worldAreaInfo.area[indices.x, indices.y].hexType == HexType.None;
        else
            return true;
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
        return hexType & HexType.AllResources;
    }

    private bool IsResourceOnlyType(HexType hexType)
    {
        switch (hexType)
        {
            case HexType.Water:
            case HexType.Metals:
            case HexType.RareMetals:
            case HexType.Dust:
            case HexType.Radioactive:
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
            case HexType.Metals:
                return GameResourceType.Metals;
            case HexType.RareMetals:
                return GameResourceType.RareMetals;
            case HexType.Dust:
                return GameResourceType.Dust;
            case HexType.Radioactive:
                return GameResourceType.Radioactive;
            default:
                Debug.LogWarning("Unresolved hex type");
                return GameResourceType.Dust;
        }
    }

    private HexType GameResourceTypeToHexType(GameResourceType gameResourceType)
    {
        switch (gameResourceType)
        {
            case GameResourceType.Water:
                return HexType.Water;
            case GameResourceType.Metals:
                return HexType.Metals;
            case GameResourceType.RareMetals:
                return HexType.RareMetals;
            case GameResourceType.Dust:
                return HexType.Dust;
            case GameResourceType.Radioactive:
                return HexType.Radioactive;
            default:
                Debug.LogWarning("Unresolved game resource type");
                return HexType.None;
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

    private bool IsInsideMap(Vector3 position)
    {
        float width = gameParameters.mapSize.width;
        float height = gameParameters.mapSize.height;
        return position.x >= -width / 2f - Mathf.Epsilon &&
            position.x <= width / 2f + Mathf.Epsilon &&
            position.z >= -height / 2f - Mathf.Epsilon &&
            position.z <= height / 2f + Mathf.Epsilon;
    }

    private bool IsHexInsideMap(Vector3 position)
    {
        float hexMaxRadius = 2f / sqrtOfThee * hexMinRadius;

        int vertexAmount = 6;
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(0f, 0f, hexMaxRadius),
            new Vector3(hexMinRadius, 0f, hexMaxRadius / 2f),
            new Vector3(hexMinRadius, 0f, -hexMaxRadius / 2f),
            new Vector3(0f, 0f, -hexMaxRadius),
            new Vector3(-hexMinRadius, 0f, -hexMaxRadius / 2f),
            new Vector3(-hexMinRadius, 0f, hexMaxRadius / 2f),
        };
        
        for (int i = 0; i < vertexAmount; ++i)
            if (!IsInsideMap(position + offsets[i]))
                return false;

        return true;
    }

    private void InitLand()
    {
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
            {
                Vector2Int indices = new Vector2Int(x, y);
                Vector3 centerPosition = GetHexPosition(indices);

                HexCell hexCell;
                if (IsHexInsideMap(centerPosition))
                    hexCell = new HexCell
                    {
                        hexType = HexType.Land
                    };
                else
                    hexCell = new HexCell();
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

    private void InitResource(HexType hexType, float riddling, int resourceSeed)
    {
        resourceMaterial.SetFloat("Vector1_Riddling", riddling);
        resourceMaterial.SetFloat("Vector1_Seed", resourceSeed);

        for (int chankX = 0; chankX < width / chankSize; ++chankX)
            for (int chankY = 0; chankY < height / chankSize; ++chankY)
                InitResourceChank(hexType, chankX, chankY);
    }

    private void InitResources(int resourcesSeed)
    {
        int resourceSeed = resourcesSeed;
        foreach (LimitedMinedResourceInfo resourceInfo in limitedMinedResourceInfoList.resourceInfos)
        {
            InitResource(GameResourceTypeToHexType(resourceInfo.gameResourceType), resourceInfo.riddling, resourceSeed);
            ++resourceSeed;
        }
    }

    private bool IsValidDistanceToOtherColony(List<Vector2Int> colonyIndices, Vector2Int newColony, int minDistanceBetweenColony)
    {
        foreach (Vector2Int currentColony in colonyIndices)
        {
            int xDistance = Mathf.Abs(currentColony.x - newColony.x);
            int yDistance = Mathf.Abs(currentColony.y - newColony.y);
            int distanceSqr = xDistance * xDistance + yDistance * yDistance;

            if (distanceSqr < (minDistanceBetweenColony + 1) * (minDistanceBetweenColony + 1))
                return false;
        }
        return true;
    }

    private void AddColonyMainBases(List<Vector2Int> colonyIndices, int offsettedColoniesSeed)
    {
        InitResource(HexType.ColonyMainBase, mainBaseRiddling, offsettedColoniesSeed);

        List<Vector2Int> allColonyIndices = new List<Vector2Int>();

        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
            {
                HexCell hexCell = worldAreaInfo.area[x, y];

                if (hexCell.hexType == HexType.ColonyMainBase)
                {
                    allColonyIndices.Add(new Vector2Int(x, y));

                    hexCell.hexType = HexType.Land;
                    worldAreaInfo.area[x, y] = hexCell;
                }
            }

        Utils.Shuffle(allColonyIndices, offsettedColoniesSeed);
        List<Vector2Int> unusedColonyIndices = new List<Vector2Int>();

        int minDistanceBetweenColony = distanceBetweenColony;
        void AddCorrectIndices()
        {
            for (int i = 0; i < allColonyIndices.Count && colonyIndices.Count != colonyAmount; ++i)
            {
                Vector2Int currentIndices = allColonyIndices[i];

                if (IsValidDistanceToOtherColony(colonyIndices, currentIndices, distanceBetweenColony))
                    colonyIndices.Add(currentIndices);
                else
                    unusedColonyIndices.Add(currentIndices);
            }
        }

        AddCorrectIndices();

        --minDistanceBetweenColony;
        while (colonyIndices.Count != colonyAmount && unusedColonyIndices.Count != 0 && minDistanceBetweenColony >= 1)
        {
            allColonyIndices = unusedColonyIndices;
            unusedColonyIndices = new List<Vector2Int>();
            
            AddCorrectIndices();

            --minDistanceBetweenColony;
        }
    }

    private void InitColonyMainBases(int coloniesSeed)
    {
        List<Vector2Int> colonyIndices = new List<Vector2Int>();

        int offsettedColoniesSeed = coloniesSeed;
        int previousAmount = 0;
        while (colonyIndices.Count != colonyAmount)
        {
            AddColonyMainBases(colonyIndices, offsettedColoniesSeed);
            ++offsettedColoniesSeed;
            if (previousAmount == colonyIndices.Count)
            {
                Debug.LogError("Cannot add one more colony");
                break;
            }
            previousAmount = colonyIndices.Count;
        }

        foreach (Vector2Int currentColony in colonyIndices)
        {
            HexCell hexCell = new HexCell
            {
                hexType = HexType.ColonyMainBase
            };

            worldAreaInfo.area[currentColony.x, currentColony.y] = hexCell;
        }
    }

    private void SetResourcePrefabs()
    {
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
            {
                HexCell hexCell = worldAreaInfo.area[x, y];
                if (IsResourceOnlyType(hexCell.hexType))
                {
                    GameResourceType gameResourceType = HexTypeToGameResourceType(hexCell.hexType);

                    GameObject resourceDeposit = Instantiate(resourceDepositPrefab,
                        GetHexPosition(new Vector2Int(x, y)),
                        Quaternion.identity,
                        resourcesTransform);

                    ResourceSprite resourceSprite = resourceDeposit.GetComponent<ResourceSprite>();
                    resourceSprite.InitWithGameResourceType(gameResourceType);

                    ResourceDeposit resourceDepositScript = resourceDeposit.GetComponent<ResourceDeposit>();
                    // TODO: add random resource amount
                    resourceDepositScript.SetResourceType(gameResourceType, 5000);

                    hexCell.indexInResourceArray = resourceDepositArray.Count;
                    resourceDepositArray.Add(resourceDepositScript);
                }
                else if (hexCell.hexType == HexType.ColonyMainBase)
                {
                    Instantiate(mainBaseLocationPrefab,
                        GetHexPosition(new Vector2Int(x, y)),
                        Quaternion.identity,
                        mainBaseLocationsTransform);
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

        InitResources(seed + waterResourceOffset);

        InitColonyMainBases(seed + coloniesOffset);

        SetResourcePrefabs();
    }


    private void Start()
    {
        Debug.Assert(gameParameters, "Game Parameters doesn't set");
        Debug.Assert(mainCamera, "Main Camera doesn't set");
        Debug.Assert(limitedMinedResourceInfoList, "Limited Mined Resource Info List doesn't set");
        Debug.Assert(resourceCamera, "Resource Camera doesn't set");
        Debug.Assert(resourceRenderer, "Resource Renderer doesn't set");
        Debug.Assert(resourcesTransform, "Resources Transform doesn't set");
        Debug.Assert(mainBaseLocationsTransform, "Main Base Locations Transform doesn't set");
        Debug.Assert(resourceDepositPrefab, "Resource Deposit Prefab doesn't set");
        Debug.Assert(mainBaseLocationPrefab, "Main Base Location Prefab doesn't set");

        renderTexture = resourceCamera.targetTexture;
        resourceMaterial = resourceRenderer.sharedMaterial;

        chankSize = renderTexture.width / 2;

        int mapSizeToWorldAreaSize(int size) =>
            chankSize * 2 * (1 + (int) (size / 2f / chankSize + 1f));

        width = mapSizeToWorldAreaSize(Mathf.RoundToInt(gameParameters.mapSize.width));
        height = mapSizeToWorldAreaSize(Mathf.RoundToInt(gameParameters.mapSize.height));

        offsetToCenter = new Vector3(width / 2f, 0f, height / 2f);

        hexSideSize = hexMinRadius * 2f / sqrtOfThee;

        colonyAmount = gameParameters.colonyAmount;

        InitWorld();
    }

    private void OnDisable()
    {
        resourceMaterial.SetFloat("Vector1_Riddling", defaultRiddling);
        resourceMaterial.SetFloat("Vector1_Seed", 0);
        resourceMaterial.SetVector("Vector2_Offset", Vector4.zero);
    }
}
