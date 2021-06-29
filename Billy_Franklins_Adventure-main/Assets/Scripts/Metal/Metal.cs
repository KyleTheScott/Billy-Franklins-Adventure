using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Metal : MonoBehaviour, IInteractable, IElectrifiable
{
    [Header("Electrical")] [SerializeField]
    private bool electrified = false;

    [SerializeField] private bool temporaryElectrified = false;
    [SerializeField] private List<GameObject> connectedGameObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> connectedDoors = new List<GameObject>();


    [Header("General")] 
    [SerializeField] private Collider2D metalCollider;
    private Animator metalAnimator;
    //[SerializeField] private GameObject player;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject highlight;
    [Header("Movement")] private float distToPlayerXOffset;
    [SerializeField] private bool beingMoved = false;

    [SerializeField] private int groupNum = 0;

    [SerializeField] private bool movable = true;





    //when e is pressed and player is within range of the Interactable
    public void Interact()
    {
        if (movable)
        {
            //metal will stop following player
            if (beingMoved)
            {
                beingMoved = false;
            }
            //metal will start following player
            else
            {
                if (connectedDoors.Count > 0)
                {
                    player.GetComponent<Player>().SetMoveObjectStopped();
                }

                beingMoved = true;
                distToPlayerXOffset = transform.position.x - player.transform.position.x;
            }
        }
    }
    //makes highlight appear and disappear
    public void SetHighlighted(bool state)
    {
        highlight.SetActive(state);
    }



    private void Start()
    {
        player = GameObject.FindObjectOfType<Player>().gameObject;
        metalAnimator = gameObject.GetComponent<Animator>();
    }


    private void Update()
    {
        if (movable)
        {
            Movement();
        }
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

    // for number of group the metal is in ElectricityController connectedObjects
    public int GetGroupNum()
    {
        return groupNum;
    }
    public void SetGroupNum(int num)
    {
        groupNum = num;
    }

    //get if metal is electrified
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

    public bool IsMoving()
    {
        return beingMoved;
    }

    public void SetMoving(bool state)
    {
        beingMoved = state;
    }



    //returns objects directly connected
    public List<GameObject> GetConnectedObjects()
    {
        return connectedGameObjects;
    }
    // electrifies connected objects
    public void ElectrifyConnectedObjects()
    {
        foreach (GameObject obj in connectedGameObjects)
        {
            obj.GetComponent<IElectrifiable>().SetElectrified(true);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lightning"))
        {
            ElectricityController.instanceElectrical.ElectrifyConnectedObjects(
                gameObject, metalCollider, electrified, groupNum);
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
            int object2GroupNum = collision.gameObject.GetComponent<Metal>().GetGroupNum();
            ElectricityController.instanceElectrical.ConnectObjects(
                gameObject, metalCollider, electrified, groupNum,
                collision.gameObject, collision, object2Electrified, object2GroupNum);
        }
        else if (collision.CompareTag("Water") && collision.gameObject.GetComponent<Water>().GetWaterByItself())
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
            int object2GroupNum = collision.gameObject.GetComponent<Water>().GetGroupNum();
            ElectricityController.instanceElectrical.ConnectObjects(
                gameObject, metalCollider, electrified, groupNum,
                collision.gameObject, collision, object2Electrified, object2GroupNum);
        }
        else if (collision.CompareTag("Door") && beingMoved)
        {
            connectedDoors.Add(collision.gameObject);
            player.GetComponent<Player>().SetMoveObjectStopped();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //disconnect objects
        if (collision.CompareTag("Metal"))
        {
            bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
            int object2GroupNum = collision.gameObject.GetComponent<Metal>().GetGroupNum();
            ElectricityController.instanceElectrical.DisconnectObjects(
                gameObject, metalCollider, electrified, groupNum, 
                collision.gameObject, collision, object2Electrified, object2GroupNum );
            GameObject tempGameObject = gameObject;
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i] == collision.gameObject)
                {
                    connectedGameObjects.RemoveAt(i);
                }
            }
        }
        else if (collision.CompareTag("Water") && collision.gameObject.GetComponent<Water>().GetWaterByItself())
        {
            bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
            int object2GroupNum = collision.gameObject.GetComponent<Water>().GetGroupNum();
            ElectricityController.instanceElectrical.DisconnectObjects(
                gameObject, metalCollider, electrified, groupNum,
                collision.gameObject, collision, object2Electrified, object2GroupNum);
            GameObject tempGameObject = collision.gameObject;
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i] == collision.gameObject)
                {
                    connectedGameObjects.RemoveAt(i);
                }
            }
        }
        else if (collision.CompareTag("Door"))
        {
            GameObject tempGameObject = collision.gameObject;
            for (int i = 0; i < connectedDoors.Count; i++)
            {
                if (connectedDoors[i] == collision.gameObject)
                {
                    connectedDoors.RemoveAt(i);
                }
            }
            if (connectedDoors.Count <= 0)
            {
                player.GetComponent<Player>().SetMoveObject();
            }
        }



        if (connectedGameObjects.Count <= 0)
        {
            SetElectrified(false);
        }
    }
}

