using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private List<string> pendingTriggers = new List<string>();

    public void ClearTriggers()
    {
        pendingTriggers.Clear();
    }

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

    /// <summary>
    /// Starts the given coroutine and tells the LevelController to ignore player input
    /// until StopAnimation() is called
    /// </summary>
    /// <param name="routine">IEnumerator coroutine to start on this object</param>
    /// <returns>Reference to the Coroutine object that can be stopped if needed</returns>
    protected Coroutine StartAnimation(IEnumerator routine)
    {
        LevelController.Instance.RegisterAnimationBegin(this);
        Coroutine crt = StartCoroutine(routine);
        return crt;
    }

    /// <summary>
    /// Tells the LevelController that an animation on this object has stopped and that it can
    /// safely take user input (if no other animations are playing)
    /// </summary>
    protected void StopAnimation()
    {
        LevelController.Instance.RegisterAnimationEnd(this);
    }

    /// <summary>
    /// Forcibly stops all animations running on this object (not recommended unless to freeze at the end of the level)
    /// </summary>
    public void StopAllAnimations()
    {
        LevelController.Instance.RegisterAnimationEndAll(this);
        StopAllCoroutines();
    }

    /// <summary>
    /// Waits until a trigger has been called on this object using the AnimationTrigger() function
    /// </summary>
    /// <param name="triggerName">Case-sensitive trigger name to check for</param>
    /// <param name="removeTrigger">Whether the trigger should afterwards be removed from the pending triggers</param>
    /// <returns>IEnumerator coroutine</returns>
    protected IEnumerator WaitForTrigger(string triggerName, bool removeTrigger = true)
    {
        while (!HasTrigger(triggerName))
            yield return null;
        if (removeTrigger)
            pendingTriggers.Remove(triggerName);
    }

    protected bool HasTrigger(string triggerName)
    {
        return pendingTriggers.Contains(triggerName);
    }

    /// <summary>
    /// Triggers this object to continue any animations waiting for the specified trigger
    /// </summary>
    /// <param name="triggerName">Case-sensitive trigger name to call</param>
    public void AnimationTrigger(string triggerName)
    {
        pendingTriggers.Add(triggerName);
    }

    /// <summary>
    /// When this object is completely destroyed (wiped from the display), tell the
    /// LevelController that all animations on this object have ended
    /// </summary>
    private void OnDestroy()
    {
        LevelController.Instance.RegisterAnimationEndAll(this);
    }

    /// <summary>
    /// Classes that need to wait for a pressure plate signal will use Run() to check their activation status
    /// </summary>
    public virtual void Run(bool canRun, DynamicObject trigger) { }
}