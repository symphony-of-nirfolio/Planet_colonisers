using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Market : MonoBehaviour
{
    private GameResourcesStorage initialMarketStorage;
    private GameResourcesStorage marketStorage;
    private float[] prices = new float[GameResourcesStorage.resourceCount];
    private float[] initial_prices = new float[GameResourcesStorage.resourceCount];

    private bool needsToRefresh;

    private Mutex trade_mutex = new Mutex();

    public float refreshTime = 60;
    public float forcedRefreshTime = 0.1f;
    private float sinceLastRefresh = 0;

    
    void Start()
    {
        // TODO 
        // fill with coresponding initial values
        // hardcoded for now

        initialMarketStorage = new GameResourcesStorage(500);
        marketStorage = new GameResourcesStorage(500);
        for(int i = 0; i < GameResourcesStorage.resourceCount; ++i)
        {
            prices[i] = 500;
            initial_prices[i] = 500;
        }
    }

    void Update()
    {
        sinceLastRefresh += Time.deltaTime;
        if (sinceLastRefresh > refreshTime)
        {
            sinceLastRefresh -= refreshTime;
            RefreshPrices();
        }
        if (needsToRefresh && sinceLastRefresh > forcedRefreshTime)
        {
            sinceLastRefresh -= forcedRefreshTime;
            RefreshPrices();
        }
        
    }

    private void RefreshPrices()
    {
        for (int i = 0; i < GameResourcesStorage.resourceCount; ++i)
        {
            prices[i] = initialMarketStorage.getResourceAmount((GameResourceType)i) * initial_prices[i] / marketStorage.getResourceAmount((GameResourceType)i);
        }
    }

    // returns how many res can be purchaded for 1 res1
    public float GetResourceTradeRate(GameResourceType res1, GameResourceType res2)
    {
        return prices[(int)res1] / prices[(int)res2];
    }

    public float GetAvailableAmount(GameResourceType res)
    {
        return marketStorage.getResourceAmount(res);
    }

    public bool Trade(GameResource res1, GameResource res2)
    {
        if (needsToRefresh)
            return false;

        trade_mutex.WaitOne();

        bool is_successfull_trade = false;
        if (res1.amount > marketStorage.getResourceAmount(res1.resourceType) &&
           res2.amount > marketStorage.getResourceAmount(res2.resourceType))
        {
            marketStorage.RemoveResource(res1.resourceType, res1.amount);
            marketStorage.RemoveResource(res2.resourceType, res2.amount);
            is_successfull_trade = true;
            needsToRefresh = true;
        }

        trade_mutex.ReleaseMutex();
        return is_successfull_trade;
    }
}
