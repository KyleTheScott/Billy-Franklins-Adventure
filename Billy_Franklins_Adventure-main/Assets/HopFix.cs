using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopFix : MonoBehaviour
{
    [SerializeField] private List<GameObject> groundObjectsTouching = new List<GameObject>();
    private bool onGround = true;
    private Player player = null;

    private void Start()
    {
        player = player = FindObjectOfType<Player>();
    }



    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundObjectsTouching.Add(collision.gameObject);
            onGround = true;
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
            if (groundObjectsTouching.Count <= 0 )
            {
                Debug.Log("Faling will begin");
                player.GetComponent<Player>().LeavingTheGround();
            }
        }
    }
}
