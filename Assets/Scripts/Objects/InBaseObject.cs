using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class InBaseObject
{
    protected Base parent;
    protected float time;
    protected float produced;
    protected GameResourceType resourceType;
    protected float resourcePerSecond;

    protected InBaseObject(Base parent)
    {
        this.parent = parent;
    }
    public abstract String Name();

    public void Work(float deltalime)
    {
        time += deltalime;
        if (time > 1)
        {
            produced += resourcePerSecond;
            time -= 1;
        }
        if (produced > 0)
        {
            parent.resouces.AddResource(resourceType, produced);
            produced = 0;
        }
    }

}
