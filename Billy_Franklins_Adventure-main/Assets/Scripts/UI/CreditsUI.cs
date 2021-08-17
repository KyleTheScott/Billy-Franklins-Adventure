using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> credits;
    [SerializeField]
    private float fadeInTime;
    [SerializeField]
    private float fadeOutTime;
    [SerializeField]
    private float displayFullyTime;
    [SerializeField]
    private Image spriteDisplay;
    private Queue<Sprite> creditQueue = new Queue<Sprite>();
    private float timer;
    // Start is called before the first frame update
    private void OnEnable()
    {
        creditQueue.Clear();
        foreach (Sprite creditPage in credits)
        {
            creditQueue.Enqueue(creditPage);
        }
        if (creditQueue.Count > 0)
        {
            StartCoroutine(FadeInCredits());
        }
        
    }

    private IEnumerator FadeInCredits()
    {
        Sprite newCreditPage = creditQueue.Dequeue();

        timer = 0;
        spriteDisplay.sprite = newCreditPage;
        spriteDisplay.color = new Color(spriteDisplay.color.r, spriteDisplay.color.g, spriteDisplay.color.b, 0);

        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            Color newColor = spriteDisplay.color;
            newColor.a = Mathf.Lerp(0, 1, timer / fadeInTime);
            spriteDisplay.color = newColor;
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(DisplaySprite());
    }

    private IEnumerator DisplaySprite()
    {
        yield return new WaitForSeconds(displayFullyTime);
        if (creditQueue.Count > 0)
        {
            StartCoroutine(FadeOutCredits());
        }
    }

    private IEnumerator FadeOutCredits()
    {
        timer = 0;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            Color newColor = spriteDisplay.color;
            newColor.a = Mathf.Lerp(1, 0, timer / fadeInTime);
            spriteDisplay.color = newColor;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(FadeInCredits());
    }


    public void ReturnToMainMenu()
    {
        StopAllCoroutines();

        FindObjectOfType<MainMenu>()?.GetComponent<Canvas>()?.gameObject.SetActive(true);
        FindObjectOfType<MainMenu>()?.GetComponent<MainMenu>()?.MainMenuUICalled();
        FindObjectOfType<MusicController>()?.PlayMenuSelect();

    }
}
