using UnityEngine;

public class AIForNPC : MonoBehaviour
{
    public WorldMap worldMap;
    public Freezer freezer;
    public BuildingItemList buildingItemList;

    public Transform buildingTransform;

    public int colonyRaduis = 6;
    public int seed = 0;

    public float coolDown = 0.1f;


    private WorldMap.HexCellInfo[] hexCellInfos;

    private float previousTime = 0f;
    private int currentSeed = 0;

    public int id;

    
    private void UpdateSeed()
    {
        ++currentSeed;
    }

    private void AddBuildingToAvailableResource()
    {
        foreach (WorldMap.HexCellInfo hexCellInfo in hexCellInfos)
        {
            if (WorldMap.IsResourceOnlyType(hexCellInfo.hexType))
            {
                GameObject building = Instantiate(
                    buildingItemList.buildingTypeItemToBuildingItems[1].buildingItems[0].buildingPrefab,
                    hexCellInfo.position, Quaternion.identity, buildingTransform);

                ResourceDeposit resourceDeposit = worldMap.GetResourceDeposit(hexCellInfo.position);
                Debug.Assert(resourceDeposit, "Resource Deposit is null");

                ResourceExtractor resourceExtractor = building.GetComponent<ResourceExtractor>();
                resourceExtractor.SetDeposit(resourceDeposit);
                resourceExtractor.SetOwnerId(id);

                worldMap.AddBuildingToHexCell(hexCellInfo.position, building);

                return;
            }
        }
    }

    private void AddBuildingToFreeaLand()
    {
        foreach (WorldMap.HexCellInfo hexCellInfo in hexCellInfos)
        {
            if (hexCellInfo.hexType == WorldMap.HexType.Land)
            {
                int index = Random.value < 0.5f ? 0 : 2;
                GameObject building = Instantiate(
                    buildingItemList.buildingTypeItemToBuildingItems[index].buildingItems[0].buildingPrefab,
                    hexCellInfo.position, Quaternion.identity, buildingTransform);

                worldMap.AddBuildingToHexCell(hexCellInfo.position, building);

                return;
            }
        }
    }

    private void Start()
    {
        Utils.CheckFieldNotNullAndTryToSet(ref worldMap, "World map");
        Utils.CheckFieldNotNullAndTryToSet(ref freezer, "Freezer");
        Utils.CheckFieldNotNull(buildingItemList, "Building Item List");
        Utils.CheckFieldNotNull(buildingTransform, "Building Transform");

        currentSeed = seed;

        hexCellInfos = worldMap.GetHexInfoRings(transform.position, 1, colonyRaduis);
        UpdateSeed();
        Utils.Shuffle(hexCellInfos, currentSeed);


        GameObject globalStorage = GameObject.Find("GlobalStorage");
        id = globalStorage.GetComponent<GlobalStorage>().AddNewPlayer();
    }

    private void Update()
    {
        if (freezer.IsFullFreeze)
            return;

        if (Time.time - previousTime >= coolDown)
        {
            previousTime = Time.time;

            if (Random.value < 0.5f)
                AddBuildingToAvailableResource();
            else
                AddBuildingToFreeaLand();

            hexCellInfos = worldMap.GetHexInfoRings(transform.position, 1, colonyRaduis);
            UpdateSeed();
            Utils.Shuffle(hexCellInfos, currentSeed);
        }
    }
}
