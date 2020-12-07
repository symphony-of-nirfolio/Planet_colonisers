using UnityEngine;

[CreateAssetMenu(menuName = "GameResourceSprite/GameResourceSprite")]
public class GameResourceSprite : ScriptableObject
{
    public GameResourceType gameResourceType;
    public Sprite sprite;
}
