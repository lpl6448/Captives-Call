using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightManager : MonoBehaviour
{
    [SerializeField]
    private float blendLerp;
    [SerializeField]
    private Tilemap highlightMap;
    [SerializeField]
    private Tilemap wallMap;

    [SerializeField]
    private Color moveHColor, moveHoverColor, abilityHColor, abilityHoverColor, guardLOSColor, clearColor;

    private Dictionary<Vector3Int, Highlight> highlighted;

    private Dictionary<Vector3Int, Color> goalHighlightColors;

    private Vector3Int hoverGrid;

    public Tilemap HighlightMap { get { return highlightMap; } }

    // Awake is called before any GameObjects' start functions
    void Awake()
    {
        highlighted = new Dictionary<Vector3Int, Highlight>();
        goalHighlightColors = new Dictionary<Vector3Int, Color>();
        //Make all highlight tiles clear
        for (int x = -6; x < 4; x++)
        {
            for (int y = -5; y < 5; y++)
            {
                Vector3Int gridPos = new Vector3Int(x, y, 0);
                goalHighlightColors.Add(gridPos, clearColor);

                highlightMap.SetTileFlags(gridPos, TileFlags.None);
                highlightMap.SetColor(gridPos, clearColor);
                highlightMap.SetTileFlags(gridPos, TileFlags.LockColor);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool highlightsDirty = false;

        // Remove old hover highlight
        if (highlighted.TryGetValue(hoverGrid, out Highlight oldHighlight) && oldHighlight.HasFlag(Highlight.Hover))
        {
            highlighted[hoverGrid] &= ~Highlight.Hover;
            highlightsDirty = true;
        }

        // Add new hover highlight
        Vector3Int newHoverGrid = highlightMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (highlighted.TryGetValue(newHoverGrid, out Highlight highlight))
            if (!highlight.HasFlag(Highlight.Hover))
            {
                AddHighlight(newHoverGrid, Highlight.Hover);
                highlightsDirty = true;
            }
        hoverGrid = newHoverGrid;

        // Update the highlight map if necessary
        if (highlightsDirty)
            UpdateHighlightMap();

        // Every frame, update the actual colors to interpolate toward the goal colors
        foreach (KeyValuePair<Vector3Int, Color> goalData in goalHighlightColors)
        {
            Color actualColor = highlightMap.GetColor(goalData.Key);
            if (Mathf.Abs(goalData.Value.r - actualColor.r) + Mathf.Abs(goalData.Value.g - actualColor.g)
                + Mathf.Abs(goalData.Value.b - actualColor.b) + Mathf.Abs(goalData.Value.a - actualColor.a) > 0.001f)
            {
                Color newColor = Color.Lerp(actualColor, goalData.Value, 1 - Mathf.Pow(blendLerp, Time.deltaTime));
                highlightMap.SetTileFlags(goalData.Key, TileFlags.None);
                highlightMap.SetColor(goalData.Key, newColor);
                highlightMap.SetTileFlags(goalData.Key, TileFlags.LockColor);
            }
        }
    }

    /// <summary>
    /// Adds the given highlight to the highlighted dictionary using the gridPosition.
    /// If the tile is already highlighted, it "adds" this highlight onto it, allowing for
    /// different types of highlights overlaid on top of each other.
    /// </summary>
    /// <param name="gridPosition">Grid position to add the highlight to</param>
    /// <param name="highlight">Highlight type that will be added</param>
    private void AddHighlight(Vector3Int gridPosition, Highlight highlight)
    {
        if (!highlighted.ContainsKey(gridPosition))
            highlighted.Add(gridPosition, highlight);
        else

            highlighted[gridPosition] |= highlight;
    }

    /// <summary>
    /// Checks whether the given tile position is currently highlighted as a LOS tile
    /// </summary>
    /// <param name="gridPosition">Grid tile position to check</param>
    /// <returns>Whether the gridPosition is highlighted as an LOS tile</returns>
    public bool HasLOS(Vector3Int gridPosition)
    {
        if (highlighted.TryGetValue(gridPosition, out Highlight highlight))
            return highlight.HasFlag(Highlight.LineOfSight);
        else
            return false;
    }

    /// <summary>
    /// Highlights all tiles that the player can click on.
    /// Different color highlights differentiate if a tile can be moved to or influenced via abilities
    /// </summary>
    /// <param name="party"></param>
    public void HighlightTiles(Party party, List<Guard> guards, Dictionary<TileBase, TileData> dataFromTiles)
    {
        ClearHighlights();

        //Calculate and draw guard lines of sight
        if (guards != null)
            HighlightGuardLOS(guards);

        //Calculate how player highlights are drawn
        if (party != null)
            HighlightMoves(party);

        UpdateHighlightMap();
    }

    /// <summary>
    /// Removes all highlighted tiles on the board
    /// </summary>
    public void ClearHighlights()
    {
        foreach (var tile in highlighted)
        {
            if (goalHighlightColors.ContainsKey(tile.Key))
                goalHighlightColors[tile.Key] = clearColor;
            else
                goalHighlightColors.Add(tile.Key, clearColor);
        }
        highlighted.Clear();
    }

    /// <summary>
    /// Determines whether the given tile position contains any tiles/obstacles that block LOS.
    /// </summary>
    /// <param name="gridPosition">Grid position to check</param>
    /// <returns>Whether the grid position blocks LOS of guards</returns>
    private bool BlocksLOS(Vector3Int gridPosition)
    {
        TileBase target = wallMap.GetTile(gridPosition);
        if (target != null && !LevelController.Instance.GetTileData(target).isAccessible)
            return true;

        foreach (DynamicObject dobj in LevelController.Instance.GetDynamicObjectsOnTile(gridPosition))
            if (dobj.BlocksLOS())
                return true;

        return false;
    }

    /// <summary>
    /// Highlight the tiles that the party can click to move to
    /// </summary>
    /// <param name="gridPosition"></param>
    public void HighlightMoves(Party party)
    {
        Vector3Int gridPosition = party.TilePosition;

        // Highlight all tiles that the party can move to and all tiles that an ability can be used on
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                float distanceFromPlayer = Mathf.Abs(x) + Mathf.Abs(y);
                if (distanceFromPlayer == 1)
                {
                    Vector3Int travTile = new Vector3Int(gridPosition.x + x, gridPosition.y + y, 0);
                    if (party.CanMove(travTile))
                        AddHighlight(travTile, Highlight.Movement);

                    // For now, we can assume that all abilities act upon a particular tile, but if we add abilities
                    // in the future that are just general world actions (like stasis), we would need some kind of UI
                    // call to action for these.
                    if (party.CanUseAbility(travTile))
                        HighlightAbility(travTile);
                }
            }
        }
    }

    /// <summary>
    /// Run through the list of guards in scene and calculate and store each's line of sight tiles
    /// </summary>
    public void HighlightGuardLOS(List<Guard> guards)
    {
        Vector3Int gridPosition;
        foreach (Guard guard in guards)
        {
            gridPosition = guard.TilePosition;
            Vector3Int LOSTile;
            switch (guard.facing)
            {
                case Directions.Up:
                    LOSTile = new Vector3Int(gridPosition.x, gridPosition.y + 1, 0);
                    for (int i = 0; i < 2; i++)
                    {
                        if (BlocksLOS(LOSTile))
                            break;
                        AddHighlight(LOSTile, Highlight.LineOfSight);
                        LOSTile = new Vector3Int(LOSTile.x, LOSTile.y + 1, 0);
                    }
                    break;
                case Directions.Down:
                    LOSTile = new Vector3Int(gridPosition.x, gridPosition.y - 1, 0);
                    for (int i = 0; i < 2; i++)
                    {
                        if (BlocksLOS(LOSTile))
                            break;
                        AddHighlight(LOSTile, Highlight.LineOfSight);
                        LOSTile = new Vector3Int(LOSTile.x, LOSTile.y - 1, 0);
                    }
                    break;
                case Directions.Left:
                    LOSTile = new Vector3Int(gridPosition.x - 1, gridPosition.y, 0);
                    for (int i = 0; i < 2; i++)
                    {
                        if (BlocksLOS(LOSTile))
                            break;
                        AddHighlight(LOSTile, Highlight.LineOfSight);
                        LOSTile = new Vector3Int(LOSTile.x - 1, LOSTile.y, 0);
                    }
                    break;
                case Directions.Right:
                    LOSTile = new Vector3Int(gridPosition.x + 1, gridPosition.y, 0);
                    for (int i = 0; i < 2; i++)
                    {
                        if (BlocksLOS(LOSTile))
                            break;
                        AddHighlight(LOSTile, Highlight.LineOfSight);
                        LOSTile = new Vector3Int(LOSTile.x + 1, LOSTile.y, 0);
                    }
                    break;
            }
        }
    }

    private void HighlightAbility(Vector3Int gridPosition)
    {
        AddHighlight(gridPosition, Highlight.Ability);
    }

    /// <summary>
    /// Updates the color of every tile inside of the highlighted dictionary
    /// </summary>
    private void UpdateHighlightMap()
    {
        foreach (var tile in highlighted)
        {
            Vector3Int gridPosition = tile.Key;
            Highlight highlight = tile.Value;

            // Overlay/blend corresponding colors for each highlight
            Color color = Color.clear;
            if (highlight.HasFlag(Highlight.Movement))
            {
                color = BlendColors(moveHColor, color);
                if (highlight.HasFlag(Highlight.Hover))
                    color = BlendColors(moveHoverColor, color);
            }
            if (highlight.HasFlag(Highlight.Ability))
            {
                color = BlendColors(abilityHColor, color);
                if (highlight.HasFlag(Highlight.Hover))
                    color = BlendColors(abilityHoverColor, color);
            }
            if (highlight.HasFlag(Highlight.LineOfSight))
            {
                color = BlendColors(guardLOSColor, color);
            }

            // Update the highlight map
            if (goalHighlightColors.ContainsKey(gridPosition))
                goalHighlightColors[gridPosition] = color;
            else
                goalHighlightColors.Add(gridPosition, color);
        }
    }

    private Color BlendColors(Color top, Color bottom)
    {
        Color color = Color.Lerp(top, bottom, bottom.a);
        color.a = 1 - (1 - top.a) * (1 - bottom.a);
        return color;
    }
}
