using UnityEngine;

[CreateAssetMenu(menuName = "LimitedMinedResourceInfo/LimitedMinedResourceInfo")]
public class LimitedMinedResourceInfo : ScriptableObject
{
    public GameResourceType gameResourceType;
    public Sprite sprite;
    public float riddling = 0.001f;
    public float minAmount = 10f;
    public float maxAmount = 100f;
    public float power = 0.25f;
}
