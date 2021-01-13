using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStorage : MonoBehaviour
{
    private List<GameResourcesStorage> userStorages;
    private PersonalResourcesMenu personalResourcesUI;
    public float refreshTime = 2;
    private float timeSinceRefresh = 0;
    private int currentMainPlayer = 0;
    void Start()
    {
        userStorages = new List<GameResourcesStorage>();
        userStorages.Add(new GameResourcesStorage(20));
        personalResourcesUI = FindObjectOfType<PersonalResourcesMenu>();
    }

    void Update()
    {
        timeSinceRefresh += Time.deltaTime;
        if (timeSinceRefresh > refreshTime)
        {
            timeSinceRefresh -= refreshTime;
            personalResourcesUI.UpdateResources(userStorages[currentMainPlayer]);
        }

        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown("0"))
            {
                currentMainPlayer = 0;
            }
            else if (Input.GetKeyDown("1"))
            {
                currentMainPlayer = 1;
            }
            else if (Input.GetKeyDown("2"))
            {
                currentMainPlayer = 2;
            }
            else if (Input.GetKeyDown("3"))
            {
                currentMainPlayer = 3;
            }
            else if (Input.GetKeyDown("4"))
            {
                currentMainPlayer = 4;
            }
        }
        
    }

    public void AddResource(GameResourceType resource, float count, int userId = -1)
    {
        if (userId == -1)
        {
            userId = currentMainPlayer;
        }
        userStorages[userId].AddResource(resource, count);
    }

    public void RemoveResource(GameResourceType resource, float count, int userId = -1)
    {
        if (userId == -1)
        {
            userId = currentMainPlayer;
        }
        userStorages[userId].RemoveResource(resource, count);
    }

    // Creates new player inventory and returs it's id
    public int AddNewPlayer()
    {
        userStorages.Add(new GameResourcesStorage());
        return userStorages.Count - 1;
    }

    public int CurrentMainPlayerId()
    {
        return currentMainPlayer;
    }

    public void SetCurrentMainPlayer(int playerId)
    {
        currentMainPlayer = playerId;
    }
}
