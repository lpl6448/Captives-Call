using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelController : MonoBehaviour
{
    //Get a reference to all controllers in the scene
    [SerializeField]
    private HighlightManager hm;
    [SerializeField]
    private PlayerManager pm;
    [SerializeField]
    private GuardManager gm;
    [SerializeField]
    private ResetScene rs;
    //Get a reference to necessary grid items
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Tilemap floorMap;
    [SerializeField]
    private Tilemap wallMap;
    /// <summary>
    /// Field to hold TileData scriptable objects
    /// </summary>
    [SerializeField]
    private List<TileData> tileDatas;

    /// <summary>
    /// Dictionary that holds all tileData objects
    /// </summary>
    private Dictionary<TileBase, TileData> dataFromTiles;
    /// <summary>
    /// Field to control if the game runs player or CPU logic
    /// </summary>
    private bool playerTurn;
    //TODO: REPLACE TO TRIGGER ON LEVEL LOAD ONCE WE IMPLEMENT THAT
    bool spawnHighlights;

    // Start is called before the first frame update
    void Start()
    {
        spawnHighlights = false;
        playerTurn = true;
        pm.party.Init();

        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach(var tileData in tileDatas)
        {
            foreach(var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawnHighlights)
        {
            hm.HighlightTiles(pm.party, gm.guardList, dataFromTiles);
            spawnHighlights = true;
        }

        if (playerTurn)
        {
            if (Input.GetMouseButtonDown(0) && validClick())
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pm.MovePlayer(mousePosition);
                pm.party.UpdateTick();
                playerTurn = false;
            }
        }
        else
        {
            //Clear all of the highlights while the CPU takes its turn
            hm.ClearHighlights();
            gm.MoveGuards(dataFromTiles);
            //Call at the end of cpu loop so highlight does not appear until the CPU turn is completed
            hm.HighlightTiles(pm.party, gm.guardList, dataFromTiles);
            //Check if player is in the guardLOS
            guardAttack();
            playerTurn = true;
        }
    }

    /// <summary>
    /// Returns false is clicking on a tile outside of movement range
    /// Also returns false if clicking on a tile marked isAccessible=false
    /// </summary>
    /// <returns></returns>
    private bool validClick()
    {
        Vector3Int gridPosition = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3Int partyPos = grid.WorldToCell(pm.party.transform.position);
        int dX = gridPosition.x-partyPos.x;
        int dY = gridPosition.y-partyPos.y;
        TileBase clickedTile = wallMap.GetTile(gridPosition);
        bool accessible=true;
        if (clickedTile != null)
            accessible = dataFromTiles[clickedTile].isAccessible;

        return ((Mathf.Abs(dX) + Mathf.Abs(dY)) == 1)&&accessible;
    }

    private void guardAttack()
    {
        if (hm.HighlightedLOS.ContainsKey(hm.HighlightMap.WorldToCell(pm.party.transform.position)) ||
            gm.TouchingParty(pm.party))
            rs.Reset();
    }
}
