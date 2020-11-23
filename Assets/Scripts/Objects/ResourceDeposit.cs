using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ResourceDeposit : MonoBehaviour
{
    private GameResource resource;
    private bool isResourseSet = false;
    public int maxAmount;

    public void SetResourceType(GameResourceType resourceType)
    {
        Debug.Assert(!isResourseSet);
        this.resource = new GameResource(resourceType);
        isResourseSet = true;
    }

    public void GetResourceFromDeposit(float amount)
    {
        Debug.Assert(resource.amount >= 0f);
        resource.amount -= amount;
    }

    public GameResourceType GetResourceType()
    {
        return resource.resourceType;
    }

    public float AvailableResource()
    {
        return resource.amount;
    }
    
}
