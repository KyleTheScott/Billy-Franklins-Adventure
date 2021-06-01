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
}
