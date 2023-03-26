using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIEffects : MonoBehaviour
{
    public static UIEffects Instance;

    [SerializeField]
    private Animation arrowRotateAnimation;

    [SerializeField]
    private Image fadeImage;

    [SerializeField]
    private Animator magicOverlayAnimator;

    private void Awake()
    {
        Instance = this;
    }

    public void AnimateArrowRotate()
    {
        arrowRotateAnimation.Stop();
        arrowRotateAnimation.Play();
    }

    public void SetFade(float fade)
    {
        Color color = fadeImage.color;
        color.a = fade;
        fadeImage.color = color;
    }

    public IEnumerator AnimateFade(float duration, bool fadeIn = true)
    {
        Color color = fadeImage.color;
        float startAlpha = color.a;
        float goalAlpha = fadeIn ? 1 : 0;
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            color.a = Mathf.Lerp(startAlpha, goalAlpha, t);
            fadeImage.color = color;
            yield return null;
        }
        color.a = goalAlpha;
        fadeImage.color = color;
    }

    public void SetMagicOverlay(bool active)
    {
        magicOverlayAnimator.SetBool("Active", active);
    }
}