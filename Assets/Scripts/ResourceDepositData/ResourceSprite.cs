using UnityEngine;

public class ResourceSprite : MonoBehaviour
{
    public LimitedMinedResourceInfoList limitedMinedResourceInfoList;
    public SpriteRenderer spriteRenderer;


    public void InitWithGameResourceType(GameResourceType gameResourceType)
    {
        foreach (LimitedMinedResourceInfo resourceInfo in
            limitedMinedResourceInfoList.resourceInfos)
        {
            if (resourceInfo.gameResourceType == gameResourceType)
            {
                spriteRenderer.sprite = resourceInfo.sprite;
                return;
            }
        }
        Debug.LogError("Game Resource Type doesn't found in Limited Mined Resource Info List");
    }


    private void Start()
    {
        Debug.Assert(limitedMinedResourceInfoList, "Limited Mined Resource Info List doesn't set");
        Debug.Assert(spriteRenderer, "Sprite Renderer doesn't set");
    }
}
