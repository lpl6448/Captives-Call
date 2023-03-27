using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameData
{
    private static Dictionary<int, bool> coinsCollected = new Dictionary<int, bool>();
    public static Dictionary<int, bool> CoinsCollected { get { return coinsCollected; } }
    public static int CoinCount { get; set; }
    public static SceneFinder levels;
    public static int levelCount;

    static GameData()
    {
        //Find level count
        levels = new SceneFinder(@".\Assets\Scenes\Levels");
        levelCount = levels.SceneCount();
        //Create coin tracker pair for each level
        coinsCollected.Add(0, false);
        for(int i=1; i<levelCount+1; i++)
        {
            coinsCollected.Add(i, false);
        }
    }
    
    public static void CollectCoin(int level)
    {
        coinsCollected[CorrectLevel(level)] = true;
        CoinCount = 0;
        foreach(KeyValuePair<int, bool> entry in coinsCollected)
        {
            if (entry.Value)
                CoinCount++;
        }
    }

    public static void LoseCoin(int level)
    {
        coinsCollected[CorrectLevel(level)] = false;
        CoinCount = 0;
        foreach (KeyValuePair<int, bool> entry in coinsCollected)
        {
            if (entry.Value)
                CoinCount++;
        }
    }

    public static bool ContainsLevel(string level)
    {
        return levels.Contains(level);
    }

    /// <summary>
    /// Temporary helper method to turn the level num of wizard levels to a lower key while warlock levels aren't there
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static int CorrectLevel(int level)
    {
        if (level > 19)
            return level - 11;
        return level;
    }
}
