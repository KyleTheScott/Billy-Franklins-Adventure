using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private Player playerScript;

    //collision for the player with ground
    public void OnCollisionStay2D(Collision2D collision)
    {
        //Player.PlayerState tempPlayerState = playerScript.GetPlayerState();
        if (collision.collider.CompareTag("Ground") && playerScript.GetFalling())
        {
            //Debug.Log("On ground");
            playerScript.SetOnGround(true);
            //offGround = false;
        }
    }

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
