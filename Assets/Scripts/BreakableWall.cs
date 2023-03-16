using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : Door
{
    public override bool IsTraversable(DynamicObject mover)
    {
        if (isOpen)
        {
            if (mover is Party)
                return true;
            if (mover is Guard)
                return true;
            if (mover is Boulder)
                return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }
}
