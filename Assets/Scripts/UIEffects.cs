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

    private void Awake()
    {
        Instance = this;
    }

    public void AnimateArrowRotate()
    {
        arrowRotateAnimation.Stop();
        arrowRotateAnimation.Play();
    }

    public IEnumerator AnimateFade(float duration)
    {
        Color color = fadeImage.color;
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            color.a = t;
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1;
        fadeImage.color = color;
    }
}