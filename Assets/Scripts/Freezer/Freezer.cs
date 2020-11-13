using UnityEngine;

public class Freezer : MonoBehaviour
{
    public bool IsInteractionFreeze
    {
        get => interactionFreezesCount > 0;
    }

    public bool IsFullFreeze
    {
        get => fullFreezesCount > 0;
    }

    private int interactionFreezesCount = 0;
    private int fullFreezesCount = 0;

    private float timeScaleBeforeFreeze = 1f;


    public void InteractionFreeze()
    {
        ++interactionFreezesCount;
    }

    public void InteractionUnfreeze()
    {
        --interactionFreezesCount;
    }

    public void FullFreeze()
    {
        InteractionFreeze();

        if (!IsFullFreeze)
        {
            timeScaleBeforeFreeze = Time.timeScale;
            Time.timeScale = 0f;
        }

        ++fullFreezesCount;
    }
    
    public void FullUnfreeze()
    {
        InteractionUnfreeze();

        --fullFreezesCount;

        if (!IsFullFreeze)
        {
            Time.timeScale = timeScaleBeforeFreeze;
        }
    }
}
