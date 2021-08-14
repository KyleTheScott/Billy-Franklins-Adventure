using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    #region Singleton

    public static GameplayUI instanceGameplayUI;

    private void Awake()
    {
        //Make sure there is only one instance
        if (instanceGameplayUI == null)
        {
            instanceGameplayUI = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField]
    private Image fadeOutImage;
    [SerializeField]
    private float maxAlpha = 0.3f;
    [SerializeField]
    private float fadeOutRate = 0.1f;
    private float currentAlpha = 0.0f;

    private void Start()
    {
        currentAlpha = fadeOutImage.color.a;
    }

    public void ResetFadeOut()
    {
        StopAllCoroutines();
        Color newColor = fadeOutImage.color;
        newColor.a = 0.0f;
        fadeOutImage.color = newColor;
        currentAlpha = 0;
    }

    private IEnumerator StartFadeOut()
    {
        while (currentAlpha <= maxAlpha)
        {
            Color newColor = fadeOutImage.color;
            newColor.a = currentAlpha += fadeOutRate * Time.deltaTime;
            fadeOutImage.color = newColor;
            yield return null;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        Color newColor = fadeOutImage.color;
        newColor.a = 0;
        fadeOutImage.color = newColor;
    }

    public void FadeOut()
    {
        StartCoroutine(StartFadeOut());
    }

}
