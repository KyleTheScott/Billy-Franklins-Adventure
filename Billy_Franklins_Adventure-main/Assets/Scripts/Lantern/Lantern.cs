//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class Lantern : MonoBehaviour, IInteractable
{
    SpriteRenderer spriteRenderer = null; //spriteRenderer of this lantern
    Light2D light2D = null;
    BoxCollider2D boxCollider = null;

    [SerializeField] Sprite lanternOnSprite = null;
    [SerializeField] Sprite lanternOffSprite = null;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        light2D = GetComponentInChildren<Light2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If lantern collided with lightning...
        if(collision.CompareTag("Lightning"))
        {
            LanternToggle();

            //Turn off collider
            if(boxCollider != null)
            {
                boxCollider.enabled = false;
            }

            //Increase current lit lantern number
            GlobalGameController.instance.IncreaseCurrentLitLanternNum();
        }
    }

    public void Interact()
    {
        LanternToggle();

        //Turn off collider
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
    }

    private void LanternToggle()
    {
        //Change lantern's sprite
        if (spriteRenderer != null && lanternOnSprite != null && lanternOffSprite != null)
        {
            if (spriteRenderer.sprite == lanternOnSprite)
            {
                spriteRenderer.sprite = lanternOffSprite;

                if(light2D != null)
                {
                    light2D.intensity = 0.0f;
                }
                else
                {
                    Debug.LogWarning("Light2D is null, Check Lantern");
                }
            }
            else
            {
                spriteRenderer.sprite = lanternOnSprite;
                if (light2D != null)
                {
                    light2D.intensity = 1.0f;
                }
                else
                {
                    Debug.LogWarning("Light2D is null, Check Lantern");
                }
            }
        }
        else
        {
            Debug.Log("Lantern On, Off sprite is not set!!!");
        }
    }
}
