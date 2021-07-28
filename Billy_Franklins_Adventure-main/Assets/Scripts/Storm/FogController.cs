using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{
    #region Singleton
    public static FogController instance;

    private void Awake()
    {
        //Make sure there is only one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    private SpriteRenderer fogRenderer;
    [SerializeField]
    private float changeTime;


    // Start is called before the first frame update
    void Start()
    {
        fogRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.one;

        UpdateFogScreenSize();
    }

    void UpdateFogScreenSize()
    {

        float spriteWidth = fogRenderer.sprite.bounds.size.x;
        float spriteHeight = fogRenderer.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector2(worldScreenWidth / spriteWidth, worldScreenHeight / spriteHeight) * 1.2f;

    }

    public void ChangeFogDensity(float newAlpha)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeFogDensity(newAlpha, changeTime));
    }

    private IEnumerator ChangeFogDensity(float newAlpha, float maxTime)
    {
        newAlpha = Mathf.Clamp(maxTime, 0, 1);
        float time = 0;
        float oldAlpha = fogRenderer.color.a;
        Color fogColor = fogRenderer.color;
        do
        {
            fogColor.a = Mathf.Lerp(oldAlpha, newAlpha, time / maxTime);
            fogRenderer.color = fogColor;
            time += Time.deltaTime;
            yield return null;
        } while (time < maxTime);

    }
}
