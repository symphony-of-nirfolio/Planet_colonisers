using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    [Flags]
    public enum HexType : ushort
    {
        None            = 0b_0000_0000_0000_0000,

        Water           = 0b_0000_0000_0000_0001,
        Metals          = 0b_0000_0000_0000_0010,
        RareMetals      = 0b_0000_0000_0000_0100,
        Dust            = 0b_0000_0000_0000_1000,
        Radioactive     = 0b_0000_0000_0001_0000,

        Land            = 0b_0000_0000_0010_0000,
        Crater          = 0b_0000_0000_0100_0000,
        Mountain        = 0b_0000_0000_1000_0000,
        Building        = 0b_0000_0001_0000_0000,
        Road            = 0b_0000_0010_0000_0000,
        ColonyMainBase  = 0b_0000_0100_0000_0000,

        AllResources    = Water | Metals | RareMetals | Dust | Radioactive,
        BuildingOrRoad  = Building | Road
    }

    public struct HexCell
    {
        public float resourceAmount;
        public short indexInResourceArray;
        public short indexInBuildingArray;
        public short indexInRoadArray;
        public HexType hexType;

        public static HexCell Empty
        {
            get => new HexCell
            {
                resourceAmount = 0f,
                indexInResourceArray = -1,
                indexInBuildingArray = -1,
                indexInRoadArray = -1,
                hexType = HexType.None
            };
        }
    }

    public class WorldAreaInfo
    {
        public HexCell[,] area;
    }

    public class HexCellInfo
    {
        public ResourceDeposit resourceDeposit = null;
        public GameObject building = null;
        public GameObject road = null;
        public string shortDescription = "Unavailable cell";
        public HexType hexType = HexType.None;
    }

    public class HexCellBuildingInfo
    {
        public HexType hexType = HexType.None;
        public ResourceDeposit resourceDeposit = null;
        public GameObject building = null;
        public GameObject road = null;
    }

    public class HexCellsAround
    {
        public HexCellBuildingInfo centerCell = new HexCellBuildingInfo();
        public HexCellBuildingInfo[] cellsAroud = new HexCellBuildingInfo[cellNeighbourAmount];
    }

    public class DoubleCircleOfHexCellsAround
    {
        public HexCellsAround firstCircle;
        public HexCellsAround[] secondCircles = new HexCellsAround[cellNeighbourAmount];
    }


    public const int cellNeighbourAmount = 6;


    public GameParameters gameParameters;
    public Camera resourceCamera;

    public float hexMinRadius = 2f;

    [HideInInspector]
    public WorldAreaInfo worldAreaInfo;
    [HideInInspector]
    public readonly List<ResourceDeposit> resourceDepositArray = new List<ResourceDeposit>();

    [HideInInspector]
    public int width;
    [HideInInspector]
    public int height;
    [HideInInspector]
    public int cellColumns;
    [HideInInspector]
    public int cellRows;

    private readonly float sqrtOfThee = Mathf.Sqrt(3);
    private readonly List<GameObject> buildingArray = new List<GameObject>();
    private readonly List<GameObject> roadArray = new List<GameObject>();

    private static readonly Vector2Int[,] neighbourOffsets = new Vector2Int[,]
    {
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1)
        },
        {
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1)
        }
    };

    private Vector3 offsetToCenter;

    private float hexSideSize;

    private int chankSize;


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

    public static HexType GetResourceOnly(HexType hexType)
    {
        return hexType & HexType.AllResources;
    }

    public static bool IsResourceOnlyType(HexType hexType)
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

    public static bool IsResourceType(HexType hexType)
    {
        HexType resourceOnly = GetResourceOnly(hexType);

        return IsResourceOnlyType(resourceOnly);
    }

    public static bool IsContainBuildingOrRoad(HexType hexType)
    {
        return (hexType & HexType.BuildingOrRoad) != HexType.None;
    }

    public static HexType GetBuildingAndRoadOnly(HexType hexType)
    {
        return hexType & HexType.BuildingOrRoad;
    }

    public static HexType RemoveBuildingAndRoad(HexType hexType)
    {
        return hexType & ~HexType.BuildingOrRoad;
    }

    public static GameResourceType HexTypeToGameResourceType(HexType hexType)
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

    public static HexType GameResourceTypeToHexType(GameResourceType gameResourceType)
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

    public HexType GetHexType(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
            return worldAreaInfo.area[indices.x, indices.y].hexType;
        else
            return HexType.None;
    }

    public bool IsHexNone(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
            return worldAreaInfo.area[indices.x, indices.y].hexType == HexType.None;
        else
            return true;
    }

    public bool IsHexContainsLand(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
            return (worldAreaInfo.area[indices.x, indices.y].hexType & HexType.Land) == HexType.Land;
        else
            return false;
    }

    public bool IsHexContainsResource(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
            return IsResourceType(worldAreaInfo.area[indices.x, indices.y].hexType);
        else
            return false;
    }

    public bool IsHexContainBuildingOrRoad(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
            return IsContainBuildingOrRoad(worldAreaInfo.area[indices.x, indices.y].hexType);
        else
            return false;
    }

    public bool IsHexAvailableForBuildingAndRoad(Vector3 position)
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

    public bool IsHexAvailableForBuilding(Vector3 position)
    {
        return IsHexAvailableForBuildingAndRoad(position);
    }

    public bool IsHexAvailableForRoad(Vector3 position)
    {
        return IsHexAvailableForBuildingAndRoad(position);
    }

    public void AddBuildingToHexCell(Vector3 position, GameObject building)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsHexAvailableForBuilding(position))
        {
            HexCell hexCell = worldAreaInfo.area[indices.x, indices.y];
            hexCell.hexType |= HexType.Building;
            hexCell.indexInBuildingArray = (short) buildingArray.Count;
            worldAreaInfo.area[indices.x, indices.y] = hexCell;

            buildingArray.Add(building);
        }
        else
            Debug.LogError("Hex cell unavalible for building");
    }

    public void AddRoadToHexCell(Vector3 position, GameObject road)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsHexAvailableForRoad(position))
        {
            HexCell hexCell = worldAreaInfo.area[indices.x, indices.y];
            hexCell.hexType |= HexType.Road;
            hexCell.indexInRoadArray = (short) roadArray.Count;
            worldAreaInfo.area[indices.x, indices.y] = hexCell;

            roadArray.Add(road);
        }
        else
            Debug.LogError("Hex cell unavalible for road");
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

    public GameObject GetRoad(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
        {
            int roadIndex = worldAreaInfo.area[indices.x, indices.y].indexInRoadArray;

            if (roadIndex == -1)
            {
                Debug.LogError("Current hex cell doesn\'t contain road");
                return null;
            }
            else
                return roadArray[roadIndex];
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

    public HexCellInfo GetHexCellInfo(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        HexCellInfo hexCellInfo = new HexCellInfo();

        if (IsValidHexIndices(indices))
        {
            HexCell hexCell = worldAreaInfo.area[indices.x, indices.y];
            hexCellInfo.hexType = hexCell.hexType;

            if (IsResourceType(hexCell.hexType))
            {
                hexCellInfo.shortDescription = "Cell with limited mined resource";

                int resourceIndex = worldAreaInfo.area[indices.x, indices.y].indexInResourceArray;
                if (resourceIndex != -1)
                    hexCellInfo.resourceDeposit = resourceDepositArray[resourceIndex];
                else
                    Debug.LogError("Resource index is -1");
            }
            else if ((hexCell.hexType & HexType.Land) == HexType.Land)
                hexCellInfo.shortDescription = "Free cell";
            else if (hexCell.hexType == HexType.Crater)
                hexCellInfo.shortDescription = "Unavailable cell with crater";
            else if (hexCell.hexType == HexType.Mountain)
                hexCellInfo.shortDescription = "Unavailable cell with mountain";

            if ((hexCellInfo.hexType & HexType.Building) == HexType.Building)
            {
                int buildingIndex = worldAreaInfo.area[indices.x, indices.y].indexInBuildingArray;

                if (buildingIndex != -1)
                    hexCellInfo.building = buildingArray[buildingIndex];
                else
                    Debug.LogError("Building index is -1");
            }
            else if ((hexCellInfo.hexType & HexType.Road) == HexType.Road)
            {
                int roadIndex = worldAreaInfo.area[indices.x, indices.y].indexInRoadArray;

                if (roadIndex != -1)
                    hexCellInfo.road = roadArray[roadIndex];
                else
                    Debug.LogError("Road index is -1");
            }
        }

        return hexCellInfo;
    }

    public DoubleCircleOfHexCellsAround GetDoubleCircleOfHexCellsAround(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        DoubleCircleOfHexCellsAround doubleCircleOfHexCellsAround = EmptyDoubleCircleOfHexCellsAround();

        if (IsValidHexIndices(indices))
        {
            doubleCircleOfHexCellsAround.firstCircle = GetHexCellsAround(indices);

            Vector2Int[] hexNeighbourIndices = GetHexNeighbourIndices(indices);
            for (int i = 0; i < cellNeighbourAmount; ++i)
                doubleCircleOfHexCellsAround.secondCircles[i] = GetHexCellsAround(hexNeighbourIndices[i]);
        }

        return doubleCircleOfHexCellsAround;
    }

    public Vector2Int GetHexIndices(Vector3 position)
    {
        float y = 2f / 3f * position.z / hexSideSize;
        float x = (1f / sqrtOfThee * position.x - 1f / 3f * position.z) / hexSideSize;
        Vector2 hex = HexRound(new Vector2(x, y));

        int yHex = Mathf.RoundToInt(hex.y);
        int xHex = Mathf.RoundToInt(hex.x) + yHex / 2;

        return new Vector2Int(xHex, yHex);
    }

    public bool IsHexInsideMap(Vector3 position)
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

    private static Vector2 CubeToAxial(Vector3 cube)
    {
        return new Vector2(cube.x, cube.z);
    }

    private static Vector3 AxialToCube(Vector2 axial)
    {
        float x = axial.x;
        float z = axial.y;
        float y = -x - z;
        return new Vector3(x, y, z);
    }

    private static Vector3 CubeRound(Vector3 cube)
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

    private static Vector2 HexRound(Vector2 hex)
    {
        return CubeToAxial(CubeRound(AxialToCube(hex)));
    }

    public bool IsValidHexIndices(Vector2Int indices)
    {
        return indices.x >= 0 &&
            indices.y >= 0 &&
            indices.x < worldAreaInfo.area.GetLength(0) &&
            indices.y < worldAreaInfo.area.GetLength(1);
    }

    private static HexCellsAround EmptyHexCellsAround()
    {
        HexCellsAround hexCellsAround = new HexCellsAround();
        for (int i = 0; i < cellNeighbourAmount; ++i)
            hexCellsAround.cellsAroud[i] = new HexCellBuildingInfo();
        return hexCellsAround;
    }

    private static DoubleCircleOfHexCellsAround EmptyDoubleCircleOfHexCellsAround()
    {
        DoubleCircleOfHexCellsAround doubleCircleOfHexCellsAround = new DoubleCircleOfHexCellsAround
        {
            firstCircle = EmptyHexCellsAround()
        };

        for (int i = 0; i < cellNeighbourAmount; ++i)
            doubleCircleOfHexCellsAround.secondCircles[i] = EmptyHexCellsAround();
        return doubleCircleOfHexCellsAround;
    }

    private static Vector2Int[] GetHexNeighbourIndices(Vector2Int indices)
    {
        int parity = indices.y & 1;
        Vector2Int[] neighbourIndices = new Vector2Int[cellNeighbourAmount];
        for (int i = 0; i < cellNeighbourAmount; ++i)
            neighbourIndices[i] = indices + neighbourOffsets[parity, i];
        return neighbourIndices;
    }

    private HexCellBuildingInfo GetHexCellBuildingInfo(Vector2Int indices)
    {
        HexCellBuildingInfo hexCellBuildingInfo = new HexCellBuildingInfo();

        if (IsValidHexIndices(indices))
        {
            HexCell hexCell = worldAreaInfo.area[indices.x, indices.y];
            hexCellBuildingInfo.hexType = hexCell.hexType;

            if (IsResourceType(hexCell.hexType))
            {
                int resourceIndex = worldAreaInfo.area[indices.x, indices.y].indexInResourceArray;
                if (resourceIndex != -1)
                    hexCellBuildingInfo.resourceDeposit = resourceDepositArray[resourceIndex];
                else
                    Debug.LogError("Resource index is -1");
            }

            if ((hexCell.hexType & HexType.Building) == HexType.Building)
            {
                int buildingIndex = worldAreaInfo.area[indices.x, indices.y].indexInBuildingArray;

                if (buildingIndex != -1)
                    hexCellBuildingInfo.building = buildingArray[buildingIndex];
                else
                    Debug.LogError("Building index is -1");
            }
            else if ((hexCell.hexType & HexType.Road) == HexType.Road)
            {
                int roadIndex = worldAreaInfo.area[indices.x, indices.y].indexInRoadArray;

                if (roadIndex != -1)
                    hexCellBuildingInfo.road = roadArray[roadIndex];
                else
                    Debug.LogError("Road index is -1");
            }
        }

        return hexCellBuildingInfo;
    }

    private HexCellsAround GetHexCellsAround(Vector2Int indices)
    {
        HexCellsAround hexCellsAround = new HexCellsAround
        {
            centerCell = GetHexCellBuildingInfo(indices)
        };

        Vector2Int[] hexNeighbourIndices = GetHexNeighbourIndices(indices);
        for (int i = 0; i < cellNeighbourAmount; ++i)
            hexCellsAround.cellsAroud[i] = GetHexCellBuildingInfo(hexNeighbourIndices[i]);
        return hexCellsAround;
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


    private void Awake()
    {
        Utils.CheckFieldNotNullAndTryToSet(ref gameParameters, "Game Parameters doesn't set");
        Utils.CheckFieldNotNull(resourceCamera, "Resource Camera doesn't set");

        chankSize = resourceCamera.targetTexture.width / 2;

        int mapSizeToWorldAreaSize(int size) =>
            chankSize * 2 * (1 + (int) (size / 2f / chankSize + 1f));

        width = mapSizeToWorldAreaSize(Mathf.RoundToInt(gameParameters.mapSize.width));
        height = mapSizeToWorldAreaSize(Mathf.RoundToInt(gameParameters.mapSize.height));
        cellColumns = width / 4;
        cellRows = Mathf.RoundToInt(height / 2f / sqrtOfThee);

        offsetToCenter = new Vector3(width / 2f, 0f, height / 2f);

        hexSideSize = hexMinRadius * 2f / sqrtOfThee;

        worldAreaInfo = new WorldAreaInfo
        {
            area = new HexCell[cellColumns, cellRows]
        };
    }
}
