using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    public BuildingItemList buildingItemList;
    public Builder builder;
    public MenuManager menuManager;

    public RectTransform buildingLayoutGroup;
    public RectTransform buildingTypeLayoutGroup;

    public GameObject buildingItemPrefab;
    public GameObject buildingTypeItemPrefab;


    private void UpdateBuildingItemList(BuildingItem[] buildingItems)
    {
        buildingLayoutGroup.DetachChildren();

        foreach (BuildingItem buildingItem in buildingItems)
        {
            GameObject buildingButton = Instantiate(buildingItemPrefab, buildingLayoutGroup);

            BuildingMenuItem buildingMenuItem = buildingButton.GetComponent<BuildingMenuItem>();
            buildingMenuItem.image.sprite = buildingItem.buildingSprite;
            buildingMenuItem.nameText.text = buildingItem.name;
            buildingMenuItem.SetCallBack(() =>
            {
                builder.SetCurrentBuilding(buildingItem.buildingPrefab);
                menuManager.CloseMenu(Menu.MenuName.BuildingMenu);
            });
        }
    }

    public void Start()
    {
        Debug.Assert(buildingItemList, "Building Item List doesn't set");
        Debug.Assert(builder, "Builder doesn't set");
        Debug.Assert(menuManager, "Menu Manager doesn't set");
        Debug.Assert(buildingLayoutGroup, "Building Layout Group doesn't set");
        Debug.Assert(buildingTypeLayoutGroup, "Building Type Layout Group doesn't set");
        Debug.Assert(buildingItemPrefab, "Building Item Prefab doesn't set");
        Debug.Assert(buildingTypeItemPrefab, "Building Type Item Prefab doesn't set");

        foreach (BuildingItemList.BuildingTypeItemToBuildingItem buildingTypeItemToBuildingItem in
            buildingItemList.buildingTypeItemToBuildingItems)
        {
            GameObject buildingTypeItem = Instantiate(buildingTypeItemPrefab, buildingTypeLayoutGroup);

            BuildingMenuItem buildingMenuItem = buildingTypeItem.GetComponent<BuildingMenuItem>();
            buildingMenuItem.image.sprite = buildingTypeItemToBuildingItem.buildingTypeItem.buildingSprite;

            buildingMenuItem.SetCallBack(() => UpdateBuildingItemList(buildingTypeItemToBuildingItem.buildingItems));
        }

        UpdateBuildingItemList(buildingItemList.buildingTypeItemToBuildingItems[0].buildingItems);
    }
}
