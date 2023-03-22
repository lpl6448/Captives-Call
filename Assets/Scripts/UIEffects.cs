using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIEffects : MonoBehaviour
{
    public static UIEffects Instance;

    [SerializeField]
    private Animation arrowRotateAnimation;

    private void Awake()
    {
        Instance = this;
    }

    public void AnimateArrowRotate()
    {
        arrowRotateAnimation.Stop();
        arrowRotateAnimation.Play();
    }
}