using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Bucket : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject waterObject;
    private Animator bucketAnimator;
    private bool tippedOver;
    

    public void Interact()
    {
        Debug.Log("Tipped");
        //change bucket here
        if (!tippedOver)
        {
            tippedOver = true;
            bool facingRight = true;
            if (player.transform.position.x >= transform.position.x)
            {
                transform.localScale = new Vector2(-transform.lossyScale.x, transform.lossyScale.y);
                facingRight = false;
                bucketAnimator.SetBool("FacingRight", false);
            }
            else
            {
                bucketAnimator.SetBool("FacingRight", true);
            }
            bucketAnimator.SetBool("BucketTipped", true);
            waterObject.GetComponent<Water>().SpillWater(facingRight);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        bucketAnimator = gameObject.GetComponent<Animator>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
