using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : MonoBehaviour, IInteractable
{
    [SerializeField] private bool electrified = false;
    [SerializeField] private int numOfElectricConnections = 0;
    [SerializeField] private bool connectedToWater = false;
    [SerializeField] private GameObject electrifiedByObject = null;
    [SerializeField] private bool connectedToElectrifiedMetal = false;

    [SerializeField] List<GameObject> connectedGameObjects = new List<GameObject>();

    [SerializeField] private GameObject player;
    private float distToPlayerXOffset;
    [SerializeField] private bool beingMoved = false;


    public void Interact()
    {

        if (beingMoved)
        {
            beingMoved = false;
        }
        else
        {
            beingMoved = true;
            distToPlayerXOffset = transform.position.x - player.transform.position.x;
        }
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (beingMoved)
        {
            float moveWithPlayerX = player.transform.position.x + distToPlayerXOffset;
            transform.position = new Vector2(moveWithPlayerX, transform.position.y);
        }
    } 

    public bool GetElectrified()
    {
        return electrified;
    }

    public bool GetConnectedToWater()
    {
        return connectedToWater;
    }
    public bool GetConnectedToElectrifiedMetal()
    {
        return connectedToElectrifiedMetal;
    }

    public GameObject GetElectricifiedByObject()
    {
        return electrifiedByObject;
    }

    public bool IsConnectedToElectrical(GameObject currObject)
    {
        foreach (GameObject electric in connectedGameObjects)
        {
            if (currObject.GetComponent<Metal>().electrifiedByObject = electric)
            {
                return true;
            }
        }
        return true;
    }

    public void SetElectrified(bool state)
    {
        electrified = state;
        if (connectedGameObjects.Count > 0)
        {
            if (electrified)
            {
                //going through and electrifying gameObjects that are connected
                foreach (GameObject electric in connectedGameObjects)
                {
                    //metal
                    if (electric.gameObject.layer == 11)
                    {
                        electric.GetComponent<Metal>().SetElectrified(true);
                    }
                    //water
                    else if (electric.gameObject.layer == 4)
                    {
                        electric.GetComponent<Water>().SetElectrified(true);
                    }
                }
            }
            else
            {

                bool stillElectrified = false;
                //still connected to the game objects
                for (int i = 0; i < connectedGameObjects.Count; i++)
                {
                    //metal
                    if (connectedGameObjects[i].layer == 11)
                    {
                        if (!connectedGameObjects[i].GetComponent<Metal>().IsConnectedToElectrical(gameObject))
                        {
                            connectedGameObjects[i].GetComponent<Metal>().SetElectrified(false);
                            connectedGameObjects.RemoveAt(i);
                        }
                        else
                        {
                            electrifiedByObject = connectedGameObjects[i];
                            stillElectrified = true;
                        }
                    }
                    else
                    {
                        stillElectrified = true;
                    }
                }
                electrified = stillElectrified;
            }
        }
    }

    // Start is called before the first frame update
    public void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("Lightning"))
        {
            if (connectedToWater)
            {
                if (electrifiedByObject == null)
                {
                    electrifiedByObject = collision.gameObject;
                }
                SetElectrified(true);
            }
        }
        else if (collision.CompareTag("Metal"))
        {
            if (collision.gameObject.GetComponent<Metal>().GetElectrified())
            {
                connectedToElectrifiedMetal = true;
                if (electrifiedByObject == null)
                {
                    electrifiedByObject = collision.gameObject;
                }
                SetElectrified(true);
            }
            else
            {
                connectedGameObjects.Add(collision.gameObject);
            }
        }
        else if (collision.CompareTag("Water"))
        {
            if (collision.gameObject.GetComponent<Water>().GetElectrified())
            {
                SetElectrified(true);
            }
            else
            {
                connectedToWater = true;
                connectedGameObjects.Add(collision.gameObject);
            }
        }
    } 
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Metal"))
        {
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i] == collision.gameObject)
                {
                    if (connectedGameObjects[i] == electrifiedByObject)
                    {
                        electrifiedByObject = null;
                    }
                    connectedGameObjects.RemoveAt(i);
                }
            }
            if (electrified)
            {
                SetElectrified(false);
            }
            
               
            
        }
        else if (collision.CompareTag("Water"))
        {

        }
    }
} 
