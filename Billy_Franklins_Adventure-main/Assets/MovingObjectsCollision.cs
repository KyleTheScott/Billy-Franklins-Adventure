using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjectsCollision : MonoBehaviour
{
    private GameObject player = null;
    private bool onGround = true;
    [SerializeField] private List<GameObject> groundObjectsTouching = new List<GameObject>();
    private CheckPointDeathSystem checkPointDeathSystem = null;

    private void Awake()
    {
        player = GameObject.Find("Player");
        checkPointDeathSystem = GameObject.Find("GlobalGameController").GetComponent<CheckPointDeathSystem>();
    }
    
    public bool IsGrounded()
    {
        return onGround;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundObjectsTouching.Add(collision.gameObject);
            onGround = true;
        }
        //if player touches electrified water then indicate player death
        if (collision.CompareTag("Water") && collision.GetComponent<Water>().GetElectrified())
        {
            checkPointDeathSystem.PlayerDeath();
        }
        //if player touches electrified metal then indicate player death
        if (collision.CompareTag("Metal") && collision.GetComponent<Metal>().GetElectrified())
        {
            checkPointDeathSystem.PlayerDeath();
        }
        //if player hits a checkpoint then set checkpoint
        if (collision.CompareTag("Checkpoint"))
        {
            checkPointDeathSystem.SetCheckpoint();
        }
        
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            onGround = false;
            for (int i = 0; i < groundObjectsTouching.Count; i++)
            {
                if (groundObjectsTouching[i] == collision.gameObject)
                {
                    groundObjectsTouching.RemoveAt(i);
                }
            }

            if (groundObjectsTouching.Count <= 0)
            {
                player.GetComponent<Player>().LeavingTheGround();
            }
        }
    }
}
