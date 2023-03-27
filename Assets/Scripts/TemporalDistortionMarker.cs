using UnityEngine;
using System.Collections;

public class TemporalDistortionMarker : MonoBehaviour
{
    [SerializeField]
    private Animation markerAnimation;

    private void Start()
    {
        markerAnimation.Play("TemporalDistortionIn");
    }

    public void DestroyMarker()
    {
        markerAnimation.Play("TemporalDistortionOut");
    }
    private IEnumerator WaitToDestroy()
    {
        while (markerAnimation.isPlaying)
            yield return null;
        Destroy(gameObject);
    }
}