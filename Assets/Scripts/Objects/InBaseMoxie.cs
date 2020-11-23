using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InBaseMoxie : InBaseObject
{
    public InBaseMoxie(Base parent, float oxygenPerSecond = 1) : base(parent)
    {
        this.resourcePerSecond = oxygenPerSecond;
        resourceType = GameResourceType.oxygen;
    }
    public override String Name()
    {
        return "Moxie";
    }

}