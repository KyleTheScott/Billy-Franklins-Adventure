using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //state of door
    public enum DoorState
    {
        DOOR_OPENING,
        DOOR_CLOSING,
        DOOR_STOPPED_OPEN,
        DOOR_STOPPED_CLOSED

    };

    [SerializeField] private DoorState doorState; // state of door

    [SerializeField] private List<Door> connectedDoors = new List<Door>(); // doors connected to switch 


    [SerializeField] private float moveSpeed = 3; // speed door moves up or down at
    [SerializeField] private float moveMin; // lowest y position door moves to
    [SerializeField] private float moveMax; // highest y position door moves to

    private Rigidbody2D doorRB;

    [SerializeField] private GameObject puzzleObject;

    void Start()
    {
        doorRB = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
       DoorMovement();
    }
    // moves door up or down
    private void DoorMovement()
    {
        switch (doorState)
        {
            case DoorState.DOOR_OPENING:
                if (transform.position.y < moveMax + puzzleObject.transform.position.y)
                {
                    transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
                    Debug.Log("Y: " + transform.position.y + "Max: " + moveMax);
                }
                else
                {
                    doorState = DoorState.DOOR_STOPPED_OPEN;
                    transform.position = new Vector2(transform.position.x, moveMax + puzzleObject.transform.position.y);
                }
                break;
            case DoorState.DOOR_CLOSING:
                if (transform.position.y > moveMin + puzzleObject.transform.position.y)
                {
                    transform.position += new Vector3(0, -moveSpeed * Time.deltaTime, 0);
                    Debug.Log("Y: " + transform.position.y + "Min: " + moveMin);
                }
                else
                {
                    doorState = DoorState.DOOR_STOPPED_CLOSED;
                    transform.position = new Vector2(transform.position.x, moveMin + puzzleObject.transform.position.y);
                }
                break;
        }
    }

    public DoorState GetDoorState ()
    {
        return doorState;
    }
    public void SetDoorState(DoorState state)
    {
        doorState = state;
    }

    /*
    sets the state of the door and all doors connected to switch if it is the first door 
    being processed when you Interact or shoot the Switch
    */
    public void SetDoorState()
    {
        Debug.LogError("Problem 1");
        if (doorState == DoorState.DOOR_STOPPED_OPEN || doorState == DoorState.DOOR_STOPPED_CLOSED)
        {
            Debug.LogError("Problem 2");
            bool doorsActive = false;
            for (int i = 0; i < connectedDoors.Count; i++)
            {
                DoorState tempDoorState = connectedDoors[i].GetComponent<Door>().GetDoorState();
                if (tempDoorState == DoorState.DOOR_OPENING || tempDoorState == DoorState.DOOR_CLOSING)
                {
                    Debug.LogError("Problem 3");
                    doorsActive = true;
                }
            }
            if (!doorsActive)
            {
                Debug.LogError("Problem 4");
                if (doorState == DoorState.DOOR_STOPPED_OPEN)
                {
                    Debug.LogError("Problem 5");
                    doorState = DoorState.DOOR_CLOSING;
                }
                else
                {
                    Debug.LogError("Problem 6");
                    doorState = DoorState.DOOR_OPENING;
                }

                for (int i = 0; i < connectedDoors.Count; i++)
                {
                    DoorState tempDoorState = connectedDoors[i].GetComponent<Door>().GetDoorState();
                    if (tempDoorState == DoorState.DOOR_STOPPED_OPEN)
                    {
                        Debug.LogError("Problem 7");
                        connectedDoors[i].GetComponent<Door>().SetDoorState(DoorState.DOOR_CLOSING);
                    }
                    else
                    {
                        Debug.LogError("Problem 8");
                        connectedDoors[i].GetComponent<Door>().SetDoorState(DoorState.DOOR_OPENING);
                    }
                }
            }
        }
    }
}
