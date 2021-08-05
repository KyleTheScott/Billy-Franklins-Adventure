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

    // Start is called before the first frame update
    void Start()
    {
        movingMetalRB = gameObject.GetComponent<Rigidbody2D>();
    }

    public Rigidbody2D GetMovingMetalRigidbody()
    {
        return movingMetalRB;
    }
}
