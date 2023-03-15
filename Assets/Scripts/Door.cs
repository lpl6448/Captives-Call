using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : DynamicObject
{
    //tracks if the "door" is accessible to move on
    protected bool isOpen;

    public bool willOpen;

    public override bool IsTraversable(DynamicObject mover)
    {
        if (willOpen||isOpen)
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

    public override bool CanMove(Vector3Int tilePosition)
    {
        return false;
    }

    public override void Run(bool canRun)
    {
        isOpen = canRun;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
