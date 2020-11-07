using UnityEngine;


[CreateAssetMenu(menuName = "BuildingItems/BuildingItem")]
public class BuildingItem : ScriptableObject
{
    public GameObject buildingPrefab;
    public Sprite buildingSprite;
}
