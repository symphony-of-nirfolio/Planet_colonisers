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

[CreateAssetMenu(menuName = "GameResouces/GameResource")]
public class GameResouces : ScriptableObject
{
    public ResourcePack currentPack;

    public float food = 0;
    public float oxygen = 0;
    public float water = 0;
    public float elecricity = 0;

    public float metals = 0;
    public float rareMetals = 0;
    public float dust = 0;
    public float radioactive = 0;

    public float concrete = 0;
    public float electronics = 0;
    public float machineParts = 0;
    public float steel = 0;
}
