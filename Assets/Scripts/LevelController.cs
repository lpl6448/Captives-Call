using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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
    [SerializeField]
    private FxController am;
    //Get a reference to necessary grid items
    private Grid grid;
    public Tilemap floorMap;
    public Tilemap wallMap;
    //[SerializeField]
    private string currentLevel;
    public string CurrentLevel => currentLevel;
    //Get references to objects needed to advance to next level
    private string nextLevel;
    public string NextLevel => nextLevel;
    /// <summary>
    /// Field to hold TileData scriptable objects
    /// </summary>
    [SerializeField]
    private List<TileData> tileDatas;

    /// <summary>
    /// List of exit tiles (up, down, left, right), used to figure out what direction
    /// to move the player off-screen after beating the level
    /// </summary>
    [SerializeField]
    private Tile[] exitTiles;
    /// <summary>
    /// List of tile tiles (up, down, left, right), used to figure out what direction
    /// to move the player from at the beginning of the level
    /// </summary>
    [SerializeField]
    private Tile[] startTiles;

    /// <summary>
    /// List of all DynamicObjects present at the beginning of the level
    /// </summary>
    private List<DynamicObject> initialDynamicObjects;

    /// <summary>
    /// Holds game object that is responsible to triggering level progression
    /// </summary>
    private GameObject exit;
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
    /// Whether the game is accepting user input currently (false at the end of the level or when the party dies)
    /// </summary>
    private bool acceptingUserInput;

    /// <summary>
    /// Whether user has selected if they are moving or attacking
    /// </summary>
    private bool acceptingActionInput;
    public bool AcceptingActionInput { set { acceptingActionInput = value; } }

    /// <summary>
    /// List of DynamicObjects that are currently blocking player input
    /// </summary>
    private List<DynamicObject> currentlyAnimatedObjects;
    /// <summary>
    /// HashSet containing grid positions of tiles that can no longer affect game logic but will be destroyed imminently
    /// </summary>
    private HashSet<Vector3Int> deactivatedTiles;
    /// <summary>
    /// Holds how many moves the player has made this level
    /// </summary>
    private int movesTaken;
    public int MovesTaken => movesTaken;
    //Triggers the turn when the player switches characters
    private bool characterSwitch;
    /// <summary>
    /// How many tunrs remain of wizard stasis
    /// </summary>
    private int stasisCount;
    public int StasisCount
    {
        get { return stasisCount; }
        set { stasisCount = value; }
    }
    /// <summary>
    /// How many turns remain of temporal distortion
    /// </summary>
    private int distortionCount;
    public int DistortionCount
    {
        get { return distortionCount; }
        set { distortionCount = value; }
    }
    /// <summary>
    /// Reflected the hidden count of party
    /// </summary>
    private int hiddenCount;
    public int HiddenCount => hiddenCount;
    /// <summary>
    /// Used to stop a double shanty turn because Nick is too dumb to find the root of the bug
    /// </summary>
    private bool justSang;

    private void FindAllGameObjects()
    {
        initialDynamicObjects = new List<DynamicObject>();
        //Find and add all dynamic objects in the scene to the initialdynamicobject list
        GameObject[] foundObjects;
        string[] tags = { "Party", "Guard", "Boulder", "Pressure", "Gate", "Key", "Locked", "Breakable", "Power", "Spikes", "Coin" };
        for (int i = 0; i < tags.Length; i++)
        {
            foundObjects = GameObject.FindGameObjectsWithTag(tags[i]);
            if (foundObjects.Length > 0)
            {
                foreach (GameObject g in foundObjects)
                {
                    initialDynamicObjects.Add(g.GetComponent<DynamicObject>());
                }
            }
        }
        //Find the exit and assign it to the exit variable
        exit = GameObject.FindGameObjectsWithTag("Exit")[0];
        //Find grid and assign it to grid variable
        grid = GameObject.FindGameObjectsWithTag("Grid")[0].GetComponent<Grid>();
        //Find tilemaps and assign them to their respective variables
        floorMap = GameObject.FindGameObjectsWithTag("Floor")[0].GetComponent<Tilemap>();
        wallMap = GameObject.FindGameObjectsWithTag("Walls")[0].GetComponent<Tilemap>();
    }

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
    /// Finds the tile on the wallMap at a particular grid position, or null if
    /// (a) no tile is found or (b) if that tile has been deactivated and is no longer affecting game logic
    /// </summary>
    /// <param name="tilePos">Tile position to check</param>
    /// <returns>Tile at the tilePos, or null if there is no tile or if it has been deactivated</returns>
    public TileBase GetFloorTile(Vector3Int tilePos)
    {
        TileBase tile = floorMap.GetTile(tilePos);
        if (tile != null && !deactivatedTiles.Contains(tilePos))
            return tile;
        else
            return null;
    }

    /// <summary>
    /// Deactivates the tile at the given grid position, preventing it from affecting game logic
    /// </summary>
    /// <param name="tilePos">Tile position to deactivate</param>
    public void DeactivateFloorTile(Vector3Int tilePos)
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

    public void BeginStasis(int turns)
    {
        StasisCount = turns;
        UIEffects.Instance.SetMagicOverlay(true);
    }
    public void BeginTemporalDistortion(int turns)
    {
        DistortionCount = turns;
        gm.AddTemporalDistortionMarkers();
    }

    private void EndStasis()
    {
        StasisCount = 0;
        UIEffects.Instance.SetMagicOverlay(false);
    }
    private void EndTemporalDistortion()
    {
        DistortionCount = 0;
        gm.RemoveTemporalDistortionMarkers();
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = SceneManager.GetActiveScene().name;
        //Define nextLevel as the next scene or "Thanks" if there are no more scenes
        int.TryParse(currentLevel, out int current);
        nextLevel = GameData.GetNextLevel(currentLevel);
        //Use level 1 literals since party manager is not fully defined when this function runs
        lastPartyGrid = Vector3Int.FloorToInt(new Vector3(-2.5f, 1.5f, 0.0f));
        // Initialize all DynamicObjects
        FindAllGameObjects();
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
        movesTaken = 0;
        characterSwitch = false;

        //Remove coin from level if already collected 
        //int.TryParse(currentLevel, out int levelNum);
        if (GameData.CoinsCollected[currentLevel])
        {
            GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
            if (coins.Length > 0)
            {
                foreach (GameObject coin in coins)
                {
                    DynamicObject dCoin = coin.GetComponent<DynamicObject>();
                    DestroyDynamicObject(dCoin.TilePosition, dCoin);
                }
            }
        }
        // Begin the level turn logic
        StartCoroutine(DoLevel());
    }

    // Update is called once per frame
    void Update()
    {
        //Application.Quit();

        if (Input.GetKeyDown(KeyCode.R))
        {
            //int.TryParse(currentLevel, out int levelNum);
            GameData.LoseCoin(currentLevel);
            rs.Reset();
        }
    }

    /// <summary>
    /// Runs turns until a new level is loaded
    /// </summary>
    /// <returns>IEnumerator coroutine</returns>
    private IEnumerator DoLevel()
    {
        hm.HighlightTiles(null, gm.guardList, dataFromTiles);
        UIEffects.Instance.SetFade(1);

        // First, we animate the party in from the start tile
        Vector3Int startDir = Vector3Int.zero;
        for (int i = 0; i < 4; i++)
        {
            Vector3Int dir = i == 0 ? Vector3Int.up
                : i == 1 ? Vector3Int.down
                : i == 2 ? Vector3Int.left
                : i == 3 ? Vector3Int.right : Vector3Int.zero;
            if (Array.IndexOf(startTiles, GetWallTile(pm.party.TilePosition + dir)) != -1)
            {
                startDir = dir;
                break;
            }
        }
        if (startDir != Vector3Int.zero)
        {
            pm.party.transform.position = CellToWorld(pm.party.TilePosition) + new Vector3(0.5f, 0.5f, 0) + (Vector3)startDir * 1.5f;
            pm.party.Move(pm.party.TilePosition, 3f);
            yield return UIEffects.Instance.AnimateFade(0.75f, false);
            while (currentlyAnimatedObjects.Count > 0)
                yield return null;
        }
        else
            yield return UIEffects.Instance.AnimateFade(0.75f, false);

        acceptingUserInput = true;
        while (acceptingUserInput)
        {
            yield return DoTurn();
            yield return null;
        }
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
            //Check to close the game on "esc" press
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                yield return HUDManager.Instance.Paused();
            }
            // Check if the player has just clicked (not over the UI) or if the character has been switched
            if ((Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() || characterSwitch) && !justSang)
            {
                Vector3Int clickGrid = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                bool canMove = validMovementClick(clickGrid);
                bool canUseAbility = validAbilityClick(clickGrid);
                if (characterSwitch)
                    clickGrid = pm.party.TilePosition;
                if (canMove || canUseAbility || characterSwitch)
                {
                    am.GoodClick();

                    DoPreAction();

                    //Clear all of the highlights while the CPU takes its turn
                    //Wait to clear if the selection overlay is going to pop up
                    if (!(canMove && canUseAbility))
                        hm.ClearHighlights();
                    //Don't move guards if there is a temporal distortion cast or sea shanty is being sung
                    if (distortionCount < 1 && !(canUseAbility && canMove) &&
                        !(pm.party.currentMember == PartyMember.Sailor && canUseAbility && clickGrid == pm.party.TilePosition) &&
                        !(pm.party.currentMember == PartyMember.Wizard && canUseAbility && pm.party.poweredUp && clickGrid == pm.party.TilePosition))
                        gm.MoveGuards(dataFromTiles, hm);


                    if (canUseAbility && canMove)
                    {
                        acceptingActionInput = true;
                        while (acceptingActionInput)
                            yield return HUDManager.Instance.ChooseAction(clickGrid, Input.mousePosition);
                    }
                    else if (canUseAbility)
                    {
                        AbilityTurn(clickGrid);
                        //Check if sea shanty was just used
                        if ((pm.party.currentMember == PartyMember.Sailor && canUseAbility && clickGrid == pm.party.TilePosition))
                            justSang = true;
                    }
                    else
                    {
                        MoveTurn(clickGrid);
                    }
                    DoPostAction();

                    //Call at the end of cpu loop so highlight does not appear until the CPU turn is completed
                    hm.HighlightTiles(null, gm.guardList, dataFromTiles);
                    //Check if player is in the guardLOS
                    guardAttack();
                    if (stasisCount > 0)
                    {
                        stasisCount--;
                        if (stasisCount == 0)
                            EndStasis();
                    }
                    if (distortionCount > 0)
                    {
                        distortionCount--;
                        if (distortionCount == 0)
                            EndTemporalDistortion();
                    }
                    hiddenCount = pm.party.Hidden;
                    movesTaken++;
                    break;
                }
                else
                {
                    am.BadClick();
                }
            }
            //TODO: FIND A BETTER SOLUTION BECAUSE THIS IS DUMB
            else if (justSang)
            {
                justSang = false;
            }
            yield return null;
        }

        characterSwitch = false;

        // Wait for all animations to finish before continuing
        while (currentlyAnimatedObjects.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.P))
                foreach (DynamicObject dobj in currentlyAnimatedObjects)
                    Debug.Log(dobj);
            yield return null;
        }
        // Officially end the turn
        deactivatedTiles.Clear();

        // Since guards can currently turn after animations are finished (shantyman), we need the guards to attack again at the end of the turn.
        hm.HighlightTiles(null, gm.guardList, dataFromTiles);
        guardAttack();
        guardWillPress();

        // Check if the party has died
        if (pm.party.dead)
        {
            acceptingUserInput = false;
            StartCoroutine(DefeatAnimation());
            yield break;
        }

        //Check if any boulders need to be destroyed
        GameObject[] boulders = GameObject.FindGameObjectsWithTag("Boulder");
        foreach (GameObject boulder in boulders)
        {
            if (boulder.GetComponent<Boulder>().WillDestroy)
            {
                DynamicObject b = boulder.GetComponent<DynamicObject>();
                DestroyDynamicObject(b.TilePosition, b);
            }
        }

        //Check if the party has reached the exit
        if (pm.party.TilePosition == grid.WorldToCell(exit.transform.position))
        {
            StartCoroutine(VictoryAnimation());
            acceptingUserInput = false;
            yield break;
        }
    }

    //Helper methods
    public void AbilityTurn(Vector3Int clickGrid)
    {
        pm.party.UseAbility(clickGrid, am);
    }
    public void MoveTurn(Vector3Int clickGrid)
    {
        // Only highlight guard LOS until the player can act again
        hm.ClearHighlights();
        hm.HighlightTiles(null, gm.guardList, dataFromTiles);

        lastPartyGrid = pm.party.TilePosition;
        MoveDynamicObject(clickGrid, pm.party);
    }
    public void CallMoveGuards()
    {
        hm.ClearHighlights();
        gm.MoveGuards(dataFromTiles, hm);
    }

    /// <summary>
    /// Runs PreAction() on all activeDynamicObjects
    /// </summary>
    private void DoPreAction()
    {
        foreach (DynamicObject dobj in activeDynamicObjects)
            dobj.ClearTriggers();
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
        if (pm.party == null)
            return false;

        Vector3Int partyPos = pm.party.TilePosition;
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
        if ((hm.HasLOS(pm.party.TilePosition) && pm.party.Hidden < 1) ||
            gm.TouchingParty(pm.party))
        {
            //am.Defeat(rs);
            DestroyDynamicObject(pm.party.TilePosition, pm.party);
        }
        else
        {
            // If any guards on the party's previous tile have just turned toward the player this turn,
            // the player technically collided with them.
            foreach (Guard guard in GetDynamicObjectsOnTile<Guard>(lastPartyGrid))
            {
                if (gm.ToTranslate(guard) == lastPartyGrid - pm.party.TilePosition)
                {
                    //am.Defeat(rs);
                    DestroyDynamicObject(pm.party.TilePosition, pm.party, guard);
                    return;
                }
            }
        }
    }

    private void guardWillPress()
    {
        foreach (PressurePlate plate in GetDynamicObjectsByType<PressurePlate>())
        {
            // Check if this plate will be pressed by a boulder or party next turn
            bool isPressedByNotGuard = false;
            if (GetDynamicObjectsOnTile<Boulder>(plate.TilePosition).Count > 0
                || GetDynamicObjectsOnTile<Party>(plate.TilePosition).Count > 0)
                isPressedByNotGuard = true;

            // Check if this plate will be pressed by a guard next turn
            bool willPressByGuard = false;
            foreach (Guard guard in gm.guardList)
            {
                Vector3Int nextTile =
                    guard.CanMove(guard.TilePosition + gm.ToTranslate(guard)) ? guard.TilePosition + gm.ToTranslate(guard)
                    : guard.CanMove(guard.TilePosition - gm.ToTranslate(guard)) ? guard.TilePosition - gm.ToTranslate(guard) : guard.TilePosition;

                if (nextTile == plate.TilePosition)
                {
                    willPressByGuard = true;
                    break;
                }
            }

            if (isPressedByNotGuard || willPressByGuard)
                foreach (DynamicObject dobj in plate.linkedObjects)
                    if (dobj is Door)
                    {
                        Door door = dobj as Door;
                        door.WillActivate = true;
                    }
        }
    }

    public void TriggerCharacterSwitch(int characterIndex)
    {
        characterSwitch = true;
        pm.party.ChangeCharacter(characterIndex);
    }

    private IEnumerator DefeatAnimation()
    {
        yield return new WaitForSeconds(0.25f);

        am.Defeat();
        hm.BlinkTile(pm.party.TilePosition);

        yield return new WaitForSeconds(0.75f);

        // Find guard that saw player
        if (hm.HasLOS(pm.party.TilePosition))
        {
            Guard foundGuard = null;
            foreach (Guard guard in gm.guardList)
            {
                Vector3Int faceDir = guard.facing == Directions.Right ? Vector3Int.right
                    : guard.facing == Directions.Up ? Vector3Int.up
                    : guard.facing == Directions.Left ? Vector3Int.left
                    : guard.facing == Directions.Down ? Vector3Int.down : Vector3Int.zero;
                Vector3Int losTile = guard.TilePosition + faceDir;
                bool foundPlayer = false;
                for (int i = 0; i < 2; i++)
                {
                    if (hm.BlocksLOS(losTile) || !guard.CanMove(losTile))
                        break;

                    if (pm.party.TilePosition == losTile)
                    {
                        foundPlayer = true;
                        break;
                    }
                    losTile += faceDir;
                }
                if (foundPlayer)
                {
                    foundGuard = guard;
                    break;
                }
            }

            if (foundGuard != null)
            {
                int dif = (int)(pm.party.TilePosition - foundGuard.TilePosition).magnitude;
                MoveDynamicObject(pm.party.TilePosition, foundGuard, dif * 4f);
            }
        }

        //Reset coin for level
        //int.TryParse(currentLevel, out int levelNum);
        GameData.LoseCoin(currentLevel);

        yield return new WaitForSeconds(0.5f);
        yield return UIEffects.Instance.AnimateFade(0.75f);
        yield return new WaitForSeconds(1);

        rs.Reset();
    }
    private IEnumerator VictoryAnimation()
    {
        am.Victory();

        yield return new WaitForSeconds(2);

        // Find the exit tile that the player is on
        int tileIndex = Array.IndexOf(exitTiles, GetWallTile(pm.party.TilePosition));
        if (tileIndex != -1)
        {
            // Move the player off-screen using the exit tile direction
            Vector3Int dir = tileIndex == 0 ? Vector3Int.up
                : tileIndex == 1 ? Vector3Int.down
                : tileIndex == 2 ? Vector3Int.left
                : tileIndex == 3 ? Vector3Int.right : Vector3Int.zero;
            MoveDynamicObject(pm.party.TilePosition - dir, pm.party, 2f);
        }

        yield return new WaitForSeconds(0.25f);
        yield return UIEffects.Instance.AnimateFade(0.75f);
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(nextLevel);
    }
}
