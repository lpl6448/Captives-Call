using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Boulder : DynamicObject
{
    [SerializeField]
    private List<TileData> tilesToDestroy;

    [SerializeField]
    private List<TileBase> tilesToPlace;

    private bool willDestroy;
    public bool WillDestroy => willDestroy;

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
        foreach (DynamicObject collidingObj in LevelController.Instance.GetDynamicObjectsOnTile(tilePosition))
            if (!collidingObj.IsTraversable(this))
                return false;

        return true;
    }

    public override bool BlocksLOS()
    {
        return true;
    }

    public override void PostAction()
    {
        // The addition of animations made it so the logic had to be moved to the Move() function.
        // This actually is probably okay because we will be phasing out PreAction() and PostAction() somewhat.
        
    }

    public override void Move(Vector3Int tilePosition, object context)
    {
        // Destroy the tile that the boulder landed on if necessary
        bool destroyTile = false;
        //Brush
        TileBase tile = LevelController.Instance.GetWallTile(TilePosition);
        if (tile != null)
        {
            TileData data = LevelController.Instance.GetTileData(tile);
            if (data != null && tilesToDestroy.Contains(data))
            {
                LevelController.Instance.DeactivateWallTile(TilePosition);
                destroyTile = true;
            }
        }
        //Gap
        tile = LevelController.Instance.GetFloorTile(TilePosition);
        if (tile != null)
        {
            TileData data = LevelController.Instance.GetTileData(tile);
            if (data != null && tilesToDestroy.Contains(data))
            {
                LevelController.Instance.DeactivateFloorTile(TilePosition);
                destroyTile = true;
                LevelController.Instance.floorMap.SetTile(TilePosition, tilesToPlace[0]);
                willDestroy = true;
            }
        }

        // If on a guard, crush the guard
        List<Guard> guards = LevelController.Instance.GetDynamicObjectsOnTile<Guard>(TilePosition);
        foreach (Guard guard in guards)
            LevelController.Instance.DestroyDynamicObject(TilePosition, guard, this);

        Vector3 start = transform.position;
        Vector3 end = LevelController.Instance.CellToWorld(TilePosition) + new Vector3(0.5f, 0.5f, 0);
        StartAnimation(MoveAnimation(start, end, destroyTile, guards));
    }

    private IEnumerator MoveAnimation(Vector3 start, Vector3 end, bool destroyTile, List<Guard> destroyGuards)
    {
        yield return AnimationUtility.StandardLerp(transform, start, end, AnimationUtility.StandardAnimationDuration);

        if (destroyTile)
            LevelController.Instance.wallMap.SetTile(TilePosition, null);
        if (destroyGuards != null)
            foreach (Guard guard in destroyGuards)
                guard.AnimationTrigger("crush");

        StopAnimation();
    }
}