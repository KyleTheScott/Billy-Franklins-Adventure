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


    //add ground object to trigger collider that hangs lower than the player to fix hopping when dragging metal so the metal isn't dropped
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundObjectsTouching.Add(collision.gameObject);
            onGround = true;
        }
    }


    //checks if player has left the hop fix collider so the player doesn't leave ground from every hop
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
                player.GetComponent<Player>().LeavingTheGround();
            }
        }
        //player is leaving diagonal platform so this makes the player a child of the diagonal platform
        if (collision.gameObject.layer == 19)
        {
            player.SetOnDiagonalPlatform(false);
            player.transform.parent = null;
            DontDestroyOnLoad(player);
            player.SetCurrentPlatform(null);
            player.transform.localScale = new Vector3(1, 1, 1);
            
        }
    }
}
