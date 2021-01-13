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
    private float maxAmount;

    public void SetResourceType(GameResourceType resourceType, float amount = 0)
    {
        Debug.Assert(!isResourseSet);
        this.resource = new GameResource(resourceType);
        isResourseSet = true;
        maxAmount = amount;
        resource.amount = amount;
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

    public float GetMaxAmount()
    {
        return maxAmount;
    }
    
}
