using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameData
{
    private static Dictionary<string, bool> coinsCollected = new Dictionary<string, bool>();
    public static Dictionary<string, bool> CoinsCollected { get { return coinsCollected; } }
    public static int CoinCount { get; set; }
    public static SceneFinder levels;
    public static int levelCount;

    static GameData()
    {
        //Find level count
        levels = new SceneFinder(@".\Assets\Scenes\Levels");
        levelCount = levels.SceneCount();
        //Create coin tracker pair for each level
        foreach(string scene in levels.scenes)
        {
            coinsCollected.Add(scene, false);
        }
    }
    
    public static void CollectCoin(string level)
    {
        coinsCollected[level] = true;
        CoinCount = 0;
        foreach(KeyValuePair<string, bool> entry in coinsCollected)
        {
            if (entry.Value)
                CoinCount++;
        }
    }

    public static void LoseCoin(string level)
    {
        coinsCollected[level] = false;
        CoinCount = 0;
        foreach (KeyValuePair<string, bool> entry in coinsCollected)
        {
            if (entry.Value)
                CoinCount++;
        }
    }

    public static bool ContainsLevel(string level)
    {
        return levels.Contains(level);
    }

    public static string GetNextLevel(string level)
    {
        int nextLevel;
        //int index = levels.scenes.IndexOf(level);
        int.TryParse(level, out int current);
        nextLevel = int.MaxValue;
        foreach(string scene in levels.scenes)
        {
            int.TryParse(scene, out int next);
            if(next>current && next<nextLevel)
                nextLevel = next;
        }
        if (levels.scenes.Contains($"{nextLevel}"))
            return $"{nextLevel}";
        else
            return "Thanks";
    }
}
