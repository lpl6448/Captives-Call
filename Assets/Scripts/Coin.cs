using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : DynamicObject
{
    [SerializeField]
    private Animation collectAnimation;

    // Start is called before the first frame update
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
        // StartCoroutine is used here instead of StartAnimation so that the coin pickup animation
        // doesn't delay the next player input.
        StartCoroutine(WaitToDestroy());
    }

    private IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(AnimationUtility.StandardAnimationDuration / 4);
        collectAnimation.Play();
        while (collectAnimation.isPlaying)
            yield return null;
        Destroy(gameObject);
    }
}
