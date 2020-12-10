using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameParameters gameParameters;
    public Camera mainCamera;
    public WorldMap worldMap;
    public CameraMovement cameraMovement;

    public LimitedMinedResourceInfoList limitedMinedResourceInfoList;
    public Camera resourceCamera;
    public Renderer resourceRenderer;
    public Camera mountainsCamera;
    public Renderer mountainsRenderer;
    public Renderer planeRenderer;
    public Transform mountainsTransform;
    public Transform cratersTransform;
    public Transform resourcesTransform;
    public Transform mainBaseLocationsTransform;
    public GameObject mountainPrefab;
    public GameObject craterPrefab;
    public GameObject resourceDepositPrefab;
    public GameObject mainBaseLocationPrefab;

    public int seed = 1;

    public int distanceBetweenColony = 2;
    public int colonyRaduis = 6;
    public int attemptsAmountToAddAnyResourcesPerType = 5;

    public float hexMinRadius = 1f;

    public float craterRiddling = 0.0001f;
    public float defaultRiddling = 0.001f;
    public float mainBaseRiddling = 0.0003f;

    public float planeYOffset = -0.2137f;


    private const float maxPossibleResourceValuePerCell = 4f * 256f;
    private const int mountainOffset = 1;
    private const int craterOffset = 11;
    private const int waterResourceOffset = 21;
    private const int coloniesOffset = 31;


    private RenderTexture renderTexture;
    private Material resourceMaterial;
    private Material mountainMaterial;
    private Material planeMaterial;

    private int chankSize;
    private int width;
    private int height;
    private int cellColumns;
    private int cellRows;
    private int colonyAmount;


    private Color32[] TakeResourceSnapshot(int xOffset, int yOffset, Material currentMaterial, Camera currentCamera)
    {
        currentCamera.gameObject.SetActive(true);

        currentMaterial.SetVector("Vector2_Offset", new Vector4(xOffset, yOffset, 0f, 0f));
        Texture2D snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        currentCamera.Render();
        RenderTexture.active = renderTexture;
        snapshot.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);

        currentCamera.gameObject.SetActive(false);

        return snapshot.GetPixels32();
    }

    private void InitLand()
    {
        for (int x = 0; x < cellColumns; ++x)
            for (int y = 0; y < cellRows; ++y)
            {
                Vector2Int indices = new Vector2Int(x, y);
                Vector3 centerPosition = worldMap.GetHexPosition(indices);

                WorldMap.HexCell hexCell = WorldMap.HexCell.Empty;
                if (worldMap.IsHexInsideMap(centerPosition))
                    hexCell.hexType = WorldMap.HexType.Land;
                worldMap.worldAreaInfo.area[x, y] = hexCell;
            }
    }

    private void InitChankWithHexType(WorldMap.HexType hexType, int chankX, int chankY, Material currentMaterial, Camera currentCamera, bool isIncludeNoneType, ref bool isAdded)
    {
        int currentXOffset = chankX * chankSize;
        int currentYOffset = chankY * chankSize;

        Color32[] colors = TakeResourceSnapshot(chankX, chankY, currentMaterial, currentCamera);

        for (int pixelX = 0; pixelX < renderTexture.width; ++pixelX)
            for (int pixelY = 0; pixelY < renderTexture.height; ++pixelY)
            {
                Color32 color = colors[pixelX + pixelY * renderTexture.width];

                Vector3 position = new Vector3(pixelX + currentXOffset, 0f, pixelY + currentYOffset);
                Vector2Int indices = worldMap.GetHexIndices(position);

                if (worldMap.IsValidHexIndices(indices))
                {
                    WorldMap.HexCell hexCell = worldMap.worldAreaInfo.area[indices.x, indices.y];

                    if (color.r != 0)
                    {
                        if (hexCell.hexType == hexType)
                        {
                            hexCell.resourceAmount += color.r;

                            worldMap.worldAreaInfo.area[indices.x, indices.y] = hexCell;
                        }
                        else if ((isIncludeNoneType && hexCell.hexType == WorldMap.HexType.None) || hexCell.hexType == WorldMap.HexType.Land)
                        {
                            isAdded |= true;

                            hexCell.hexType = hexType;
                            hexCell.resourceAmount += color.r;

                            worldMap.worldAreaInfo.area[indices.x, indices.y] = hexCell;
                        }
                    }
                }
            }
    }

    private void InitWorldAreaWithHexType(WorldMap.HexType hexType, float riddling, int areaSeed, Material currentMaterial, Camera currentCamera, bool isIncludeNoneType, ref bool isAdded)
    {
        currentMaterial.SetFloat("Vector1_Riddling", riddling);
        currentMaterial.SetFloat("Vector1_Seed", areaSeed);

        for (int chankX = 0; chankX < width / chankSize; ++chankX)
            for (int chankY = 0; chankY < height / chankSize; ++chankY)
                InitChankWithHexType(hexType, chankX, chankY, currentMaterial, currentCamera, isIncludeNoneType, ref isAdded);
    }

    private void InitMountains(int mountainsSeed)
    {
        bool isAdded = false;
        InitWorldAreaWithHexType(WorldMap.HexType.Mountain, defaultRiddling, mountainsSeed, mountainMaterial, mountainsCamera, true, ref isAdded);
    }

    private void InitCraters(int cratersSeed)
    {
        bool isAdded = false;
        InitWorldAreaWithHexType(WorldMap.HexType.Crater, craterRiddling, cratersSeed, resourceMaterial, resourceCamera, true, ref isAdded);
    }

    private void InitResources(int resourcesSeed)
    {
        int resourceSeed = resourcesSeed;
        foreach (LimitedMinedResourceInfo resourceInfo in limitedMinedResourceInfoList.resourceInfos)
        {
            bool isAdded = false;
            float riddling = resourceInfo.riddling;
            WorldMap.HexType hexType = WorldMap.GameResourceTypeToHexType(resourceInfo.gameResourceType);
            for (int i = 0; i < attemptsAmountToAddAnyResourcesPerType && !isAdded; ++i)
            {
                InitWorldAreaWithHexType(hexType,
                    riddling, resourceSeed, resourceMaterial, resourceCamera, false, ref isAdded);
                riddling *= 2f;
                ++resourceSeed;
            }
            if (!isAdded)
                Debug.LogError(string.Format("Cannot add {0} resource. Increase resource riddling may fix the problem", hexType.ToString()));
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
        bool isAdded = false;
        InitWorldAreaWithHexType(WorldMap.HexType.ColonyMainBase, mainBaseRiddling, offsettedColoniesSeed, resourceMaterial, resourceCamera, false, ref isAdded);

        List<Vector2Int> allColonyIndices = new List<Vector2Int>();

        for (int x = 0; x < cellColumns; ++x)
            for (int y = 0; y < cellRows; ++y)
            {
                WorldMap.HexCell hexCell = worldMap.worldAreaInfo.area[x, y];

                if (hexCell.hexType == WorldMap.HexType.ColonyMainBase)
                {
                    allColonyIndices.Add(new Vector2Int(x, y));

                    hexCell.hexType = WorldMap.HexType.Land;
                    worldMap.worldAreaInfo.area[x, y] = hexCell;
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
            WorldMap.HexCell hexCell = WorldMap.HexCell.Empty;
            hexCell.hexType = WorldMap.HexType.ColonyMainBase;

            worldMap.worldAreaInfo.area[currentColony.x, currentColony.y] = hexCell;
        }
    }

    private void SetResourcePrefabs()
    {
        LimitedMinedResourceInfo[] hexTypeToLimitedMinedResourceInfo = new LimitedMinedResourceInfo[(int) WorldMap.HexType.AllResources];

        foreach (LimitedMinedResourceInfo resourceInfo in limitedMinedResourceInfoList.resourceInfos)
        {
            WorldMap.HexType hexType = WorldMap.GameResourceTypeToHexType(resourceInfo.gameResourceType);
            hexTypeToLimitedMinedResourceInfo[(int) hexType] = resourceInfo;
        }

        for (int x = 0; x < cellColumns; ++x)
            for (int y = 0; y < cellRows; ++y)
            {
                WorldMap.HexCell hexCell = worldMap.worldAreaInfo.area[x, y];
                if (WorldMap.IsResourceOnlyType(hexCell.hexType))
                {
                    GameResourceType gameResourceType = WorldMap.HexTypeToGameResourceType(hexCell.hexType);

                    GameObject resourceDeposit = Instantiate(resourceDepositPrefab,
                        worldMap.GetHexPosition(new Vector2Int(x, y)),
                        Quaternion.identity,
                        resourcesTransform);

                    ResourceSprite resourceSprite = resourceDeposit.GetComponent<ResourceSprite>();
                    resourceSprite.InitWithGameResourceType(gameResourceType);

                    ResourceDeposit resourceDepositScript = resourceDeposit.GetComponent<ResourceDeposit>();

                    LimitedMinedResourceInfo limitedMinedResourceInfo = hexTypeToLimitedMinedResourceInfo[(int) hexCell.hexType];
                    float resourceAmount = limitedMinedResourceInfo.minAmount +
                        Mathf.Pow(hexCell.resourceAmount / maxPossibleResourceValuePerCell, limitedMinedResourceInfo.power) *
                        (limitedMinedResourceInfo.maxAmount - limitedMinedResourceInfo.minAmount);
                    resourceDepositScript.SetResourceType(gameResourceType, resourceAmount);

                    hexCell.indexInResourceArray = (short) worldMap.resourceDepositArray.Count;
                    worldMap.worldAreaInfo.area[x, y] = hexCell;
                    worldMap.resourceDepositArray.Add(resourceDepositScript);
                    worldMap.resourceDepositIndicesArray.Add(new Vector2Int(x, y));
                }
                else if (hexCell.hexType == WorldMap.HexType.Mountain)
                {
                    GameObject mountain = Instantiate(mountainPrefab,
                        worldMap.GetHexPosition(new Vector2Int(x, y)),
                        Quaternion.identity,
                        mountainsTransform);

                    EnvironmentHeightParameter environmentHeightParameter =
                        mountain.GetComponent<EnvironmentHeightParameter>();
                    environmentHeightParameter.InitHeight(hexCell.resourceAmount / maxPossibleResourceValuePerCell);
                }
                else if (hexCell.hexType == WorldMap.HexType.Crater)
                {
                    GameObject crater = Instantiate(craterPrefab,
                        worldMap.GetHexPosition(new Vector2Int(x, y)),
                        Quaternion.identity,
                        cratersTransform);

                    EnvironmentHeightParameter environmentHeightParameter =
                        crater.GetComponent<EnvironmentHeightParameter>();
                    environmentHeightParameter.InitHeight(hexCell.resourceAmount / maxPossibleResourceValuePerCell);
                }
                else if (hexCell.hexType == WorldMap.HexType.ColonyMainBase)
                {
                    GameObject mainBaseLocation = Instantiate(mainBaseLocationPrefab,
                        worldMap.GetHexPosition(new Vector2Int(x, y)),
                        Quaternion.identity,
                        mainBaseLocationsTransform);

                    ColonyTeritory colonyTeritory = mainBaseLocation.GetComponent<ColonyTeritory>();
                    Vector3[] positions = worldMap.GetHexRings(worldMap.GetHexPosition(new Vector2Int(x, y)), 1, colonyRaduis);
                    colonyTeritory.InitAvailableHexes(new List<Vector3>(positions), worldMap.colonyMainBaseArray.Count == 0);

                    hexCell.indexInColonyMainBaseArray = (short) worldMap.colonyMainBaseArray.Count;
                    worldMap.worldAreaInfo.area[x, y] = hexCell;
                    worldMap.colonyMainBaseArray.Add(mainBaseLocation);
                    worldMap.colonyMainBaseIndicesArray.Add(new Vector2Int(x, y));
                }
            }
    }

    private void InitWorld()
    {
        InitLand();

        InitMountains(seed + mountainOffset);

        InitCraters(seed + craterOffset);

        InitResources(seed + waterResourceOffset);

        InitColonyMainBases(seed + coloniesOffset);

        SetResourcePrefabs();

        cameraMovement.playerMainBasePosition = worldMap.GetPositoinByColonyMainBase(worldMap.colonyMainBaseArray[0]);
        cameraMovement.SetCameraViewToMainBase();
    }


    private void Start()
    {
        Utils.CheckFieldNotNullAndTryToSet(ref gameParameters, "Game Parameters");
        Utils.CheckMainCameraNotNullAndTryToSet(ref mainCamera);
        Utils.CheckFieldNotNullAndTryToSet(ref worldMap, "World map");
        Utils.CheckFieldNotNullAndTryToSet(ref cameraMovement, "Camera Movement");
        Utils.CheckFieldNotNull(limitedMinedResourceInfoList, "Limited Mined Resource Info List");
        Utils.CheckFieldNotNull(resourceCamera, "Resource Camera");
        Utils.CheckFieldNotNull(resourceRenderer, "Resource Renderer");
        Utils.CheckFieldNotNull(mountainsCamera, "Mountains Camera");
        Utils.CheckFieldNotNull(mountainsRenderer, "Mountains Renderer");
        Utils.CheckFieldNotNull(planeRenderer, "Plane Renderer");
        Utils.CheckFieldNotNull(mountainsTransform, "Mountains Transform");
        Utils.CheckFieldNotNull(cratersTransform, "Craters Transform");
        Utils.CheckFieldNotNull(resourcesTransform, "Resources Transform");
        Utils.CheckFieldNotNull(mainBaseLocationsTransform, "Main Base Locations Transform");
        Utils.CheckFieldNotNull(craterPrefab, "Crater Prefab");
        Utils.CheckFieldNotNull(resourceDepositPrefab, "Resource Deposit Prefab");
        Utils.CheckFieldNotNull(mainBaseLocationPrefab, "Main Base Location Prefab");

        renderTexture = resourceCamera.targetTexture;
        resourceMaterial = resourceRenderer.sharedMaterial;
        mountainMaterial = mountainsRenderer.sharedMaterial;
        planeMaterial = planeRenderer.sharedMaterial;

        chankSize = renderTexture.width / 2;

        width = worldMap.width;
        height = worldMap.height;
        cellColumns = worldMap.cellColumns;
        cellRows = worldMap.cellRows;

        int repeatOffset = ((int) gameParameters.mapSize.height / chankSize / 2 + 1) * 2 + 1;
        planeMaterial.SetVector("Vector2_Offset", new Vector4(0f, planeYOffset * repeatOffset, 0f, 0f));

        colonyAmount = gameParameters.colonyAmount;

        InitWorld();
    }

    private void OnDisable()
    {
        resourceMaterial.SetFloat("Vector1_Riddling", defaultRiddling);
        resourceMaterial.SetFloat("Vector1_Seed", 0);
        resourceMaterial.SetVector("Vector2_Offset", Vector4.zero);

        mountainMaterial.SetFloat("Vector1_Riddling", defaultRiddling);
        mountainMaterial.SetFloat("Vector1_Seed", 0);
        mountainMaterial.SetVector("Vector2_Offset", Vector4.zero);

        planeMaterial.SetVector("Vector2_Offset", Vector4.zero);
    }
}
