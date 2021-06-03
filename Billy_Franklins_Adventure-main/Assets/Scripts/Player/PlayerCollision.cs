using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private Player playerScript;

    //collision for the player with ground
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") && playerScript.GetFalling())
        {
            playerScript.SetOnGround(true);
        }
    }

    //public void OnCollisionExit2D(Collision2D collision)
    //{
    //    Debug.Log("Metal Exit: " + collision.gameObject);
    //    if (collision.collider.CompareTag("Metal"))
    //    {
    //        playerScript.SetObjectDisconnected();
    //    }
    //}
    public void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Metal Exit: " + collision.gameObject);
        if (collision.CompareTag("Metal"))
        {
            if (collision.gameObject == playerScript.GetCurrentMovingObject())
            {
                playerScript.SetObjectDisconnected();
            }
        }
    }
}
