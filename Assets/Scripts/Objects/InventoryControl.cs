using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryControl : MonoBehaviour
{
    GameResourcesStorage storage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddResource(GameResourceType resource, float count)
    {
        storage.AddResource(resource, count);
    }

    public void RemoveResource(GameResourceType resource, float count)
    {
        storage.RemoveResource(resource, count);
    }
}
