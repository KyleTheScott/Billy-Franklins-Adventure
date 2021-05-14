using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Metal : MonoBehaviour, IInteractable, IElectrifiable
{
    [SerializeField] private bool electrified = false;
    [SerializeField] private bool temporaryElectrified = false;
    [SerializeField] private Collider2D metalCollider;
    [SerializeField] private List<GameObject> connectedGameObjects = new List<GameObject>();
    [SerializeField] private GameObject player;
    private float distToPlayerXOffset;
    [SerializeField] private bool beingMoved = false;

    private Animator metalAnimator;




    public void Interact()
    {
        //metal will stop following player
        if (beingMoved)
        {
            beingMoved = false;
        }
        //metal will start following player
        else
        {
            beingMoved = true;
            distToPlayerXOffset = transform.position.x - player.transform.position.x;
        }
    }


    private void Start()
    {
        //ElectricityController.instanceElectrical.onConnectionChange.AddListener(OnConnectionChangeCallBack);
        player = GameObject.Find("Player");
        metalAnimator = gameObject.GetComponent<Animator>();
    }


    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        //if true metal will follow player
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



    //setting if metal is electrified
    public void SetElectrified(bool state)
    {
        electrified = state;
        metalAnimator.SetBool("Electrified", electrified);
    }

    public List<GameObject> GetConnectedObjects()
    {
        return connectedGameObjects;
    }


    // Start is called before the first frame update
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lightning"))
        {
            ElectricityController.instanceElectrical.ElectrifyConnectedObjects(gameObject, metalCollider);
        }
        else if (collision.CompareTag("Metal"))
        {
            bool alreadyExists = false;
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i] == collision.gameObject)
                {
                    Debug.Log("Already exists");
                    alreadyExists = true;
                }
            }
            if (!alreadyExists && collision.gameObject != gameObject)
            {
                Debug.Log("Collision Test 1");
                connectedGameObjects.Add(collision.gameObject);
            }



            bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
            ElectricityController.instanceElectrical.ConnectObjects(gameObject, metalCollider, electrified,
                collision.gameObject, collision, object2Electrified);

            Debug.Log("Metal On Metal 2");
        }
        else if (collision.CompareTag("Water"))
        {
            bool alreadyExists = false;
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i] == collision.gameObject)
                {
                    Debug.Log("Already exists");
                    alreadyExists = true;
                }
            }
            if (!alreadyExists)
            {
                Debug.Log("Collision Test 2");
                connectedGameObjects.Add(collision.gameObject);

            }
            if (collision.gameObject == gameObject)
            {
                Debug.Log("They Are The Same 2");
            }
            bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
            ElectricityController.instanceElectrical.ConnectObjects(gameObject, metalCollider, electrified,
                collision.gameObject, collision, object2Electrified);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Metal"))
        {
            Debug.Log("Disconnect before 1");
            bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
            ElectricityController.instanceElectrical.DisconnectObjects(gameObject, metalCollider, electrified,
                collision.gameObject, collision, object2Electrified);
            GameObject tempGameObject = gameObject;
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                Debug.Log("Remove: " + i + connectedGameObjects[i]);
                if (connectedGameObjects[i] == collision.gameObject)
                {
                    Debug.Log("Remove " + i + ": " + connectedGameObjects[i]);
                    connectedGameObjects.RemoveAt(i);
                }
            }
            Debug.Log("Disconnect before 2");
        }
        else if (collision.CompareTag("Water"))
        {
            bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
            ElectricityController.instanceElectrical.DisconnectObjects(gameObject, metalCollider, electrified,
                collision.gameObject, collision, object2Electrified);
            GameObject tempGameObject = collision.gameObject;
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                Debug.Log("Remove: " + i + connectedGameObjects[i]);
                if (connectedGameObjects[i] == collision.gameObject)
                {
                    Debug.Log("Remove " + i + ": " + connectedGameObjects[i]);
                    connectedGameObjects.RemoveAt(i);
                }
            }
        }

        if (connectedGameObjects.Count <= 0)
        {
            SetElectrified(false);
        }
    }
}


////if connected objects is empty then electrified will be set from state parameter
//if (connectedGameObjects.Count > 0)
//{
//    if (electrified)
//    {
//        //going through and electrifying gameObjects that are connected
//        foreach (GameObject electric in connectedGameObjects)
//        {
//            //metal
//            if (electric.gameObject.layer == 11)
//            {
//                electric.GetComponent<Metal>().SetElectrified(true);
//            }
//            //water
//            else if (electric.gameObject.layer == 4)
//            {
//                electric.GetComponent<Water>().SetElectrified(true);
//            }
//        }
//    }
//    else
//    {
//        bool stillElectrified = false;
//        //still connected to the game objects
//        for (int i = 0; i < connectedGameObjects.Count; i++)
//        {
//            //metal
//            //change this to tag because layer is now interactable
//            if (connectedGameObjects[i].layer == 11)
//            {
//                //change this to is connected to an electrified by object
//                //if this object is the electrified by object than disconnect that first
//                if (!connectedGameObjects[i].GetComponent<Metal>().IsConnectedToElectrical(gameObject))
//                {
//                    connectedGameObjects[i].GetComponent<Metal>().SetElectrified(false);
//                    connectedGameObjects.RemoveAt(i);
//                }
//                else
//                {
//                    electrifiedByObject = connectedGameObjects[i];
//                    stillElectrified = true;
//                }
//            }
//            //still electrified because it is water
//            else
//            {
//                stillElectrified = true;
//            }
//        }
//        electrified = stillElectrified;
//    }
//}


////lightning connects with metal
//if (collision.CompareTag("Lightning"))
//{
//    if (connectedToWater)
//    {
//        if (electrifiedByObject == null)
//        {
//            electrifiedByObject = collision.gameObject;
//        }

//        SetElectrified(true);
//    }
//}

//else if (collision.CompareTag("Metal"))
//{
//    if (collision.gameObject.GetComponent<Metal>().GetElectrified())
//    {
//        connectedToElectrifiedMetal = true;
//        if (electrifiedByObject == null)
//        {
//            electrifiedByObject = collision.gameObject;
//        }
//        SetElectrified(true);
//    }
//    else
//    {
//        connectedGameObjects.Add(collision.gameObject);
//    }
//}
//else if (collision.CompareTag("Water"))
//{
//    if (collision.gameObject.GetComponent<Water>().GetElectrified())
//    {
//        SetElectrified(true);
//    }
//    else
//    {
//        connectedToWater = true;
//        connectedGameObjects.Add(collision.gameObject);
//    }
//}