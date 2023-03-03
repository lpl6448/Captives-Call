using UnityEngine;
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
        return true; // Currently, the Party can occupy the same tile as a Guard (but the game ends instantly)
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