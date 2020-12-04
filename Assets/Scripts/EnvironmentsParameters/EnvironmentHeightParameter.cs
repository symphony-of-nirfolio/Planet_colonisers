using UnityEngine;

public class EnvironmentHeightParameter : MonoBehaviour
{
    public Transform evironmentTransform;

    public float minHeight = 150f;
    public float maxHeight = 400f;

    /// <param name="heightCoefficient">Have to be in range [0f : 1f]</param>
    public void InitHeight(float heightCoefficient)
    {
        Vector3 localScale = evironmentTransform.localScale;
        localScale.z = minHeight + heightCoefficient * (maxHeight - minHeight);
        evironmentTransform.localScale = localScale;
    }


    private void Start()
    {
        Debug.Assert(evironmentTransform, "Mountain Transform doesn't set");
    }
}
