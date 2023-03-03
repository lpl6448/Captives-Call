using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    /// <summary>
    /// Singleton variable (if we want to use this convention) to allow easy access
    /// to the LevelController from any script in the scene
    /// </summary>
    public static LevelController Instance;

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
    public Tilemap floorMap;
    public Tilemap wallMap;
    //Get references to objects needed to advance to next level
    [SerializeField]
    private string nextLevel;
    [SerializeField]
    private GameObject exit;
    /// <summary>
    /// Field to hold TileData scriptable objects
    /// </summary>
    [SerializeField]
    private List<TileData> tileDatas;
    /// <summary>
    /// List of all DynamicObjects present at the beginning of the level
    /// </summary>
    [SerializeField]
    private List<DynamicObject> initialDynamicObjects;

    /// <summary>
    /// Dictionary that holds all tileData objects
    /// </summary>
    private Dictionary<TileBase, TileData> dataFromTiles;
    /// <summary>
    /// List of all currently active DynamicObjects, used for iteration
    /// </summary>
    private List<DynamicObject> activeDynamicObjects;
    /// <summary>
    /// Diciontary that holds all DynamicObjects currently on the grid
    /// </summary>
    private Dictionary<Vector2Int, List<DynamicObject>> dynamicObjectsGrid;
    /// <summary>
    /// Field to control if the game runs player or CPU logic
    /// </summary>
    private bool playerTurn;
    //TODO: REPLACE TO TRIGGER ON LEVEL LOAD ONCE WE IMPLEMENT THAT
    bool spawnHighlights;

    /// <summary>
    /// Gets a list of all DynamicObjects currently occupying the given tile
    /// </summary>
    /// <param name="tile">2D integer vector of the tile to check</param>
    /// <returns>List of all DynamicObjects occupying the tile</returns>
    public List<DynamicObject> GetDynamicObjectsOnTile(Vector2Int tile)
    {
        if (dynamicObjectsGrid.TryGetValue(tile, out List<DynamicObject> list))
            return list;
        else
            return new List<DynamicObject>();
    }

    /// <summary>
    /// Gets a list of all DynamicObjects, of type T, currently occupying the given tile
    /// </summary>
    /// <param name="tile">2D integer vector of the tile to check</param>
    /// <returns>List of all DynamicObjects, of type T, occupying the tile</returns>
    public List<T> GetDynamicObjectsOnTile<T>(Vector2Int tile) where T : DynamicObject
    {
        if (dynamicObjectsGrid.TryGetValue(tile, out List<DynamicObject> allList))
        {
            List<T> list = new List<T>();
            foreach (DynamicObject dobj in allList)
                if (dobj is T)
                    list.Add(dobj as T);
            return list;
        }
        else
            return new List<T>();
    }

    /// <summary>
    /// Adds the given DynamicObject to the DynamicObjects grid, to allow for easy querying later on
    /// </summary>
    /// <param name="tile">2D integer vector of the tile that the DynamicObject occupies</param>
    /// <param name="dobj">DynamicObject to add</param>
    public void AddDynamicObject(Vector2Int tile, DynamicObject dobj)
    {
        if (dynamicObjectsGrid.TryGetValue(tile, out List<DynamicObject> list))
            list.Add(dobj);
        else
            dynamicObjectsGrid.Add(tile, new List<DynamicObject>() { dobj });
    }

    /// <summary>
    /// Removes the given DynamicObject from the DynamicObjects grid (used if it is deleted or has moved to a new tile)
    /// </summary>
    /// <param name="tile">2D integer vector of the tile that the DynamicObject occupied before</param>
    /// <param name="dobj">DynamicObject to remove</param>
    public bool RemoveDynamicObject(Vector2Int tile, DynamicObject dobj)
    {
        if (dynamicObjectsGrid.TryGetValue(tile, out List<DynamicObject> list))
        {
            bool removed = list.Remove(dobj);
            if (list.Count == 0)
                dynamicObjectsGrid.Remove(tile);
            return removed;
        }
        else
            return false;
    }

    /// <summary>
    /// Retrieves the TileData corresponding to a specific tile on the tilemap,
    /// or null if there is no data for that tile.
    /// </summary>
    /// <param name="tile">TileBase from the tilemap</param>
    /// <returns>Corresponding TileData (or null)</returns>
    public TileData GetTileData(TileBase tile)
    {
        if (dataFromTiles.TryGetValue(tile, out TileData data))
            return data;
        else
            return null;
    }

    /// <summary>
    /// Converts a world position to a grid position within the tilemap.
    /// (This function is just a convenient wrapper for floorMap.WorldToCell().
    /// </summary>
    /// <param name="worldPosition">World position</param>
    /// <returns>Equivalent grid position</returns>
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return floorMap.WorldToCell(worldPosition);
    }

    /// <summary>
    /// Converts a grid position within the tilemap to a world position.
    /// (This function is just a convenient wrapper for floorMap.WorldToCell().
    /// </summary>
    /// <param name="gridPosition">Cell/grid position</param>
    /// <returns>Equivalent world position</returns>
    public Vector3 CellToWorld(Vector3Int gridPosition)
    {
        return floorMap.CellToWorld(gridPosition);
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnHighlights = false;
        playerTurn = true;

        // Initialize all DynamicObjects
        activeDynamicObjects = new List<DynamicObject>(initialDynamicObjects);
        dynamicObjectsGrid = new Dictionary<Vector2Int, List<DynamicObject>>();
        foreach (DynamicObject dobj in activeDynamicObjects)
        {
            dobj.Initialize();
            dobj.UpdateTilePosition(WorldToCell(dobj.transform.position));
        }


        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check to close the game on "esc" press
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (!spawnHighlights)
        {
            hm.HighlightTiles(pm.party, gm.guardList, dataFromTiles);
            spawnHighlights = true;
        }

        if (playerTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3Int clickGrid = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                bool canMove = validMovementClick(clickGrid);
                bool canUseAbility = validAbilityClick(clickGrid);
                if (canMove || canUseAbility)
                {
                    if (canMove && canUseAbility)
                        print("BOTH??");
                    DoPreAction();

                    if (canUseAbility)
                        pm.party.UseAbility(clickGrid);
                    else
                        pm.party.Move(clickGrid);

                    //Check if the party has reached the exit
                    if (grid.WorldToCell(pm.party.transform.position) == grid.WorldToCell(exit.transform.position))
                    {
                        SceneManager.LoadScene(nextLevel);
                        return;
                    }

                    DoPostAction();
                    playerTurn = false;
                }
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
    /// Runs PreAction() on all activeDynamicObjects
    /// </summary>
    private void DoPreAction()
    {
        foreach (DynamicObject dobj in activeDynamicObjects)
            dobj.PreAction();
    }

    /// <summary>
    /// Runs PostAction() on all activeDynamicObjects
    /// </summary>
    private void DoPostAction()
    {
        foreach (DynamicObject dobj in activeDynamicObjects)
            dobj.PostAction();
    }

    /// <summary>
    /// Returns false if clicking on a tile outside of movement range
    /// Also returns false if clicking on a tile marked isAccessible=false
    /// </summary>
    /// <returns></returns>
    private bool validMovementClick(Vector3Int gridPosition)
    {
        Vector3Int partyPos = grid.WorldToCell(pm.party.transform.position);
        int dX = gridPosition.x - partyPos.x;
        int dY = gridPosition.y - partyPos.y;

        // The tile can only be moved to if it is one tile away from the party position
        if (Mathf.Abs(dX) + Mathf.Abs(dY) != 1)
            return false;

        return pm.party.CanMove(gridPosition);
    }

    /// <summary>
    /// Returns true if clicking on a tile that the current PartyMember can use an ability on
    /// </summary>
    /// <returns></returns>
    private bool validAbilityClick(Vector3Int gridPosition)
    {
        return pm.party.CanUseAbility(gridPosition);
    }

    private void guardAttack()
    {
        if (hm.HasLOS(hm.HighlightMap.WorldToCell(pm.party.transform.position)) ||
            gm.TouchingParty(pm.party))
            rs.Reset();
    }
}
