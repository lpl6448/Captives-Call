using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for any GameObjects on the tile grid that have independent state
/// and can move between tiles on the grid.
/// </summary>
public abstract class DynamicObject : MonoBehaviour
{
    /// <summary>
    /// Returns whether another DynamicObject can move onto the same tile as this one
    /// </summary>
    /// <param name="mover">DynamicObject that is moving</param>
    /// <returns>Whether the mover can occupy the same tile as this DynamicObject</returns>
    public abstract bool IsTraversable(DynamicObject mover);

    /// <summary>
    /// Returns whether this DynamicObject can move to a particular tile
    /// </summary>
    /// <param name="tilePosition">Grid position to move to</param>
    /// <returns>Whether this object can move to the tilePosition</returns>
    public abstract bool CanMove(Vector3Int tilePosition);

    /// <summary>
    /// Returns whether this DynamicObject blocks the line of sight of guards.
    /// </summary>
    /// <returns>False by default</returns>
    public virtual bool BlocksLOS()
    {
        return false;
    }

    /// <summary>
    /// Current position (x, y) on the tile grid of this DynamicObject
    /// </summary>
    public Vector3Int TilePosition => tilePosition;

    /// <summary>
    /// Current position (x, y) on the tile grid of this DynamicObject.
    /// Other classes should use TilePosition to get and UpdateTilePosition() to set.
    /// </summary>
    private Vector3Int tilePosition;

    /// <summary>
    /// Updates this object's tile position variable (and other useful related values in the future)
    /// </summary>
    /// <param name="tilePosition">New grid position</param>
    public void UpdateTilePosition(Vector3Int tilePosition)
    {
        this.tilePosition = tilePosition;
    }

    /// <summary>
    /// Called once at the initialization of the level.
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    /// Called once at the beginning of every turn.
    /// </summary>
    public virtual void PreAction() { }

    /// <summary>
    /// Called directly after the Party has moved.
    /// </summary>
    public virtual void PostAction() { }

    /// <summary>
    /// Moves this DynamicObject to the new tile position. By default, the object teleports,
    /// but it can be overloaded to allow for animation (using the IEnumerator)
    /// </summary>
    /// <param name="tilePosition">New grid position</param>
    /// <returns>IEnumerator used for coroutines (unused currently)</returns>
    /// <param name="context">Optional data passed in (about who moved this object, for example)</param>
    public virtual void Move(Vector3Int tilePosition, object context)
    {
        UpdateTilePosition(tilePosition);
        transform.position = LevelController.Instance.CellToWorld(TilePosition);
        //TODO: REMOVE THIS LINE AFTER FIXING GAMEOBJECT ANCHOR POINT ISSUE
        transform.Translate(0.5f, 0.5f, 0.0f);
    }

    /// <summary>
    /// Called when this DynamicObject has just been removed from the level's DynamicObject list
    /// </summary>
    /// <param name="context">Optional data passed in (about who destroyed this object, for example)</param>
    public virtual void DestroyObject(object context)
    {
        Destroy(gameObject);
    }
}