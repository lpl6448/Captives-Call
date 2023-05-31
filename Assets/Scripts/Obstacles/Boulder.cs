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

    [SerializeField]
    private TileData water;

    [SerializeField]
    private ParticleSystem dirtEffect;

    private bool willDestroy;
    public bool WillDestroy => willDestroy;

    private bool moving = false;

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

    public override void PreAction()
    {
        moving = false;
    }
    public override void PostAction()
    {
        // The addition of animations made it so the logic had to be moved to the Move() function.
        // This actually is probably okay because we will be phasing out PreAction() and PostAction() somewhat.

        if (!moving)
            EndMove();
    }

    public override void Move(Vector3Int tilePosition, object context)
    {
        moving = true;
        // Destroy the tile that the boulder landed on if necessary
        bool destroyBrush = false;
        bool fillGap = false;
        bool sink = false;
        //Brush
        TileBase tile = LevelController.Instance.GetWallTile(TilePosition);
        if (tile != null)
        {
            TileData data = LevelController.Instance.GetTileData(tile);
            if (data != null && tilesToDestroy.Contains(data))
            {
                LevelController.Instance.DeactivateWallTile(TilePosition);
                destroyBrush = true;
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
                fillGap = true;
                willDestroy = true;
            }
        }
        //Water
        tile = LevelController.Instance.GetFloorTile(TilePosition);
        if (tile != null)
        {
            TileData data = LevelController.Instance.GetTileData(tile);
            if (data != null && data == water)
            {
                sink = true;
                willDestroy = true;
            }
        }

        // If on a guard, crush the guard
        List<Guard> guards = LevelController.Instance.GetDynamicObjectsOnTile<Guard>(TilePosition);
        foreach (Guard guard in guards)
            LevelController.Instance.DestroyDynamicObject(TilePosition, guard, this);

        Vector3 start = transform.position;
        Vector3 end = LevelController.Instance.CellToWorld(TilePosition) + new Vector3(0.5f, 0.5f, 0);
        StartAnimation(MoveAnimation(start, end, destroyBrush, fillGap, sink, guards));
    }

    private IEnumerator MoveAnimation(Vector3 start, Vector3 end, bool destroyBrush, bool fillGap, bool sink, List<Guard> destroyGuards)
    {
        yield return AnimationUtility.CustomInterpolate(transform, start, end, AnimationUtility.StandardAnimationDuration,
            t =>
            {
                float bt = 0.125f;
                float bp = 0.15f;
                if (t < bt)
                {
                    float mt = t / bt;
                    return mt * mt * bp;
                }
                else
                {
                    float mt = (t - bt) / (1 - bt);
                    return Mathf.Lerp(bp, 1, 1 - (1 - mt) * (1 - mt));
                }
            });

        if (destroyBrush)
            LevelController.Instance.wallMap.SetTile(TilePosition, null);
        if (fillGap)
            LevelController.Instance.floorMap.SetTile(TilePosition, tilesToPlace[0]);
        if (sink)
            yield return SinkAnimation();

        if (destroyGuards != null)
            foreach (Guard guard in destroyGuards)
                guard.AnimationTrigger("crush");

        EndMove();
        StopAnimation();
    }
    private IEnumerator SinkAnimation()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;

        float startTime = Time.time;
        float duration = 0.3f;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            float st = t * t;

            transform.localScale = Vector3.one * Mathf.Lerp(1, 0.85f, st);

            yield return null;
        }

        startTime = Time.time;
        duration = 0.8f;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            float st = Mathf.Lerp(t * t * t, Mathf.Sqrt(t), t);

            color.a = 1 - st;
            sr.color = color;
            transform.localScale = Vector3.one * Mathf.Lerp(0.85f, 0.6f, st);

            yield return null;
        }
    }

    private void EndMove()
    {
        // Notify any pressure plates that the object has finished moving
        foreach (DynamicObject dobj in LevelController.Instance.GetDynamicObjectsOnTile<PressurePlate>(TilePosition))
            dobj.AnimationTrigger("press");
    }
}