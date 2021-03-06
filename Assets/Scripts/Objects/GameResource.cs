﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum GameResourceType
{
    Food,
    Oxygen,
    Water,
    Elecricity,

    Metals,
    RareMetals,
    Dust,
    Radioactive,

    Concrete,
    Electronics,
    MachineParts,
    Steel,
}

public class GameResource
{
    public readonly GameResourceType resourceType;
    public float amount;
    public GameResource(GameResourceType resource)
    {
        resourceType = resource;
        amount = 0;
    }

    public GameResource(GameResourceType resource, float startAmont)
    {
        resourceType = resource;
        amount = startAmont;
    }
}
