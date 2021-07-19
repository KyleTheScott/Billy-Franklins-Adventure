using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingMetal : MonoBehaviour
{
    #region Singleton
    public static MovingMetal movingMetalInstance;

   

    private void Awake()
    {
        //Make sure there is only one instance
        if (movingMetalInstance == null)
        {
            movingMetalInstance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion
    private Rigidbody2D movingMetalRB;

    public enum MovingMetalState
    {
        NOT_MOVING_METAL,
        PICK_UP_METAL_START,
        PICK_UP_METAL,
        DRAG_METAL,
        DROP_METAL
    }
    [SerializeField] private MovingMetalState movingMetalState = MovingMetalState.NOT_MOVING_METAL;


    public MovingMetalState GetMovingMetalState()
    {
        return movingMetalState;
    }

    public void SetMovingMetalState(MovingMetalState state)
    {
        movingMetalState = state;
    }

    // Start is called before the first frame update
    void Start()
    {
        movingMetalRB = gameObject.GetComponent<Rigidbody2D>();
        //fixedJoint = gameObject.GetComponent<FixedJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Rigidbody2D GetMovingMetalRigidbody()
    {
        return movingMetalRB;
    }


    public void SetFixedJoint(Rigidbody2D rBody)
    {
        //fixedJoint.connectedBody = rBody;
    }


}
