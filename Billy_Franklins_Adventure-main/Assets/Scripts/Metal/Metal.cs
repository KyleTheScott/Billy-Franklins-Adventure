using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [SerializeField] private Player player;
    private PlayerGFX playerGFX;
    [SerializeField] private GameObject movingMetal;
    [SerializeField] private GameObject highlight;
    [Header("Movement")] private float distToPlayerXOffset;
    [SerializeField] private bool beingMoved = false;

    [SerializeField] private int groupNum = 0;

    [SerializeField] private bool movable = true;

    [SerializeField] private GameObject pickUpPointLeft;
    [SerializeField] private GameObject pickUpPointRight;

    [SerializeField] private HingeJoint2D hingeJointPointLeft;
    [SerializeField] private HingeJoint2D hingeJointPointRight;

    private float lastPositionLeft;
    private float lastPositionRight;

    //[SerializeField] private float rotationSpeed = 1f ;

    //private float rotation = 0;
    //private bool metalIsRotating;



    private void Start()
    {
        player = FindObjectOfType<Player>();
        playerGFX = FindObjectOfType<PlayerGFX>();
        //pickUpPointLeft = FindObjectOfType<PickUpPointLeft>().gameObject;
        //pickUpPointRight = FindObjectOfType<GameObject>().gameObject;
        movingMetal = FindObjectOfType<MovingMetal>().gameObject;
        metalAnimator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (movable)
        {
            Movement();
        }

        //if (metalIsRotating)
        //{
        //    if (playerGFX.GetFacingRight())
        //    {

        //    }
        //}
    }

    public GameObject GetPickUpPointLeft()
    {
        return pickUpPointLeft;
    }
    public GameObject GetPickUpPointRight()
    {
        return pickUpPointRight;
    }




    //when e is pressed and player is within range of the Interactable
    public void Interact()
    {
        if (movable)
        {
            //metal will stop following player
            if (beingMoved)
            {
                PlayerObjectInteractions.playerObjectIInstance.SetInteracting(false);
                beingMoved = false;
                //player.SetPlayerState(Player.PlayerState.MOVING_OBJECT_END);
            }
            //metal will start following player
            else
            {
                PlayerObjectInteractions.playerObjectIInstance.SetInteracting(true);
                beingMoved = true;
                if (connectedDoors.Count > 0)
                {
                    player.GetComponent<Player>().SetMoveObjectStopped();
                }
            }
        }
    }
    //makes highlight appear and disappear
    public void SetHighlighted(bool state)
    {
        highlight.SetActive(state);
    }

    public void SetPickUpMetalDirection()
    {
        //Debug.Log("Pick up metal");
        bool leftOnGround = pickUpPointLeft.GetComponent<MetalEndObjectInteraction>().GetTouchingGround();
        bool rightOnGround = pickUpPointLeft.GetComponent<MetalEndObjectInteraction>().GetTouchingGround();

        //bothe sides of the metal are on the ground
        if (leftOnGround && rightOnGround)
        {
            float distToLeft = Mathf.Abs(player.transform.position.x - pickUpPointLeft.transform.position.x);
            float distToRight = Mathf.Abs(player.transform.position.x - pickUpPointRight.transform.position.x);

            player.SetAnimationMovement(true);

            if (player.GetAnimationMovement())
            {
                //going to the left side of the metal
                if (distToLeft <= distToRight)
                {
                    //set going to left side of the metal
                    playerGFX.SetGoingToRightSideOfMetal(false);
                    //player is to the right of the left side of the metal
                    if (player.transform.position.x - pickUpPointLeft.transform.position.x >= 0)
                    {
                        //Debug.Log("Move to metal left");
                        player.SetMovingRight(false);
                    }
                    //player is to the left of the left side of the metal
                    else
                    {
                        //Debug.Log("Move to metal right");
                        player.SetMovingRight(true);
                    }
                }
                //going to the right side of the metal
                else
                {
                    //set going to right side of the metal
                    playerGFX.SetGoingToRightSideOfMetal(true);

                    //player is to the right of the right side of the metal
                    if (player.transform.position.x - pickUpPointRight.transform.position.x >= 0)
                    {
                        //Debug.Log("Move to metal left");
                        player.SetMovingRight(false);
                    }
                    //player is to the left of the right side of the metal
                    else
                    {
                        //Debug.Log("Move to metal right");
                        player.SetMovingRight(true);
                    }
                }
            }

        }
        else if (leftOnGround)
        {
            playerGFX.SetGoingToRightSideOfMetal(false);
            float distToLeft = Mathf.Abs(player.transform.position.x - pickUpPointLeft.transform.position.x);
            //if (distToLeft > .1f)
            //{
            player.SetAnimationMovement(true);
            //}

            if (player.GetAnimationMovement())
            {
                //player is to the right of the left side of the metal
                if (player.transform.position.x - pickUpPointLeft.transform.position.x >= 0)
                {
                    Debug.Log("Move to metal left");
                    player.SetMovingRight(false);
                }
                //player is to the left of the left side of the metal
                else
                {
                    Debug.Log("Move to metal right");
                    player.SetMovingRight(true);
                }
            }
        }
        else if (rightOnGround)
        {
            playerGFX.SetGoingToRightSideOfMetal(true);
            float distToRight = Mathf.Abs(player.transform.position.x - pickUpPointRight.transform.position.x);
            //if (distToRight > .1f)
            //{
            player.SetAnimationMovement(true);
            //}

            if (player.GetAnimationMovement())
            {
                //player is to the right of the right side of the metal
                if (player.transform.position.x - pickUpPointRight.transform.position.x >= 0)
                {
                    Debug.Log("Move to metal left");
                    player.SetMovingRight(false);
                }
                //player is to the left of the right side of the metal
                else
                {
                    Debug.Log("Move to metal right");
                    player.SetMovingRight(true);
                }
            }
        }
        else
        {
            Debug.Log("Metal not on ground");
        }
    }


    public void ConnectMetalToPlayer()
    {
        if (playerGFX.GetGoingToRightSideOfMetal())
        {
            //Debug.Log("Right set parent");
            //distToPlayerXOffset = PlayerObjectInteractions.playerObjectIInstance.GetMetalRightPos().transform.position.x - player.transform.position.x;
            //distToPlayerXOffset = transform.position.x - player.transform.position.x; 
            //gameObject.transform.SetParent(pickUpPointRight.transform);
            //pickUpPointRight.transform.position = MovingMetal.movingMetalInstance.transform.position;
            hingeJointPointRight.enabled = true; 
            hingeJointPointRight.connectedBody = MovingMetal.movingMetalInstance.GetMovingMetalRigidbody();
            transform.position = new Vector2(transform.position.x, transform.position.y + .4f);
            transform.Rotate(0f, 0f, 40);
            transform.Rotate(0f, 0f, 0);

            //StartCoroutine("RotateToMoveRight");
        }
        else
        {
            //Debug.Log("Left set parent");
            //distToPlayerXOffset = transform.position.x - player.transform.position.x;
            //distToPlayerXOffset = PlayerObjectInteractions.playerObjectIInstance.GetMetalLeftPos().transform.position.x - player.transform.position.x;
            //gameObject.transform.SetParent(pickUpPointLeft.transform);
            //pickUpPointLeft.transform.position = MovingMetal.movingMetalInstance.transform.position;

            //MovingMetal.movingMetalInstance.SetFixedJoint(pickUpPointLeft.GetComponent<Rigidbody2D>());
            hingeJointPointLeft.enabled = true;
            hingeJointPointLeft.connectedBody = MovingMetal.movingMetalInstance.GetMovingMetalRigidbody();
            transform.position = new Vector2(transform.position.x, transform.position.y + .4f);
            transform.Rotate(0f, 0f, -40);
            transform.Rotate(0f, 0f, 0);
        }
        player.SetKinematic(true);
        beingMoved = true;
    }

    public void DisconnectMetalFromPlayer()
    {
        PlayerObjectInteractions.playerObjectIInstance.SetInteracting(false);
        Debug.Log("Disconnect Metal");
        beingMoved = false;
        //hingeJointPointLeft.connectedBody = null;
        hingeJointPointLeft.enabled = false;
        hingeJointPointRight.enabled = false;
        transform.Rotate(0f, 0f, 0);
    }

    //public void DisconnectMetalFromPlayerSwitch()
    //{
    //    Debug.Log("Disconnect");
    //    if (playerGFX.GetFacingRight())
    //    {

    //        //hingeJointPointRight.connectedBody = null;
    //        hingeJointPointRight.enabled = false;
    //        hingeJointPointLeft.enabled = false;
    //        transform.Rotate(0f, 0f, 0);
    //    }
    //    else
    //    {
    //        //hingeJointPointLeft.connectedBody = null;
    //        hingeJointPointLeft.enabled = false;
    //        hingeJointPointRight.enabled = false;
    //        transform.Rotate(0f, 0f, 0);
    //    }
    //}


    //moving metal with player
    private void Movement()
    {
        //if true metal will follow player
        //if (beingMoved)
        //{
        //    switch (player.GetPlayerState())
        //    {
        //        case Player.PlayerState.MOVING_OBJECT_STOPPED_LEFT:
        //        case Player.PlayerState.MOVING_OBJECT_LEFT:
        //            pickUpPointLeft.transform.position = movingMetal.transform.position;
        //            break;
        //        case Player.PlayerState.MOVING_OBJECT_STOPPED_RIGHT:
        //        case Player.PlayerState.MOVING_OBJECT_RIGHT:
        //            pickUpPointRight.transform.position = movingMetal.gameObject.transform.position;
        //            break;
        //    }
        if (beingMoved)
        {
            //float moveWithPlayerX = player.transform.position.x + distToPlayerXOffset;
            //transform.position = new Vector2(moveWithPlayerX, transform.position.y);
            //if (playerGFX.GetFacingRight())
            //{
            //    pickUpPointRight.transform.position = MovingMetal.movingMetalInstance.transform.position;
            //}
            //else
            //{
            //    pickUpPointLeft.transform.position = MovingMetal.movingMetalInstance.transform.position;
            //}
        }

        //}
    }

    public bool GetMovable()
    {
        return movable;
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
        if (!state)
        {
            DisconnectMetalFromPlayer();
        }
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
    public void TemporarilyElectrifyObject()
    {
        //add animation code here
        //then set electrified false at the end of the animation with an animation event
        electrified = true;
    }

    public void RemoveDisconnectedObject(GameObject disconnectObject)
    {
        for (int i = 0; i < connectedGameObjects.Count; i++)
        {
            if (connectedGameObjects[i] == disconnectObject)
            {
                connectedGameObjects.RemoveAt(i);
            }
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
            Debug.Log("Test Metal 1");
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
            RemoveDisconnectedObject(collision.gameObject);
            collision.gameObject.GetComponent<Metal>().RemoveDisconnectedObject(gameObject);
            Debug.Log("Test Metal 2");
            bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
            int object2GroupNum = collision.gameObject.GetComponent<Metal>().GetGroupNum();
            ElectricityController.instanceElectrical.DisconnectObjects(
                gameObject, metalCollider, electrified, groupNum,
                collision.gameObject, collision, object2Electrified, object2GroupNum);
            //GameObject tempGameObject = gameObject;
          

        }
        else if (collision.CompareTag("Water") && collision.gameObject.GetComponent<Water>().GetWaterByItself())
        {
            RemoveDisconnectedObject(collision.gameObject);
            collision.gameObject.GetComponent<Water>().RemoveDisconnectedObject(gameObject);
            bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
            int object2GroupNum = collision.gameObject.GetComponent<Water>().GetGroupNum();
            ElectricityController.instanceElectrical.DisconnectObjects(
                gameObject, metalCollider, electrified, groupNum,
                collision.gameObject, collision, object2Electrified, object2GroupNum);
            GameObject tempGameObject = collision.gameObject;
           
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

