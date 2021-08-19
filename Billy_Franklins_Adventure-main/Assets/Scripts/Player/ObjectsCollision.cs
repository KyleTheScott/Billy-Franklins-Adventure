using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsCollision : MonoBehaviour
{
    private GameObject player = null;

    
    public List<GameObject> checkForElectric = new List<GameObject>();

    private void Start()
    {
        player = player = FindObjectOfType<Player>().gameObject;
    }

    private void Update()
    {
        if (checkForElectric.Count > 0)
        {
            //checks if player is being electrocuted
            foreach (GameObject item in checkForElectric)
            {
                //water
                if (item.CompareTag("Water") && item.GetComponent<Water>().GetElectrified())
                {
                    //Debug.LogError("Death Working Water");
                    EmptyObjects();
                    player.GetComponent<Player>().SetPlayerState(Player.PlayerState.PLAYER_DEATH_ELECTRIFIED_START);
                    break;
                }
                //metal
                if (item.CompareTag("Metal") && item.GetComponent<Metal>().GetElectrified())
                {
                    if (item.GetComponent<Metal>().GetMovable())
                    {
                        //Debug.LogError("Death Working Metal");
                        EmptyObjects();
                        player.GetComponent<Player>().SetPlayerState(Player.PlayerState.PLAYER_DEATH_ELECTRIFIED_START);
                        break;
                    }
                }
            }
        }
    }

    public void EmptyObjects()
    {
        checkForElectric.Clear();
        FindObjectOfType<PlayerObjectInteractions>().EmptyObjects();
        FindObjectOfType<ElectricityController>().EmptyObjects();
        player.GetComponent<Player>().SetPlayerState(Player.PlayerState.IDLE);
    }

    //add electrifiable object connected to the player
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water") || collision.CompareTag("Metal"))
        {
            checkForElectric.Add(collision.gameObject);
        }
    }
    //remove electrifiable object connected to the player
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water") || collision.CompareTag("Metal"))
        {
            checkForElectric.Remove(collision.gameObject);
        }
    }
}
