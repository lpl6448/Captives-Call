using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : DynamicObject
{
    public override bool IsTraversable(DynamicObject mover)
    {
        return true;
    }

    public override bool CanMove(Vector3Int tilePosition)
    {
        return false;
    }

    public override void DestroyObject(object context)
    {
        StartAnimation(WaitToDestroy());
    }

    private IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(AnimationUtility.StandardAnimationDuration / 2);
        StopAnimation();
        Destroy(gameObject);
    }
}
