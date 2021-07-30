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
        if (collision.collider.CompareTag("Ground") && playerScript.GetFalling() && Vector2.Angle(Vector2.up, collision.GetContact(0).normal) <= 45f)
        {
            Debug.Log("On ground Test");
            playerScript.SetOnGround(true);
        }
        else if (collision.gameObject.name == "Scaffolding" && playerScript.GetPlayerState() == Player.PlayerState.JUMPING &&
                 Vector2.Angle(Vector2.up, collision.GetContact(0).normal) <= 45f && Mathf.Abs(collision.GetContact(0).point.y - (playerScript.gameObject.transform.position.y - 1.23f)) <= .5f &&
                 !playerScript.GetJumpFix())
        {
            playerScript.SetOnGroundJumpFix(true);
            playerScript.SetPlayerState(Player.PlayerState.IDLE);
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
