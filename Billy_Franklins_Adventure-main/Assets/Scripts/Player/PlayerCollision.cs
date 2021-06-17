using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private Player playerScript;
    //[SerializeField] private bool hitHead = false; // keeps track if the player hit head
    //private bool offGround;
    //private float offGroundTimer = 0;
    //private float offGroundTimerMax = 0.5f;


    //void Update()
    //{
    //    if (offGround)
    //    {
    //        Debug.Log("off ground 2");
    //        offGroundTimer += Time.deltaTime;
    //        if (offGroundTimer >= offGroundTimerMax)
    //        {
    //            Debug.Log("Fix Ground");
    //            playerScript.GetComponent<Player>().LeavingTheGround();
    //        }
    //    }
    //    else
    //    {
    //        offGround = false;
    //        offGroundTimer = 0;
    //    }
    //}

    //public void SetHitHead(bool state)
    //{
    //    hitHead = state;
    //}

    //collision for the player with ground
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Player.PlayerState tempPlayerState = playerScript.GetPlayerState();
        if (collision.collider.CompareTag("Ground") && playerScript.GetFalling())
        {
            //Debug.Log("On ground");
            playerScript.SetOnGround(true);
            //offGround = false;
        }
    }

    //public void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Ground") && playerScript.GetFalling())
    //    {
    //        Debug.Log("off ground 4");
    //        playerScript.SetOnGround(true);
    //        offGround = false;
    //    }
    //}


    //public void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Ground"))
    //    {
    //        if ((playerScript.GetPlayerState() == Player.PlayerState.IDLE ||
    //            playerScript.GetPlayerState() == Player.PlayerState.WALKING) &&
    //            playerScript.GetAimLineState() == Player.AimLineState.NOT_AIMED)
    //        {
    //            Debug.Log("off ground");
    //            offGround = true;
    //            playerScript.SetPlayerState(Player.PlayerState.WALK_FALL);
    //            playerScript.SetKinematic(false);
    //        }
    //    }
    //}

    public void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Metal Exit: " + collision.gameObject);
        if (collision.CompareTag("Metal"))
        {
            if (collision.gameObject == playerScript.GetCurrentMovingObject())
            {
                playerScript.SetObjectDisconnected();
            }
        }
       
    }
}
