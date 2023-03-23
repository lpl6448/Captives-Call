using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneFinder
{
    private string[] scenes;
    private string path;
    
    public SceneFinder(string s_path)
    {
        path = s_path;
        scenes = System.IO.Directory.GetFiles(path, "*.unity");
    }

    public int SceneCount()
    {
        return scenes.Length;
    }
    public void FindDir()
    {
        Debug.Log($"Scene count = {SceneCount()}");
        return;
    }
}