//using System.Collections;
//using System.Collections.Generic;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class Lantern : MonoBehaviour, IInteractable
{
    Light2D light2D = null;
    BoxCollider2D boxCollider = null;
    [SerializeField] private bool lanternInWater;
    [SerializeField] private bool lanterOn = false;
    private Animator lanternAnimator;
    [SerializeField] private GameObject highlight;
    [SerializeField] private List<GameObject> ghosts = new List<GameObject>();
    [SerializeField] private float ghostDissipateDistance = 5;
    [Header("FMOD Settings")]
    [FMODUnity.EventRef]
    [SerializeField]
    private string lanternHummingSound;
    [FMODUnity.EventRef]
    public string inputSound;
    [SerializeField]
    private float lanternHumingVolume = 0.8f;
    [SerializeField]
    private float inputVolume = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
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

    // called when you press E and are within the Interactable circle
    public void Interact()
    {
        LanternToggle();

        //Turn off collider
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
    }

    public void SetHighlighted(bool state)
    {
        highlight.SetActive(state);
    }

    //lights candle if it is not lit
    public void LanternToggle()
    {
        if (!lanterOn)
        {
            SetHighlighted(false);
            PlayerObjectInteractions.playerObjectIInstance.DisconnectObject(gameObject);
            lanterOn = true;
            lanternAnimator.SetBool("Lit", true);
            FMODUnity.RuntimeManager.PlayOneShot(inputSound, inputVolume);
            //Debug.Log("LanternToggle");
            FMODUnity.RuntimeManager.PlayOneShot(lanternHummingSound, lanternHumingVolume);
            DissipateGhosts();
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

    private void DissipateGhosts()
    {
        for (int i = 0; i < ghosts.Count; i++)
        {
            //Debug.Log("Lantern");
            Ghost ghost = ghosts[i].GetComponent<Ghost>();
            ghost.SetDissipatedByLantern(true);
            ghost.SetGhostDissipation();
        }
    }
}
