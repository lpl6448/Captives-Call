using System;
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
    public GuardManager gm;
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
    /// Dictionary that holds all DynamicObjects currently on the grid
    /// </summary>
    private Dictionary<Vector2Int, List<DynamicObject>> dynamicObjectsGrid;
    /// <summary>
    /// Dictionary that holds all active DynamicObjects by type
    /// </summary>
    private Dictionary<Type, List<DynamicObject>> dynamicObjectsByType;
    /// <summary>
    /// Grid position that the party occupied last turn, used to destroy the party if it collides head-on with a guard
    /// </summary>
    private Vector3Int lastPartyGrid;

    /// <summary>
    /// List of DynamicObjects that are currently blocking player input
    /// </summary>
    private List<DynamicObject> currentlyAnimatedObjects;
    /// <summary>
    /// HashSet containing grid positions of tiles that can no longer affect game logic but will be destroyed imminently
    /// </summary>
    private HashSet<Vector3Int> deactivatedTiles;

    /// <summary>
    /// Gets a list of all DynamicObjects currently occupying the given tile
    /// </summary>
    /// <param name="tile">2D integer vector of the tile to check</param>
    /// <returns>List of all DynamicObjects occupying the tile</returns>
    public List<DynamicObject> GetDynamicObjectsOnTile(Vector3Int tile)
    {
        if (dynamicObjectsGrid.TryGetValue((Vector2Int)tile, out List<DynamicObject> list))
            return list;
        else
            return new List<DynamicObject>();
    }

    /// <summary>
    /// Gets a list of all DynamicObjects, of type T, currently occupying the given tile
    /// </summary>
    /// <param name="tile">2D integer vector of the tile to check</param>
    /// <returns>List of all DynamicObjects, of type T, occupying the tile</returns>
    public List<T> GetDynamicObjectsOnTile<T>(Vector3Int tile) where T : DynamicObject
    {
        if (dynamicObjectsGrid.TryGetValue((Vector2Int)tile, out List<DynamicObject> allList))
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
    /// Gets a list of all DynamicObjects of type T that are on the map
    /// </summary>
    /// <typeparam name="T">Type of DynamicObject to find on the map</typeparam>
    /// <returns>List of all DynamicObjects (type T) on the map</returns>
    public List<T> GetDynamicObjectsByType<T>() where T : DynamicObject
    {
        if (dynamicObjectsByType.TryGetValue(typeof(T), out List<DynamicObject> list))
        {
            List<T> listType = new List<T>();
            foreach (DynamicObject dobj in list)
                listType.Add(dobj as T);
            return listType;
        }
        else
            return new List<T>();
    }

    /// <summary>
    /// Adds the given DynamicObject to the DynamicObjects grid, to allow for easy querying later on
    /// </summary>
    /// <param name="tile">2D integer vector of the tile that the DynamicObject occupies</param>
    /// <param name="dobj">DynamicObject to add</param>
    public void AddDynamicObject(Vector3Int tile, DynamicObject dobj)
    {
        if (dynamicObjectsGrid.TryGetValue((Vector2Int)tile, out List<DynamicObject> list))
            list.Add(dobj);
        else
            dynamicObjectsGrid.Add((Vector2Int)tile, new List<DynamicObject>() { dobj });
    }

    /// <summary>
    /// Moves the DynamicObject to the indicated tile, updating the dynamicObjectsGrid and
    /// calling the Move() function on the DynamicObject
    /// </summary>
    /// <param name="tile">Grid position to move this object to</param>
    /// <param name="dobj">DynamicObject to move</param>
    /// <param name="context">Optional data passed in (about who moved this object, for example)</param>
    public void MoveDynamicObject(Vector3Int tile, DynamicObject dobj, object context = null)
    {
        RemoveDynamicObject(dobj.TilePosition, dobj);
        AddDynamicObject(tile, dobj);
        dobj.UpdateTilePosition(tile);
        dobj.Move(tile, context);
    }

    /// <summary>
    /// Removes the given DynamicObject from the DynamicObjects grid (used if it is deleted or has moved to a new tile)
    /// </summary>
    /// <param name="tile">2D integer vector of the tile that the DynamicObject occupied before</param>
    /// <param name="dobj">DynamicObject to remove</param>
    private bool RemoveDynamicObject(Vector3Int tile, DynamicObject dobj)
    {
        if (dynamicObjectsGrid.TryGetValue((Vector2Int)tile, out List<DynamicObject> list))
        {
            bool removed = list.Remove(dobj);
            if (list.Count == 0)
                dynamicObjectsGrid.Remove((Vector2Int)tile);
            return removed;
        }
        else
            return false;
    }

    /// <summary>
    /// Removes the given DynamicObject from the DynamicObjects grid and calls Destroy() on the object
    /// </summary>
    /// <param name="tile">2D integer vector of the tile that the DynamicObject occupied before</param>
    /// <param name="dobj">DynamicObject to remove</param>
    /// <param name="context">Optional data passed in (about who destroyed this object, for example)</param>
    public void DestroyDynamicObject(Vector3Int tile, DynamicObject dobj, object context = null)
    {
        RemoveDynamicObject(tile, dobj);
        activeDynamicObjects.Remove(dobj);
        dobj.DestroyObject(context);

        if (dynamicObjectsByType.TryGetValue(dobj.GetType(), out List<DynamicObject> list))
        {
            list.Remove(dobj);
            if (list.Count == 0)
                dynamicObjectsByType.Remove(dobj.GetType());
        }
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
    /// Finds the tile on the wallMap at a particular grid position, or null if
    /// (a) no tile is found or (b) if that tile has been deactivated and is no longer affecting game logic
    /// </summary>
    /// <param name="tilePos">Tile position to check</param>
    /// <returns>Tile at the tilePos, or null if there is no tile or if it has been deactivated</returns>
    public TileBase GetWallTile(Vector3Int tilePos)
    {
        TileBase tile = wallMap.GetTile(tilePos);
        if (tile != null && !deactivatedTiles.Contains(tilePos))
            return tile;
        else
            return null;
    }

    /// <summary>
    /// Deactivates the tile at the given grid position, preventing it from affecting game logic
    /// </summary>
    /// <param name="tilePos">Tile position to deactivate</param>
    public void DeactivateWallTile(Vector3Int tilePos)
    {
        deactivatedTiles.Add(tilePos);
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

    /// <summary>
    /// Adds the DynamicObject to a list of objects currently preventing player input
    /// </summary>
    /// <param name="dobj">DynamicObject that is beginning an animation</param>
    public void RegisterAnimationBegin(DynamicObject dobj)
    {
        currentlyAnimatedObjects.Add(dobj);
    }

    /// <summary>
    /// Removes the DynamicObject from a list of objects currently preventing player input
    /// </summary>
    /// <param name="dobj">DynamicObject that is ending an animation</param>
    public void RegisterAnimationEnd(DynamicObject dobj)
    {
        currentlyAnimatedObjects.Remove(dobj);
    }

    /// <summary>
    /// Removes every instance of the DynamicObject from a list of objects currently preventing player input
    /// </summary>
    /// <param name="dobj">DynamicObject that is ending all of its animations</param>
    public void RegisterAnimationEndAll(DynamicObject dobj)
    {
        while (currentlyAnimatedObjects.Contains(dobj))
            currentlyAnimatedObjects.Remove(dobj);
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        lastPartyGrid = pm.party.TilePosition;

        // Initialize all DynamicObjects
        activeDynamicObjects = new List<DynamicObject>(initialDynamicObjects);
        dynamicObjectsGrid = new Dictionary<Vector2Int, List<DynamicObject>>();
        dynamicObjectsByType = new Dictionary<Type, List<DynamicObject>>();
        currentlyAnimatedObjects = new List<DynamicObject>();
        deactivatedTiles = new HashSet<Vector3Int>();
        foreach (DynamicObject dobj in activeDynamicObjects)
        {
            dobj.Initialize();
            Vector3Int tilePos = WorldToCell(dobj.transform.position);
            AddDynamicObject(tilePos, dobj);
            dobj.UpdateTilePosition(tilePos);

            if (dynamicObjectsByType.TryGetValue(dobj.GetType(), out List<DynamicObject> list))
                list.Add(dobj);
            else
                dynamicObjectsByType.Add(dobj.GetType(), new List<DynamicObject>() { dobj });
        }

        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

        // Begin the level turn logic
        StartCoroutine(DoLevel());
    }

    // Update is called once per frame
    void Update()
    {
        //Check to close the game on "esc" press
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.R))
            rs.Reset();
    }

    /// <summary>
    /// Runs turns until a new level is loaded
    /// </summary>
    /// <returns>IEnumerator coroutine</returns>
    private IEnumerator DoLevel()
    {
        while (true)
            yield return DoTurn();
    }

    /// <summary>
    /// Runs the game logic for one turn using coroutines (for more flexibility)
    /// yield return null just means "wait until the next frame"
    /// yield break ends the coroutine
    /// </summary>
    /// <returns>IEnumerator coroutine</returns>
    private IEnumerator DoTurn()
    {
        // Update highlights with party actions included
        hm.ClearHighlights();
        hm.HighlightTiles(pm.party, gm.guardList, dataFromTiles);

        // Perform an action once the player clicks on a tile
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3Int clickGrid = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                bool canMove = validMovementClick(clickGrid);
                bool canUseAbility = validAbilityClick(clickGrid);
                if (canMove || canUseAbility)
                {
                    DoPreAction();

                    //Clear all of the highlights while the CPU takes its turn
                    hm.ClearHighlights();
                    gm.MoveGuards(dataFromTiles, hm);

                    if (canUseAbility)
                        pm.party.UseAbility(clickGrid);
                    else
                    {
                        // Only highlight guard LOS until the player can act again
                        hm.ClearHighlights();
                        hm.HighlightTiles(null, gm.guardList, dataFromTiles);

                        lastPartyGrid = pm.party.TilePosition;
                        MoveDynamicObject(clickGrid, pm.party);
                    }

                    DoPostAction();

                    //Call at the end of cpu loop so highlight does not appear until the CPU turn is completed
                    hm.HighlightTiles(null, gm.guardList, dataFromTiles);
                    //Check if player is in the guardLOS
                    guardAttack();

                    break;
                }
            }
            yield return null;
        }

        // Wait for all animations to finish before continuing
        while (currentlyAnimatedObjects.Count > 0)
            yield return null;

        // Officially end the turn
        deactivatedTiles.Clear();

        // Check if the party has died
        if (pm.party == null)
        {
            rs.Reset();
            yield break;
        }

        //Check if the party has reached the exit
        if (pm.party.TilePosition == grid.WorldToCell(exit.transform.position))
        {
            yield return new WaitForSeconds(1); // Give the player a second to revel in their victory
            SceneManager.LoadScene(nextLevel);
            yield break;
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
        foreach (DynamicObject dobj in new List<DynamicObject>(activeDynamicObjects))
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
        if (hm.HasLOS(pm.party.TilePosition) ||
            gm.TouchingParty(pm.party))
            DestroyDynamicObject(pm.party.TilePosition, pm.party);
        else
        {
            // If any guards on the party's previous tile have just turned toward the player this turn,
            // the player technically collided with them.
            foreach (Guard guard in GetDynamicObjectsOnTile<Guard>(lastPartyGrid))
                if (gm.toTranslate(guard) == lastPartyGrid - pm.party.TilePosition)
                {
                    DestroyDynamicObject(pm.party.TilePosition, pm.party, guard);
                    return;
                }
        }
    }
}
