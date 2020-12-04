using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStorage : MonoBehaviour
{
    private GameResourcesStorage storage;
    private GameObject personalResourcesUI;
    public float refreshTime = 2;
    private float timeSinceRefresh = 0;
    void Start()
    {
        storage = new GameResourcesStorage();
        personalResourcesUI = GameObject.Find("GameMenuUI/Canvas/NoClosableMenus/PersonalResourcesMenu/Panel/PersonalResourcesMenu");
    }

    void Update()
    {
        timeSinceRefresh += Time.deltaTime;
        if (timeSinceRefresh > refreshTime)
        {
            timeSinceRefresh -= refreshTime;
            personalResourcesUI.GetComponent<PersonalResourcesMenu>().UpdateResources(storage);
        }
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
