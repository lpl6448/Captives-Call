using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap highlightMap;
    
    [SerializeField]
    private Color moveHColor, abilityHColor, guardLOSColor, clearColor;

    private Dictionary<Vector3Int, float> highlightedMoves;

    // Start is called before the first frame update
    void Start()
    {
        highlightedMoves = new Dictionary<Vector3Int, float>();
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

    public void HighlightTiles(Party party)
    {
        foreach (var tile in highlightedMoves)
        {
            highlightMap.SetTileFlags(tile.Key, TileFlags.None);
            highlightMap.SetColor(tile.Key, clearColor);
            highlightMap.SetTileFlags(tile.Key, TileFlags.LockColor);
        }
        highlightedMoves.Clear();

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

    }

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
}
