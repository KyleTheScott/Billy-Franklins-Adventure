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
        //if (fallingTimeDone)
        //{
        //Player.PlayerState tempPlayerState = playerScript.GetPlayerState();
        if (collision.collider.CompareTag("Ground") && playerScript.GetFalling() && Vector2.Angle(Vector2.up, collision.GetContact(0).normal) <= 45f)
        {
            Debug.Log("On ground Test");
            playerScript.SetOnGround(true);
            //offGround = false;
        }
        //else if (collision.collider.CompareTag("Ground") && fallingTimeDone && Vector2.Angle(Vector2.up, collision.GetContact(0).normal) <= 45f)
        //{
        //    Debug.Log("On ground Test2");
        //    playerScript.SetOnGround(true);
        //}
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
