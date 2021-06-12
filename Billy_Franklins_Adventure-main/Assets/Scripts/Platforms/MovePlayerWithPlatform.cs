using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovePlayerWithPlatform : MonoBehaviour
{
    [SerializeField] private SuspendedPlatform platform;
    private Player player;
    private float lastY; // last position
    private float distanceMoved; // distance platform moved during frame
    private bool playerOnPlatform =  false; // player is touching this specific platform
    private float timer = 0;
    private float timerMax = 0.5f;


    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        //timer += Time.deltaTime;
        //if (timer >= timerMax)
        //{
            if (playerOnPlatform)
            {
                distanceMoved = lastY - transform.position.y;
                if (distanceMoved > Mathf.Epsilon)
                {
                    Debug.Log("Moving");
                    player.transform.position = new Vector2(player.transform.position.x,
                        player.transform.position.y - distanceMoved);
                    //playerOnPlatform = false;
                }
            }
        //}

        lastY = transform.position.y;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = true;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = false;
        }
    }

}
