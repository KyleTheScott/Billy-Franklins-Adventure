using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Metal : MonoBehaviour, IInteractable, IElectrifiable
{
    [Header("Electrical")]
    [SerializeField] private bool electrified = false;
    [SerializeField] private bool temporaryElectrified = false;
    [SerializeField] private List<GameObject> connectedGameObjects = new List<GameObject>();
    [Header("General")]
    [SerializeField] private Collider2D metalCollider;
    private Animator metalAnimator;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject highlight;
    [Header("Movement")]
    private float distToPlayerXOffset;
    [SerializeField] private bool beingMoved = false;

    



    //when e is pressed and player is within range of the Interactable
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

    public void SetHighlighted(bool state)
    {
        highlight.SetActive(state);
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

    //moving metal with player
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

    //returns objects directly connected
    public List<GameObject> GetConnectedObjects()
    {
        return connectedGameObjects;
    }

    public void ElectrifyConnectedObjects()
    {
        foreach (GameObject obj in connectedGameObjects)
        {
            obj.GetComponent<IElectrifiable>().SetElectrified(true);
        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        //electrifies object
        if (collision.CompareTag("Lightning"))
        {
            ElectricityController.instanceElectrical.ElectrifyConnectedObjects(gameObject, metalCollider);
        }
        //connects objects
        else if (collision.CompareTag("Metal"))
        {
            bool alreadyExists = false;
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i] == collision.gameObject)
                {
                    alreadyExists = true;
                }
            }
            if (!alreadyExists && collision.gameObject != gameObject)
            {
                connectedGameObjects.Add(collision.gameObject);
            }

            bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
            ElectricityController.instanceElectrical.ConnectObjects(gameObject, metalCollider, electrified,
                collision.gameObject, collision, object2Electrified);
        }
        else if (collision.CompareTag("Water"))
        {
            bool alreadyExists = false;
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i] == collision.gameObject)
                {
                    alreadyExists = true;
                }
            }
            if (!alreadyExists)
            {
                connectedGameObjects.Add(collision.gameObject);

            }
            bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
            ElectricityController.instanceElectrical.ConnectObjects(gameObject, metalCollider, electrified,
                collision.gameObject, collision, object2Electrified);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //disconnect objects
        if (collision.CompareTag("Metal"))
        {
            bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
            ElectricityController.instanceElectrical.DisconnectObjects(gameObject, metalCollider, electrified,
                collision.gameObject, collision, object2Electrified);
            GameObject tempGameObject = gameObject;
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i] == collision.gameObject)
                {
                    connectedGameObjects.RemoveAt(i);
                }
            }
        }
        else if (collision.CompareTag("Water"))
        {
            bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
            ElectricityController.instanceElectrical.DisconnectObjects(gameObject, metalCollider, electrified,
                collision.gameObject, collision, object2Electrified);
            GameObject tempGameObject = collision.gameObject;
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i] == collision.gameObject)
                {
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