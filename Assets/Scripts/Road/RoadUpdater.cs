using System;
using UnityEngine;

public class RoadUpdater : MonoBehaviour
{
    [Flags]
    public enum RoadDirection : byte
    {
        None        = 0b_0000_0000,
        TopRight    = 0b_0000_0001,
        Right       = 0b_0000_0010,
        BottomRight = 0b_0000_0100,
        BottomLeft  = 0b_0000_1000,
        Left        = 0b_0001_0000,
        TopLeft     = 0b_0010_0000,

        Default     = Left | Right
    }

    private enum RoadDirectionIndex : byte
    {
        TopRight,
        Right,
        BottomRight,
        BottomLeft,
        Left,
        TopLeft,
        CountOf
    }


    public GameObject singleRoadPrefab;

    [HideInInspector]
    public RoadDirection currnetRoadDirection = RoadDirection.None;


    private readonly float[] angles = new float[] { -60f, 0f, 60f, 120f, 180f, 240f };

    private readonly GameObject[] singleRoads = new GameObject[(int) RoadDirectionIndex.CountOf];


    public void UpdateRoad(RoadDirection newRoadDirection)
    {
        for (int i = 0; i < (int) RoadDirectionIndex.CountOf; ++i)
        {
            RoadDirection roadDirection = IndexToRoadDirection((RoadDirectionIndex) i);
            bool isInCurrent = (currnetRoadDirection & roadDirection) == roadDirection;
            bool isInNew = (newRoadDirection & roadDirection) == roadDirection;
            
            if (isInCurrent != isInNew)
            {
                if (isInCurrent)
                {
                    Destroy(singleRoads[i]);
                }
                else
                {
                    singleRoads[i] = Instantiate(singleRoadPrefab, transform.position, Quaternion.Euler(0f, angles[i], 0f), transform);
                }
            }
        }

        currnetRoadDirection = newRoadDirection;
    }

    public void UpdateRoad(WorldGenerator.HexCellsAround hexCellsAround)
    {
        RoadDirection roadDirection = RoadDirection.None;

        for (int i = 0; i < (int) RoadDirectionIndex.CountOf; ++i)
        {
            if (WorldGenerator.IsContainDuildingOrRoad(hexCellsAround.cellsAroud[i].hexType))
            {
                roadDirection |= IndexToRoadDirection((RoadDirectionIndex) i);
            }
        }

        if (roadDirection == RoadDirection.None)
        {
            roadDirection = RoadDirection.Default;
        }

        UpdateRoad(roadDirection);
    }

    private RoadDirectionIndex RoadDirectionToIndex(RoadDirection roadDirection)
    {
        switch (roadDirection)
        {
            case RoadDirection.TopRight:
                return RoadDirectionIndex.TopRight;
            case RoadDirection.Right:
                return RoadDirectionIndex.Right;
            case RoadDirection.BottomRight:
                return RoadDirectionIndex.BottomRight;
            case RoadDirection.BottomLeft:
                return RoadDirectionIndex.BottomLeft;
            case RoadDirection.Left:
                return RoadDirectionIndex.Left;
            case RoadDirection.TopLeft:
                return RoadDirectionIndex.TopLeft;
            default:
                return RoadDirectionIndex.CountOf;
        }
    }

    private RoadDirection IndexToRoadDirection(RoadDirectionIndex roadDirectionIndex)
    {
        switch (roadDirectionIndex)
        {
            case RoadDirectionIndex.TopRight:
                return RoadDirection.TopRight;
            case RoadDirectionIndex.Right:
                return RoadDirection.Right;
            case RoadDirectionIndex.BottomRight:
                return RoadDirection.BottomRight;
            case RoadDirectionIndex.BottomLeft:
                return RoadDirection.BottomLeft;
            case RoadDirectionIndex.Left:
                return RoadDirection.Left;
            case RoadDirectionIndex.TopLeft:
                return RoadDirection.TopLeft;
            default:
                return RoadDirection.None;
        }
    }

    private void Awake()
    {
        Utils.CheckFieldNotNull(singleRoadPrefab, "Single Road Prefab");

        UpdateRoad(RoadDirection.Default);
    }
}
