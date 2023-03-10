using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{
    private Tilemap floorMap;

    /// <summary>
    /// Player Field for internal reference to the party object in the scene
    /// </summary>
    public Party party;



    // Start is called before the first frame update
    void Awake()
    {
        //Define the party object
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag("Party");
        if(foundObjects.Length>0)
            party = foundObjects[0].GetComponent<DynamicObject>().GetComponent<Party>();
        //Find tilemaps and assign them to their respective variables
        floorMap = GameObject.FindGameObjectsWithTag("Floor")[0].GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Move the party to the tile that the player clicks on
    /// </summary>
    /// <param name="worldPosition"></param>
    public void MovePlayer(Vector2 worldPosition)
    {
        // I moved this code over to DynamicObject to generalize movement a bit more
    }
}
