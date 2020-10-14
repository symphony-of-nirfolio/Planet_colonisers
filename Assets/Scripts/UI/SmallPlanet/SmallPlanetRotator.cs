using UnityEngine;

public class SmallPlanetRotator : MonoBehaviour
{
    public float rotationSpeed = 5f;


    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
