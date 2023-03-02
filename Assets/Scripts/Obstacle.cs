using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all obstacles in the game, which occupy one tile and
/// can block player movement or line of sight.
/// </summary>
public abstract class Obstacle : DynamicObject
{
    /// <summary>
    /// Whether the line of sight is blocked by this Obstacle
    /// </summary>
    public abstract bool BlocksLOS { get; }
}