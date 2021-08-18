using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private Player playerScript;
    private float fallTimer = 0;
    private float fallTime = 0.5f;
    [SerializeField] private bool fallingTimeDone = true;




    public void SetFallWait()
    {
        fallTimer = 0;
        fallingTimeDone = false;
    }

    void Update()
    {
        if (!fallingTimeDone)
        {
            if (fallTimer >= fallTime)
            {
                fallingTimeDone = true;
            }
            else
            {
                fallTimer += Time.deltaTime;
            }
        }
    }

    //collision for the player with ground
    public void OnCollisionStay2D(Collision2D collision)
    {
        if (Vector2.Angle(Vector2.up, collision.GetContact(0).normal) <= 45f)
        {
           
            if (collision.collider.CompareTag("Ground") && playerScript.GetFalling())
            {
                //Debug.Log("On ground Test");
                playerScript.SetOnGround(true);
               
            }
            else if (collision.collider.CompareTag("Ground") && collision.collider.gameObject.layer == 19 && playerScript.GetCurrentMovingObject() == null)
            {
                if (collision.collider.gameObject.layer == 19)
                {
                    //Debug.LogError("Player Diagonal");
                    playerScript.SetOnDiagonalPlatform(true);
                    playerScript.transform.parent = collision.gameObject.transform.parent;
                    //if (playerScript.GetCurrentMovingObject() != null)
                    //{
                    //    playerScript.GetCurrentMovingObject().transform.parent = collision.gameObject.transform.parent;
                    //}
                    playerScript.SetCurrentPlatform(collision.gameObject);
                }
            }
            else if (collision.gameObject.name == "Scaffolding" && playerScript.GetPlayerState() == Player.PlayerState.JUMPING 
                    && Mathf.Abs(collision.GetContact(0).point.y - (playerScript.gameObject.transform.position.y - 1.23f)) <= .5f && !playerScript.GetJumpFix())
            {
                playerScript.SetOnGroundJumpFix(true);
                playerScript.SetPlayerState(Player.PlayerState.IDLE);
                if (collision.gameObject.layer == 19)
                {
                    playerScript.transform.parent = null;
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Metal"))
        {
            if (collision.gameObject == playerScript.GetCurrentMovingObject())
            {
                playerScript.SetObjectDisconnected();
            }
        }
    }
}
