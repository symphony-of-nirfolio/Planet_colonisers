using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryControl : MonoBehaviour
{
    GameResoucesStorage storage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddResource(GameResources resource, float count)
    {
        storage.AddResource(resource, count);
    }

    public void RemoveResource(GameResources resource, float count)
    {
        storage.RemoveResource(resource, count);
    }
}
