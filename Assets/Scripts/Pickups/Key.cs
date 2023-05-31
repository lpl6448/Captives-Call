using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : DynamicObject
{
    public override bool IsTraversable(DynamicObject mover)
    {
        return true;
    }

    public override bool CanMove(Vector3Int tilePosition)
    {
        return false;
    }
}
