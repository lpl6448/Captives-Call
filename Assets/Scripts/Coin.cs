using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : DynamicObject
{
    // Start is called before the first frame update
    public override bool IsTraversable(DynamicObject mover)
    {
        return true;
    }

    public override bool CanMove(Vector3Int tilePosition)
    {
        return false;
    }
}
