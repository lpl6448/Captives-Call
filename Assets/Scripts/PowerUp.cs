using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : DynamicObject
{
    [SerializeField]
    private ParticleSystem Particles;
    [SerializeField]
    private Animation Animation;
    private Vector3Int oldLocation;

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
        yield return new WaitForSeconds(AnimationUtility.StandardAnimationDuration / 4);
        StopAnimation();

        Animation.Play();
        Particles.Play();

        yield return new WaitForSeconds(0.75f);
        Particles.Stop();
        Particles.Emit(60);

        yield return new WaitForSeconds(2);

        oldLocation = TilePosition;
        Move(new Vector3Int(100, 100, 0), Party.Instance);
    }
}
