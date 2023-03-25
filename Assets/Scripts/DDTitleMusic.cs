using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDTitleMusic : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] titleObj = GameObject.FindGameObjectsWithTag("TitleMusic");
        GameObject[] levelObj = GameObject.FindGameObjectsWithTag("LevelMusic");
        //Replace title music with level music
        if (titleObj.Length > 1 || levelObj.Length>1)
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }
}
