using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject waterObject;
    private Animator bucketAnimator;
    private bool tippedOver;
    [SerializeField] private GameObject highlight;

    // Start is called before the first frame update
    void Start()
    {
        bucketAnimator = gameObject.GetComponent<Animator>();
        player = FindObjectOfType<Player>();
    }

    //called when you the bucket is within the interactable circle
    //tips over the bucket and spills the water if it isn't already spilt
    public void Interact()
    {
        if (!tippedOver)
        {
           
            tippedOver = true;
            bool facingRight = true;
            if (player.transform.position.x >= transform.position.x)
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                facingRight = false;
                bucketAnimator.SetBool("FacingRight", false);
            }
            else
            {
                bucketAnimator.SetBool("FacingRight", true);
            }

            

            bucketAnimator.SetBool("BucketTipped", true);

           // Debug.LogError("Water collider state before: " + waterObject.GetComponent<Collider2D>().enabled);
            //waterObject.GetComponent<Collider2D>().enabled = true;
            waterObject.GetComponent<Water>().SetWaterByItself(true);
           // Debug.LogError("Water collider state after: " + waterObject.GetComponent<Collider2D>().enabled);
            waterObject.GetComponent<Water>().SpillWater(facingRight);
        }
    }

    public void SetInKickingRange()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) > .5f)
        {
            player.SetAnimationMovement(true);
        }
        //Debug.Log("Dist B2: " + Mathf.Abs(player.transform.position.x - transform.position.x));
        if (player.GetAnimationMovement())
        {
            if (player.transform.position.x >= transform.position.x)
            {
                player.SetMovingRight(false);
            }
            else
            {
                player.SetMovingRight(true);
            }
        }
    }


    public void SetHighlighted(bool state)
    {
        highlight.SetActive(state);
    }
}
