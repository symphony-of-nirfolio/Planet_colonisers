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
public class GameResoucesStorage : ScriptableObject
{
    //public ResourcePack currentPack;

    private float[] storage = new float[12];

    public void AddResource(GameResources resource, float count)
    {
        Debug.Assert(count >= 0);
        Debug.Assert((int)resource < 12);
        storage[(int)resource] += count;
    }

    public void RemoveResource(GameResources resource, float count)
    {
        Debug.Assert(count >= 0);
        Debug.Assert(storage[(int)resource] - count >= 0);
        Debug.Assert((int)resource < 12);
        storage[(int)resource] -= count;
    }
}
