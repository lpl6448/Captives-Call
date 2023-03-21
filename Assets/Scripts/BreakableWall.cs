using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : Door
{
    [SerializeField]
    private GameObject breakEffectPrefab;

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

    protected override void UpdateState()
    {
        base.UpdateState();
        if (isOpen)
            Instantiate(breakEffectPrefab, LevelController.Instance.CellToWorld(TilePosition) + Vector3.one / 2, Quaternion.identity);
    }
}
