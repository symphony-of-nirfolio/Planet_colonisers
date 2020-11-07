using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    public BuildingItemList buildingItemList;
    public Builder builder;
    public MenuManager menuManager;

    public RectTransform buildingRectTransform;

    public GameObject buildingButtonPrefab;

    public void Start()
    {
        Debug.Assert(buildingItemList, "Building Item List doesn't set");
        Debug.Assert(builder, "Builder doesn't set");
        Debug.Assert(menuManager, "Menu Manager doesn't set");
        Debug.Assert(buildingRectTransform, "Building Rect Transform doesn't set");
        Debug.Assert(buildingButtonPrefab, "Building Button Prefab doesn't set");

        foreach (BuildingItem buildingItem in buildingItemList.buildingItems)
        {
            GameObject buildingButton = Instantiate(buildingButtonPrefab, buildingRectTransform);
            
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
}
