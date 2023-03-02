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
        LevelController.Instance.RemoveDynamicObject((Vector2Int)this.tilePosition, this);
        this.tilePosition = tilePosition;
        LevelController.Instance.AddDynamicObject((Vector2Int)this.tilePosition, this);
    }

    /// <summary>
    /// Called once at the beginning of every turn. (AI movement, for example, would occur here.)
    /// </summary>
    public virtual void UpdateTick() { }

    /// <summary>
    /// Moves this DynamicObject to the new tile position. By default, the object teleports,
    /// but it can be overloaded to allow for animation (using the IEnumerator)
    /// </summary>
    /// <param name="tilePosition">New grid position</param>
    /// <returns>IEnumerator used for coroutines (unused currently)</returns>
    public virtual void Move(Vector3Int tilePosition)
    {
        UpdateTilePosition(tilePosition);
        // Once the tilemap is set up, we can set the position of this object according to its grid (or animate it in a separate function eventually)
    }
}