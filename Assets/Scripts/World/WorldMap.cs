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
        public short indexInColonyMainBaseArray;
        public short indexInBuildingArray;
        public short indexInRoadArray;
        public HexType hexType;

        public static HexCell Empty
        {
            get => new HexCell
            {
                resourceAmount = 0f,
                indexInResourceArray = -1,
                indexInColonyMainBaseArray = -1,
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
        public GameObject colonyMainBase = null;
        public GameObject building = null;
        public GameObject road = null;
        public Vector3 position = Vector3.zero;
        public string shortDescription = "Unavailable cell";
        public HexType hexType = HexType.None;
    }

    public class HexCellsAround
    {
        public HexCellInfo centerCell = new HexCellInfo();
        public HexCellInfo[] cellsAroud = new HexCellInfo[cellNeighbourAmount];
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
    public readonly List<GameObject> colonyMainBaseArray = new List<GameObject>();
    [HideInInspector]
    public readonly List<Vector2Int> resourceDepositIndicesArray = new List<Vector2Int>();
    [HideInInspector]
    public readonly List<Vector2Int> colonyMainBaseIndicesArray = new List<Vector2Int>();

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

    private readonly List<Vector2Int> buildingIndicesArray = new List<Vector2Int>();
    private readonly List<Vector2Int> roadIndicesArray = new List<Vector2Int>();


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
            buildingIndicesArray.Add(indices);
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
            roadIndicesArray.Add(indices);
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

    public GameObject GetColonyMainBase(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        if (IsValidHexIndices(indices))
        {
            int indexInColonyMainBaseArray = worldAreaInfo.area[indices.x, indices.y].indexInColonyMainBaseArray;

            if (indexInColonyMainBaseArray == -1)
            {
                Debug.LogError("Current hex cell doesn\'t contain colony main base array");
                return null;
            }
            else
                return colonyMainBaseArray[indexInColonyMainBaseArray];
        }
        else
            return null;
    }

    public Vector3 GetPositoinByBuilding(GameObject building)
    {
        return GetHexPosition(GetIndicesByBuilding(building));
    }
    
    public Vector3 GetPositoinByRoad(GameObject road)
    {
        return GetHexPosition(GetIndicesByRoad(road));
    }

    public Vector3 GetPositoinByColonyMainBase(GameObject colonyMainBase)
    {
        return GetHexPosition(GetIndicesByColonyMainBase(colonyMainBase));
    }

    public Vector3 GetPositoinByResourceDeposit(ResourceDeposit resourceDeposit)
    {
        return GetHexPosition(GetIndicesByResourceDeposit(resourceDeposit));
    }

    public HexCellInfo GetHexCellInfo(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        return GetHexCellInfo(indices);
    }

    public DoubleCircleOfHexCellsAround GetDoubleCircleOfHexCellsAround(Vector3 position)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        DoubleCircleOfHexCellsAround doubleCircleOfHexCellsAround = EmptyDoubleCircleOfHexCellsAround();

        if (IsValidHexIndices(indices))
        {
            doubleCircleOfHexCellsAround.firstCircle = GetHexCellsAround(indices);

            Vector2Int[] hexNeighbourIndices = GetHexNeighboursIndices(indices);
            for (int i = 0; i < cellNeighbourAmount; ++i)
                doubleCircleOfHexCellsAround.secondCircles[i] = GetHexCellsAround(hexNeighbourIndices[i]);
        }

        return doubleCircleOfHexCellsAround;
    }

    public Vector3[] GetHexRing(Vector3 position, int radius)
    {
        return GetHexRings(position, radius, radius);
    }

    public Vector3[] GetHexRings(Vector3 position, int minRadius, int maxRadius)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        Vector2Int[] ringIndices = GetHexIndicesRings(indices, minRadius, maxRadius);

        List<Vector3> result = new List<Vector3>();

        foreach (Vector2Int ring in ringIndices)
            if (IsValidHexIndices(ring) && (worldAreaInfo.area[ring.x, ring.y].hexType & HexType.Land) == HexType.Land)
                result.Add(GetHexPosition(ring));

        return result.ToArray();
    }

    public HexCellInfo[] GetHexInfoRings(Vector3 position, int minRadius, int maxRadius)
    {
        Vector2Int indices = GetHexIndices(position + offsetToCenter);

        Vector2Int[] ringIndices = GetHexIndicesRings(indices, minRadius, maxRadius);

        List<HexCellInfo> result = new List<HexCellInfo>();

        foreach (Vector2Int ring in ringIndices)
            if (IsValidHexIndices(ring))
                result.Add(GetHexCellInfo(ring));

        return result.ToArray();
    }

    public int Distance(Vector3 begin, Vector3 end)
    {
        return Distance(GetHexIndices(begin + offsetToCenter), GetHexIndices(end + offsetToCenter));
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

    private static Vector2Int CubeToOffset(Vector3Int cube)
    {
        int x = cube.x + (cube.z - (cube.z & 1)) / 2;
        int y = cube.z;
        return new Vector2Int(x, y);
    }

    private static Vector3Int OffsetToCube(Vector2Int hex)
    {
        int x = hex.x - (hex.y - (hex.y & 1)) / 2;
        int z = hex.y;
        int y = -x - z;
        return new Vector3Int(x, y, z);
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
            hexCellsAround.cellsAroud[i] = new HexCellInfo();
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

    private static Vector2Int[] GetHexNeighboursIndices(Vector2Int indices)
    {
        int parity = indices.y & 1;
        Vector2Int[] neighbourIndices = new Vector2Int[cellNeighbourAmount];
        for (int i = 0; i < cellNeighbourAmount; ++i)
            neighbourIndices[i] = indices + neighbourOffsets[parity, i];
        return neighbourIndices;
    }

    private static Vector2Int GetHexNeighbourIndices(Vector2Int indices, int neighbourId)
    {
        if (neighbourId < 0 || neighbourId >= cellNeighbourAmount)
        {
            Debug.LogError("Wrong neighbour id");
            return Vector2Int.zero;
        }

        int parity = indices.y & 1;
        return indices + neighbourOffsets[parity, neighbourId];
    }

    private Vector2Int GetIndicesByBuilding(GameObject building)
    {
        if (buildingArray.Contains(building))
        {
            int index = buildingArray.FindIndex(x => x == building);
            return buildingIndicesArray[index];
        }
        else
        {
            Debug.LogError("Current building isn't contains in array");
            return Vector2Int.zero;
        }
    }

    private Vector2Int GetIndicesByRoad(GameObject road)
    {
        if (roadArray.Contains(road))
        {
            int index = roadArray.FindIndex(x => x == road);
            return roadIndicesArray[index];
        }
        else
        {
            Debug.LogError("Current road isn't contains in array");
            return Vector2Int.zero;
        }
    }

    private Vector2Int GetIndicesByColonyMainBase(GameObject colonyMainBase)
    {
        if (colonyMainBaseArray.Contains(colonyMainBase))
        {
            int index = colonyMainBaseArray.FindIndex(x => x == colonyMainBase);
            return colonyMainBaseIndicesArray[index];
        }
        else
        {
            Debug.LogError("Current colony main base isn't contains in array");
            return Vector2Int.zero;
        }
    }
    
    private Vector2Int GetIndicesByResourceDeposit(ResourceDeposit resourceDeposit)
    {
        if (resourceDepositArray.Contains(resourceDeposit))
        {
            int index = resourceDepositArray.FindIndex(x => x == resourceDeposit);
            return resourceDepositIndicesArray[index];
        }
        else
        {
            Debug.LogError("Current resource deposit isn't contains in array");
            return Vector2Int.zero;
        }
    }

    private HexCellInfo GetHexCellInfo(Vector2Int indices)
    {
        HexCellInfo hexCellInfo = new HexCellInfo();

        if (IsValidHexIndices(indices))
        {
            HexCell hexCell = worldAreaInfo.area[indices.x, indices.y];
            hexCellInfo.hexType = hexCell.hexType;
            hexCellInfo.position = GetHexPosition(indices);

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
            else if (hexCellInfo.hexType == HexType.ColonyMainBase)
            {
                int indexInColonyMainBaseArray = worldAreaInfo.area[indices.x, indices.y].indexInColonyMainBaseArray;

                if (indexInColonyMainBaseArray != -1)
                    hexCellInfo.colonyMainBase = colonyMainBaseArray[indexInColonyMainBaseArray];
                else
                    Debug.LogError("Colony main base index is -1");
            }
        }

        return hexCellInfo;
    }

    private HexCellsAround GetHexCellsAround(Vector2Int indices)
    {
        HexCellsAround hexCellsAround = new HexCellsAround
        {
            centerCell = GetHexCellInfo(indices)
        };

        Vector2Int[] hexNeighbourIndices = GetHexNeighboursIndices(indices);
        for (int i = 0; i < cellNeighbourAmount; ++i)
            hexCellsAround.cellsAroud[i] = GetHexCellInfo(hexNeighbourIndices[i]);
        return hexCellsAround;
    }

    private static Vector2Int[] GetHexIndicesRing(Vector2Int indices, int radius)
    {
        if (radius <= 0)
            return new Vector2Int[] { indices };

        Vector3Int cube = OffsetToCube(indices);
        Vector3Int currentCube = cube;
        currentCube.x -= radius;

        Vector2Int[] result = new Vector2Int[cellNeighbourAmount * radius];

        for (int i = 0; i < cellNeighbourAmount; ++i)
            for (int j = 0; j < radius; ++j)
            {
                Vector2Int offset = CubeToOffset(currentCube);
                result[i * radius + j] = offset;
                currentCube = OffsetToCube(GetHexNeighbourIndices(offset, i));
            }

        return result;
    }

    private static Vector2Int[] GetHexIndicesRings(Vector2Int indices, int minRadius, int maxRadius)
    {
        if (minRadius < 0 || maxRadius < minRadius)
            Debug.LogWarning("Wrong min and max raduis");

        Vector2Int[] rings = new Vector2Int[0];

        for (int radius = minRadius; radius <= maxRadius; ++radius)
        {
            Vector2Int[] newRing = GetHexIndicesRing(indices, radius);

            Vector2Int[] expandedRings = new Vector2Int[rings.Length + newRing.Length];
            rings.CopyTo(expandedRings, 0);
            newRing.CopyTo(expandedRings, rings.Length);

            rings = expandedRings;
        }

        return rings;
    }

    private static int Distance(Vector2Int begin, Vector2Int end)
    {
        Vector3Int beginCube = OffsetToCube(begin);
        Vector3Int endCube = OffsetToCube(end);

        return Mathf.Max(Mathf.Abs(beginCube.x - endCube.x),
            Mathf.Abs(beginCube.y - endCube.y),
            Mathf.Abs(beginCube.z - endCube.z));
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


    private void Start()
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
