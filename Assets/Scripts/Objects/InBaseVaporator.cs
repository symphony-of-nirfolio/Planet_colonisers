using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class InBaseVaporator : InBaseObject
{
    public InBaseVaporator(Base parent, float waterPerSecond = 1) : base(parent) 
    {
        this.resourcePerSecond = waterPerSecond;
        resourceType = GameResourceType.water;
    }
    public override String Name()
    {
        return "Moisture Vaporator";
    }
}
