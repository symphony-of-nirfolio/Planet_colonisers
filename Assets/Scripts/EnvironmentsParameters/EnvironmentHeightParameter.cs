using UnityEngine;

public class EnvironmentHeightParameter : MonoBehaviour
{
    public Transform mountainTransform;

    public float minHeight = 150f;
    public float maxHeight = 400f;


    public void InitMountain(float heightCoefficient)
    {
        Vector3 localScale = mountainTransform.localScale;
        localScale.z = minHeight + heightCoefficient * (maxHeight - minHeight);
        mountainTransform.localScale = localScale;
    }


    private void Start()
    {
        Debug.Assert(mountainTransform, "Mountain Transform doesn't set");
    }
}
