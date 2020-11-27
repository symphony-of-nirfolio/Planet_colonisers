using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class GameResourcesStorage
{
    const int resourceCount = 12; 

    private GameResource[] storage = new GameResource[resourceCount];

    public GameResourcesStorage(float defaultCount = 0)
    {
        for (int i = 0; i < resourceCount; ++i)
        {
            storage[i] = new GameResource((GameResourceType)i, defaultCount);
        }    
    }

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
        Debug.Assert((int)resource < resourceCount);
        storage[(int)resource].amount -= count;
    }
}
