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

    public void MovePlayer(Vector2 worldPosition)
    {
        Vector3Int gridPosition = floorMap.WorldToCell(worldPosition);

        party.transform.position = floorMap.CellToWorld(gridPosition);
        party.transform.Translate(0.5f, 0.5f, 0.0f);
    }
}
