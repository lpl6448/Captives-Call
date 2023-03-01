using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents an NPC that moves along a set path and can interact with the Party
/// </summary>
public class Guard : DynamicObject
{
    /// <summary>
    /// Controls whether this guard can walk or not
    /// </summary>
    public bool canMove;

    /// <summary>
    /// Index of the current tile position (in path) that this Guard occupies
    /// </summary>
    public Directions facing;

    public override void UpdateTick()
    {
        //currentPathIndex++;
        //if (currentPathIndex >= path.Count)
        //    currentPathIndex = 0;
        
        // Move/animate this guard to its new position along the path
        //StartCoroutine(Move(path[currentPathIndex]));


    }
}