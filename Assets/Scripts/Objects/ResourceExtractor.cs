﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ResourceExtractor : MonoBehaviour
{

    private GameResourcesStorage inventory;
    private GameResourceType resourceType;
    private ResourceDeposit deposit;
    private bool hasDeposit = false;

    public float resourceGatherTime = 3.0f;
    public float resourcesPerGather = 1.0f;
    private float timer = 0.0f;

    private void Update()
    {
        if (hasDeposit)
        {
            timer += Time.deltaTime;

            if (timer > resourceGatherTime)
            {
                gatherResources();
                timer -= resourceGatherTime;
            }
        }
    }

    public void SetDeposit(ResourceDeposit deposit)
    {
        hasDeposit = true;
        this.deposit = deposit;
        resourceType = deposit.GetResourceType();
    }

    public void gatherResources()
    {
        if(deposit.AvailableResource() > 0)
        {
            deposit.GetResourceFromDeposit(resourcesPerGather);
            inventory.AddResource(resourceType, resourcesPerGather);
        }
    }

    public void SetResourceType(GameResourceType resourceType)
    {
        this.resourceType = resourceType;
    }
}
