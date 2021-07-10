using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Events;

public class Lamp : MonoBehaviour, IInteractable
{
    SpriteRenderer spriteRenderer = null; //spriteRenderer of this lantern
    Light2D light2D = null;
    BoxCollider2D boxCollider = null;

    [SerializeField] Sprite lanternOnSprite = null;
    [SerializeField] Sprite lanternOffSprite = null;
    [SerializeField] private GameObject highlight;
    [SerializeField] GhostWallController ghostWall;
    [SerializeField] private Player player;

    [SerializeField] private List<GameObject> ghosts = new List<GameObject>();
    Charges charges ;
    [HideInInspector]
    public UnityEvent onLampOn; //Invoke when lamp is on, darkborder will subscribe this

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        light2D = GetComponentInChildren<Light2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        //Turn off cllider first, it will be available once all lantern on
        boxCollider.enabled = false;

        player = FindObjectOfType<Player>().GetComponent<Player>();
        charges = FindObjectOfType<Charges>();
        //Subscribe this event
        GlobalGameController.instance.onAllLanternOn.AddListener(OnAllLanternOnCallback);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public void Interact()
    {
        charges.SetLampOn(true);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If lantern collided with lightning...
        if (collision.CompareTag("Lightning"))
        {
            

            //Turn off collider
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }

            if(onLampOn != null)
            {
                
                Debug.Log("Lamp on");
                onLampOn.Invoke();
                
            }
            charges.SetLampOn(true);
            LanternToggle();
        }
    }

    private void LanternToggle()
    {
        //Change lantern's sprite
        if (spriteRenderer != null && lanternOnSprite != null && lanternOffSprite != null)
        {
            ghostWall.RaiseGhostWall();
            DissipateAllGhosts();
            if (spriteRenderer.sprite == lanternOnSprite)
            {
                spriteRenderer.sprite = lanternOffSprite;
                if (light2D != null)
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

    void OnAllLanternOnCallback()
    {
        //Turn on collider
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }
    }


    private void DissipateAllGhosts()
    {
        for (int i = 0; i < ghosts.Count; i++)
        {
            Ghost ghost = ghosts[i].GetComponent<Ghost>();
            ghost.SetDissipatedByLantern(true);
            ghost.SetGhostDissipation();
        }
    }

}
