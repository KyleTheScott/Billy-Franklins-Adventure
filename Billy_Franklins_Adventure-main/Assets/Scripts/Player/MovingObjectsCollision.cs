using System.Collections.Generic;
using UnityEngine;

public class MovingObjectsCollision : MonoBehaviour
{
    private GameObject player = null;
    private bool onGround = true;
    [SerializeField] private List<GameObject> groundObjectsTouching = new List<GameObject>();
    private CheckPointSystem checkPointSystem = null;
    public List<GameObject> checkForElectric = new List<GameObject>();

    private void Awake()
    {
        player = GameObject.Find("Player");
        checkPointSystem = GameObject.Find("GlobalGameController").GetComponent<CheckPointSystem>();
    }
    private void Update()
    {
        if(checkForElectric.Count > 0)
        {
            foreach (GameObject item in checkForElectric)
            {
                //water
                if (item.CompareTag("Water") && item.GetComponent<Water>().GetElectrified()) 
                {
                    checkPointSystem.PlayerDeath();
                }
                //metal
                else if (item.CompareTag("Metal") && item.GetComponent<Metal>().GetElectrified())
                {
                    checkPointSystem.PlayerDeath();
                }
            }
        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundObjectsTouching.Add(collision.gameObject);
            onGround = true;
        }
        else if (collision.CompareTag("Water") || collision.CompareTag("Metal"))
        {
            checkForElectric.Add(collision.gameObject);
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
        else if(collision.CompareTag("Water") || collision.CompareTag("Metal"))
        {
            checkForElectric.Remove(collision.gameObject);
        }
    }
}
