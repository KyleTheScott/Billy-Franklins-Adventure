using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCollider : MonoBehaviour
{
    //[SerializeField] private List<GameObject> ghosts = new List<GameObject>();

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ghost"))
        {
            collision.GetComponent<Ghost>().SetGhostDissipation();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ghost"))
        {
            Ghost ghost = collision.GetComponent<Ghost>();
            if (!ghost.GetDissipatedByLantern())
            {
                ghost.SetGhostAppearing();
            }
            Debug.Log("Ghost Appear start");
        }
    }
}
