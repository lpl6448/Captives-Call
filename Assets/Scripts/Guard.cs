using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents an NPC that moves along a set path and can interact with the Party
/// </summary>
public class Guard : DynamicObject
{
    /// <summary>
    /// List of grid positions that this Guard will move on
    /// </summary>
    public List<Vector3Int> path;

    /// <summary>
    /// Index of the current tile position (in path) that this Guard occupies
    /// </summary>
    public int currentPathIndex = 0;

    public override void UpdateTick()
    {
        currentPathIndex++;
        if (currentPathIndex >= path.Count)
            currentPathIndex = 0;
        
        // Move/animate this guard to its new position along the path
        StartCoroutine(Move(path[currentPathIndex]));
    }
}