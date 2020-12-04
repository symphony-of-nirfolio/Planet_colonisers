using System.Collections;
using UnityEngine;

public class MarsSkyBoxRotator : MonoBehaviour
{
    public Material marsSkyBoxMaterial;

    public float rotationSpeed = 0.7f;
    public float maxRotatingPerFrame = 0.5f;


    private Coroutine rotator;

    private float initialAngle;
    private float currentAngle;


    private IEnumerator Rotator()
    {
        while (true)
        {
            currentAngle += Mathf.Min(rotationSpeed * Time.deltaTime, maxRotatingPerFrame);

            marsSkyBoxMaterial.SetFloat("_Rotation", currentAngle);

            yield return new WaitForEndOfFrame();
        }
    }


    private void Start()
    {
        if (!marsSkyBoxMaterial)
        {
            Debug.LogError("Mars Sky Box Material doesn't set");
            return;
        }

        initialAngle = marsSkyBoxMaterial.GetFloat("_Rotation");
        currentAngle = initialAngle;

        rotator = StartCoroutine(Rotator());
    }

    private void OnDisable()
    {
        StopCoroutine(rotator);

        marsSkyBoxMaterial.SetFloat("_Rotation", initialAngle);
    }
}
