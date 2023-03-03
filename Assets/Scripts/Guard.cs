using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents an NPC that moves along a set path and can interact with the Party
/// </summary>
public class Guard : DynamicObject
{
    /// <summary>
    /// Allows loading of sprites via unity editor
    /// </summary>
    [SerializeField]
    private List<Sprite> loadSprites;

    /// <summary>
    /// Holds all guard sprites, keyed by directions
    /// </summary>
    private Dictionary<Directions, Sprite> sprites;
    /// <summary>
    /// Controls whether this guard can walk or not
    /// </summary>
    public bool canMove;

    /// <summary>
    /// Index of the current tile position (in path) that this Guard occupies
    /// </summary>
    public Directions facing;

    public override bool IsTraversable(DynamicObject mover)
    {
        if (mover is Party)
            return true; // Currently, the Party can occupy the same tile as a Guard (but the game ends instantly)
        return false;
    }

    public override bool CanMove(Vector3Int tilePosition)
    {
        // Guard cannot move to a tile with a wall on it, unless it is traversable
        TileBase tile = LevelController.Instance.wallMap.GetTile(tilePosition);
        if (tile != null && !LevelController.Instance.GetTileData(tile).isAccessible)
            return false;

        // Guard cannot move to a tile with another DynamicObject on it if the object is not traversable
        foreach (DynamicObject collidingObj in LevelController.Instance.GetDynamicObjectsOnTile((Vector2Int)tilePosition))
            if (!collidingObj.IsTraversable(this))
                return false;

        return true;
    }

    /// <summary>
    /// Initialize and fill dictionary with the guard sprites
    /// </summary>
    public override void Initialize()
    {
        sprites = new Dictionary<Directions, Sprite>();
        sprites.Add(Directions.Up, loadSprites[0]);
        sprites.Add(Directions.Down, loadSprites[1]);
        sprites.Add(Directions.Left, loadSprites[2]);
        sprites.Add(Directions.Right, loadSprites[3]);
        sprites.Add(Directions.Static, loadSprites[4]);
        ChangeDirections(facing);
    }

    /// <summary>
    /// Take in a new direction to face th and change facing and which sprite is showing
    /// </summary>
    /// <param name="facing"></param>
    public void ChangeDirections(Directions newFacing)
    {
        facing = newFacing;
        //Change to correct sprite
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (canMove)
            spriteRenderer.sprite = sprites[facing];
        else
            spriteRenderer.sprite = sprites[Directions.Static];
    }
}