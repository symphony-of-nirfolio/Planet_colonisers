using UnityEngine;

[CreateAssetMenu(menuName = "LimitedMinedResourceInfo/LimitedMinedResourceInfo")]
public class LimitedMinedResourceInfo : ScriptableObject
{
    public GameResourceType gameResourceType;
    public Sprite sprite;
    public float riddling = 0.001f;
}
