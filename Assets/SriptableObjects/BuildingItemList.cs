using System;
using UnityEngine;

[CreateAssetMenu(menuName = "BuildingItems/BuildingItemList")]
public class BuildingItemList : ScriptableObject
{
    [Serializable]
    public class BuildingTypeItemToBuildingItem
    {
        public BuildingTypeItem buildingTypeItem;
        public BuildingItem[] buildingItems;
    }

    public BuildingItem[] buildingItems;

    public BuildingTypeItemToBuildingItem[] buildingTypeItemToBuildingItems;
}
