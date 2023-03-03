using System;

/// <summary>
/// Contains all three possible highlights for a tile on the tilemap.
/// Each integer is an increasing power of two, allowing multiple 
/// </summary>
[Flags]
public enum Highlight
{
    None = 0,
    Movement = 1,
    Ability = 2,
    LineOfSight = 4,
}