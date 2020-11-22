using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum ResourcePack
{
    noResources,
    firstBaseResources
}

[CreateAssetMenu(menuName = "GameResoucesStorages/GameResoucesStorage")]
public class GameResourcesStorage : ScriptableObject
{
    //public ResourcePack currentPack;
    const int resourceCount = 12; 

    private GameResource[] storage = new GameResource[resourceCount];

    public void AddResource(GameResourceType resource, float count)
    {
        Debug.Assert(count >= 0);
        Debug.Assert((int)resource < resourceCount);
        storage[(int)resource].amount += count;
    }

    public void RemoveResource(GameResourceType resource, float count)
    {
        Debug.Assert(count >= 0);
        Debug.Assert(storage[(int)resource].amount - count >= 0);
        Debug.Assert((int)resource < 12);
        storage[(int)resource].amount -= count;
    }
}
