using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : Door
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
        else if(mover is Party && mover.GetComponent<Party>().keyCount>0)
        {
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public int Unlock(int keyCount)
    {
        if(keyCount > 0)
        {
            isOpen = true;
            ChangeSprite(gameObject.GetComponent<SpriteRenderer>());
            return keyCount - 1;
        }
        else
        {
            return 0;
        }
    }
}
