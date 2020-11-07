using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    public BuildingItemList buildingItemList;

    public RectTransform buildingRectTransform;

    public GameObject buildingButtonPrefab;

    public void Start()
    {
        foreach (BuildingItem buildingItem in buildingItemList.buildingItems)
        {
            GameObject buildingButton = Instantiate(buildingButtonPrefab, buildingRectTransform);
            
            BuildingMenuItem buildingMenuItem = buildingButton.GetComponent<BuildingMenuItem>();
            buildingMenuItem.image.sprite = buildingItem.buildingSprite;
            buildingMenuItem.nameText.text = buildingItem.name;
        }
    }
}
