using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorMap;

    [SerializeField]
    /// <summary>
    /// Player Field for internal reference to the party object in the scene
    /// </summary>
    public Party party;



    // Start is called before the first frame update
    void Start()
    {

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
