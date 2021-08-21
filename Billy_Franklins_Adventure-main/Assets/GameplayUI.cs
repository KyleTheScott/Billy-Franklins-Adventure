using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private float maxAlpha = 1f;
    [SerializeField]
    private GameObject endText;
    [SerializeField]
    private float fadeOutRate = 0.2f;
    [SerializeField]
    private float fadeInRate = 0.2f;
    [SerializeField]
    private float fadeInTextRate = 0.1f;
    private float currentAlpha = 0.0f;
    private float currntTextAlpha = 0.0f;
    private PlayerGFX playerGFX;
    private Player player;


    private void Start()
    {
        currentAlpha = 1.0f;
        Color newColor = fadeOutImage.color;
        newColor.a = currentAlpha;
        fadeOutImage.color = newColor;
        currentAlpha = 1.0f;
        playerGFX = FindObjectOfType<PlayerGFX>();
        player = FindObjectOfType<Player>();
    }
    public void ResetFadeIn()
    {
        StopAllCoroutines();
        Color newColor = fadeOutImage.color;
        newColor.a = 1.0f;
        fadeOutImage.color = newColor;
        currentAlpha = 1.0f;
    }



    public void ResetFadeOut()
    {
        //Debug.LogError("Problem");
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

        if (player.GetPlayerState() == Player.PlayerState.PLAYER_DEATH_CHARGES_START)
        {
            playerGFX.OutOfChargesDeathEnd();
        }
    }

    private IEnumerator StartFadeIn()
    {
        while (currentAlpha >= 0)
        {
            Color newColor = fadeOutImage.color;
            newColor.a = currentAlpha -= fadeInRate * Time.deltaTime;
            fadeOutImage.color = newColor;
            yield return null;
        }
        FindObjectOfType<CheckPointSystem>().PlayerDeathLoadLevel();
    }

    private void OnLevelWasLoaded(int level)
    {
        Color newColor = fadeOutImage.color;
        newColor.a = 0;
        fadeOutImage.color = newColor;
    }

    public void FadeIn()
    {
        ResetFadeIn();
        StopAllCoroutines();
        FindObjectOfType<Shooting>().StopAiming();
        StartCoroutine(StartFadeIn());
    }

    public void FadeOut()
    {
        ResetFadeOut();
        StopAllCoroutines();
        StartCoroutine(StartFadeOut());
    }

    public void DisplayEndText(bool enabled)
    {
        endText.SetActive(enabled);
    }

    public void FadeInText()
    {
        StartCoroutine(StartFadeInText());
    }

    private IEnumerator StartFadeInText()
    {
        while (currntTextAlpha >= 0)
        {
            Color newColor = fadeOutImage.color;
            newColor.a = currntTextAlpha += fadeInTextRate * Time.deltaTime;
            endText.GetComponent<TextMeshProUGUI>().color = newColor;
            yield return null;
        }
        FindObjectOfType<CheckPointSystem>().PlayerDeathLoadLevel();
    }
}
