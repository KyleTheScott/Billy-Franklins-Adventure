//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class Lantern : MonoBehaviour, IInteractable
{
    SpriteRenderer spriteRenderer = null; //spriteRenderer of this lantern
    Light2D light2D = null;
    BoxCollider2D boxCollider = null;
    [SerializeField] private bool lanternInWater;
    [SerializeField] private bool lanterOn = false;
    private Animator lanternAnimator;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        light2D = GetComponentInChildren<Light2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        lanternAnimator = GetComponent<Animator>();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If lantern collided with lightning...
        if (collision.CompareTag("Lightning"))
        {
            LanternToggle();

            //Turn off collider
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
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

    public void LanternToggle()
    {
        if (!lanterOn)
        {
            lanternAnimator.SetBool("Lit", true);
            if (light2D != null)
            {
                light2D.intensity = 1.0f;
            }
            else
            {
                Debug.LogWarning("Light2D is null, Check Lantern");
            }

            //Increase current lit lantern number
            GlobalGameController.instance.IncreaseCurrentLitLanternNum();
        }
    }
}
