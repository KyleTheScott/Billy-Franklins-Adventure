using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BackgroundLightning : MonoBehaviour
{
    Light2D light2D = null;
    private bool strikingLightning = false; 

    private SpriteRenderer lightningSprite;

    // Start is called before the first frame update
    void Start()
    {
        lightningSprite = gameObject.GetComponent<SpriteRenderer>();
        light2D = GetComponentInChildren<Light2D>();
    }

    public void StartStriking()
    {
        lightningSprite.enabled = true;
        light2D.intensity = 1;
        StartCoroutine("StrikeLightning");
    }

    IEnumerator StrikeLightning()
    {
        LightningController.instance.PlayLightingSoundEffect();
        yield return new WaitForSeconds(1f);
        lightningSprite.enabled = false;
        light2D.intensity = 0;
    }
}
