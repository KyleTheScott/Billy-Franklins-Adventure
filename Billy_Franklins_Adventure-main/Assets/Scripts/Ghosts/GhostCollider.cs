using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCollider : MonoBehaviour
{
    //as a ghost enters collider that is the area where the player light dissipates ghosts the ghost that enters is dissipated 
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ghost"))
        {
            collision.GetComponent<Ghost>().SetGhostDissipation();
        }
    }

    //as a ghost exits collider that is the area where the player light dissipates ghosts the ghost that exits appears again 
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ghost"))
        {
            Ghost ghost = collision.GetComponent<Ghost>();
            if (!ghost.GetDissipatedByLantern())
            {
                ghost.SetGhostAppearing();
            }
        }
    }
}
