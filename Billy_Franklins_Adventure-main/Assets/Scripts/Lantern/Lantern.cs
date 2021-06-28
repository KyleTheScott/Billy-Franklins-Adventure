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


    [FMODUnity.EventRef]
    public string inputSound;

    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponentInChildren<Light2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        lanternAnimator = GetComponent<Animator>();

        Transform GhostTransform = GameObject.Find("Ghosts").transform;

        foreach (Transform g in GhostTransform)
        {
            if (Vector2.Distance(transform.position, g.transform.position) <= ghostDissipateDistance)
            {
                ghosts.Add(g.gameObject);
            }
        }
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
            lanterOn = true;
            lanternAnimator.SetBool("Lit", true);
            FMODUnity.RuntimeManager.PlayOneShot(inputSound);
            if (light2D != null)
            {
                light2D.intensity = 1.0f;
                DissipateGhosts();
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
            Ghost ghost = ghosts[i].GetComponent<Ghost>();
            ghost.SetDissipatedByLantern(true);
            ghost.SetGhostDissipation();
        }
    }
}
