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
    [SerializeField]
    private Image backgroundImage;
    private Queue<Sprite> creditQueue = new Queue<Sprite>();
    private float timer;
    // Start is called before the first frame update
    private void OnEnable()
    {
        AdjustCreditsSize();
        LoadCredits();
        
    }

    private void AdjustCreditsSize()
    {
        float spriteWidth = backgroundImage.sprite.bounds.size.x;
        float spriteHeight = backgroundImage.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector2(worldScreenWidth / spriteWidth, worldScreenHeight / spriteHeight) * 1.01f;
    }

    private void LoadCredits()
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

        StartCoroutine(FadeOutCredits());
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
        if (creditQueue.Count == 0)
        {
            LoadCredits();
        }
        else
        {
            StartCoroutine(FadeInCredits());
        }
        
    }


    public void ReturnToMainMenu()
    {
        StopAllCoroutines();

        FindObjectOfType<MainMenu>()?.GetComponent<Canvas>()?.gameObject.SetActive(true);
        FindObjectOfType<MainMenu>()?.GetComponent<MainMenu>()?.MainMenuUICalled();
        FindObjectOfType<MusicController>()?.PlayMenuSelect();

    }
}
