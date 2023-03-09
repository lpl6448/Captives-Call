using UnityEngine;
using System.Collections;

/// <summary>
/// Utility class containing functions for animating GameObjects
/// </summary>
public static class AnimationUtility
{
    /// <summary>
    /// Default seconds that StandardLerp is recommended to use for most animations
    /// </summary>
    public const float StandardAnimationDuration = 0.4f;

    /// <summary>
    /// Linearly interpolates a Transform's position between the
    /// start and end values, over the given duration
    /// </summary>
    /// <param name="transform">Transform to animate the world position of</param>
    /// <param name="start">Starting world position</param>
    /// <param name="end">Ending world position</param>
    /// <param name="duration">Time (seconds) to animate from start to end</param>
    /// <returns>IEnumerator coroutine</returns>
    public static IEnumerator StandardLerp(Transform transform, Vector3 start, Vector3 end, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        transform.position = end;
    }
}