using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneFinder
{
    private string[] scenesFull;
    public List<string> scenes;
    private string path;
    
    public SceneFinder(string s_path)
    {
        scenes = new List<string>();
        path = Path.Combine(Application.streamingAssetsPath, s_path);
        scenesFull = System.IO.Directory.GetFiles(path, "*.unity");
        foreach (string s in scenesFull)
        {
            scenes.Add(Path.GetFileNameWithoutExtension(s));
        }
    }

    public int SceneCount()
    {
        return scenes.Count;
    }

    /// <summary>
    /// Check if a certain level scene exists
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    public bool Contains(string scene)
    {
        return scenes.Contains(scene);
    }
}