using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    private static Dictionary<int, bool> coinsCollected = new Dictionary<int, bool>();
    public static Dictionary<int, bool> CoinsCollected { get { return coinsCollected; } }
    public static int CoinCount { get; set; }

    static GameData()
    {
        coinsCollected.Add(0, false);
        for(int i=1; i<20; i++)
        {
            coinsCollected.Add(i, false);
        }
    }
    
    public static void CollectCoin(int level)
    {
        coinsCollected[level] = true;
        CoinCount = 0;
        foreach(KeyValuePair<int, bool> entry in coinsCollected)
        {
            if (entry.Value)
                CoinCount++;
        }
    }

    public static void LoseCoin(int level)
    {
        coinsCollected[level] = false;
        CoinCount = 0;
        foreach (KeyValuePair<int, bool> entry in coinsCollected)
        {
            if (entry.Value)
                CoinCount++;
        }
    }
}
