using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum GameResourceType
{
    food,
    oxygen,
    water,
    elecricity,

    metals,
    rareMetals,
    dust,
    radioactive,

    concrete,
    electronics,
    machineParts,
    steel,
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
}
