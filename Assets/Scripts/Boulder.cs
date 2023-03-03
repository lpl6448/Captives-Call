using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Boulder : DynamicObject
{
    [SerializeField]
    private List<TileData> tilesToDestroy;

    /// <summary>
    /// Returns false. No DynamicObjects can occupy the same space as a boulder (currently).
    /// </summary>
    /// <param name="mover"></param>
    /// <returns></returns>
    public override bool IsTraversable(DynamicObject mover)
    {
        return false;
    }

    /// <summary>
    /// Checks whether this boulder can move to a particular tile.
    /// Returns false if there is a wall/unbreakable tile there or another DynamicObject
    /// </summary>
    /// <param name="tilePosition">Grid position to move to</param>
    /// <returns></returns>
    public override bool CanMove(Vector3Int tilePosition)
    {
        // Boulder cannot move to a tile with a wall on it, unless it is brush
        TileBase tile = LevelController.Instance.wallMap.GetTile(tilePosition);
        if (tile != null && !tilesToDestroy.Contains(LevelController.Instance.GetTileData(tile)))
            return false;

        // Boulder cannot move (yet) to a tile with another DynamicObject on it
        foreach (DynamicObject collidingObj in LevelController.Instance.GetDynamicObjectsOnTile((Vector2Int)tilePosition))
            if (!collidingObj.IsTraversable(this))
                return false;

        return true;
    }

    public override bool BlocksLOS()
    {
        return true;
    }

    public override void Move(Vector3Int tilePosition)
    {
        base.Move(tilePosition);

        // Destroy the tile that the boulder landed on if necessary
        TileBase tile = LevelController.Instance.wallMap.GetTile(tilePosition);
        if (tile != null)
        {
            TileData data = LevelController.Instance.GetTileData(tile);
            if (data != null && tilesToDestroy.Contains(data))
                LevelController.Instance.wallMap.SetTile(TilePosition, null);
        }
    }
}