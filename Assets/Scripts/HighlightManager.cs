using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap highlightMap;
    [SerializeField]
    private Tilemap wallMap;
    
    [SerializeField]
    private Color moveHColor, abilityHColor, guardLOSColor, clearColor;

    private Dictionary<Vector3Int, float> highlightedMoves, highlightedLOS;

    // Start is called before the first frame update
    void Start()
    {
        highlightedMoves = new Dictionary<Vector3Int, float>();
        highlightedLOS = new Dictionary<Vector3Int, float>();
        //Make all highlight tiles clear
        for(int x = -6; x<4; x++)
        {
            for(int y = -5; y<5; y++)
            {
                highlightMap.SetTileFlags(new Vector3Int(x,y,0), TileFlags.None);
                highlightMap.SetColor(new Vector3Int(x, y, 0), clearColor);
                highlightMap.SetTileFlags(new Vector3Int(x, y, 0), TileFlags.LockColor);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Highlights all tiles that the player can click on.
    /// Different color highlights differentiate if a tile can be moved to or influenced via abilities
    /// </summary>
    /// <param name="party"></param>
    public void HighlightTiles(Party party, List<Guard> guards, Dictionary<TileBase, TileData> dataFromTiles)
    {
        ClearHighlights();

        //Calculate how player highlights are drawn
        Vector3Int gridPosition = highlightMap.WorldToCell(party.transform.position);

        for(int x=-1; x<=1; x++)
        {
            for(int y=-1; y<=1; y++)
            {
                float distanceFromPlayer = Mathf.Abs(x) + Mathf.Abs(y);
                if(distanceFromPlayer == 1)
                {
                    Vector3Int travTile = new Vector3Int(gridPosition.x + x, gridPosition.y + y, 0);
                    HighlightMoves(travTile);
                }
            }
        }

        //Calculate and draw guard lines of sight
        foreach(Guard guard in guards)
        {
            gridPosition = highlightMap.WorldToCell(guard.transform.position);
            Vector3Int LOSTile;
            switch (guard.facing)
            {
                case Directions.Up:
                    LOSTile = new Vector3Int(gridPosition.x, gridPosition.y+1, 0);
                    for (int i=0; i<2; i++)
                    {
                        TileBase target = wallMap.GetTile(LOSTile);
                        bool accessible = true;
                        if (target != null)
                            accessible = dataFromTiles[target].isAccessible;
                        if (!accessible)
                            break;
                        HighlightGuardLOS(LOSTile);
                        LOSTile = new Vector3Int(LOSTile.x, LOSTile.y + 1, 0);
                    }
                    break;
                case Directions.Down:
                    LOSTile = new Vector3Int(gridPosition.x, gridPosition.y-1, 0);
                    for (int i = 0; i < 2; i++)
                    {
                        TileBase target = wallMap.GetTile(LOSTile);
                        bool accessible = true;
                        if (target != null)
                            accessible = dataFromTiles[target].isAccessible;
                        if (!accessible)
                            break;
                        HighlightGuardLOS(LOSTile);
                        LOSTile = new Vector3Int(LOSTile.x, LOSTile.y - 1, 0);
                    }
                    break;
                case Directions.Left:
                    LOSTile = new Vector3Int(gridPosition.x-1, gridPosition.y, 0);
                    for (int i = 0; i < 2; i++)
                    {
                        TileBase target = wallMap.GetTile(LOSTile);
                        bool accessible = true;
                        if (target != null)
                            accessible = dataFromTiles[target].isAccessible;
                        if (!accessible)
                            break;
                        HighlightGuardLOS(LOSTile);
                        LOSTile = new Vector3Int(LOSTile.x - 1, LOSTile.y, 0);
                    }
                    break;
                case Directions.Right:
                    LOSTile = new Vector3Int(gridPosition.x+1, gridPosition.y, 0);
                    for (int i = 0; i < 2; i++)
                    {
                        TileBase target = wallMap.GetTile(LOSTile);
                        bool accessible = true;
                        if (target != null)
                            accessible = dataFromTiles[target].isAccessible;
                        if (!accessible)
                            break;
                        HighlightGuardLOS(LOSTile);
                        LOSTile = new Vector3Int(LOSTile.x + 1, LOSTile.y, 0);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Removes all highlighted tiles on the board
    /// </summary>
    public void ClearHighlights()
    {
        foreach (var tile in highlightedMoves)
        {
            highlightMap.SetTileFlags(tile.Key, TileFlags.None);
            highlightMap.SetColor(tile.Key, clearColor);
            highlightMap.SetTileFlags(tile.Key, TileFlags.LockColor);
        }
        foreach (var tile in highlightedLOS)
        {
            highlightMap.SetTileFlags(tile.Key, TileFlags.None);
            highlightMap.SetColor(tile.Key, clearColor);
            highlightMap.SetTileFlags(tile.Key, TileFlags.LockColor);
        }
        highlightedMoves.Clear();
        highlightedLOS.Clear();
    }

    /// <summary>
    /// Highlight the tiles that the party can click to move to
    /// </summary>
    /// <param name="gridPosition"></param>
    private void HighlightMoves(Vector3Int gridPosition)
    {
        if (!highlightedMoves.ContainsKey(gridPosition))
            highlightedMoves.Add(gridPosition, 0f);

        foreach(var tile in highlightedMoves)
        {
            highlightMap.SetTileFlags(tile.Key, TileFlags.None);
            highlightMap.SetColor(gridPosition, moveHColor);
            highlightMap.SetTileFlags(gridPosition, TileFlags.LockColor);
        }
    }

    private void HighlightGuardLOS(Vector3Int gridPosition)
    {
        if (!highlightedLOS.ContainsKey(gridPosition))
            highlightedLOS.Add(gridPosition, 0f);

        foreach(var tile in highlightedLOS)
        {
            highlightMap.SetTileFlags(tile.Key, TileFlags.None);
            highlightMap.SetColor(gridPosition, guardLOSColor);
            highlightMap.SetTileFlags(gridPosition, TileFlags.LockColor);
        }
    }
}
