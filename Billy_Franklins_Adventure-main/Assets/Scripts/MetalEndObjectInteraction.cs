using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalEndObjectInteraction : MonoBehaviour
{


    [SerializeField] private List<GameObject> groundObjects = new List<GameObject>();
    [SerializeField] private bool touchingGround = true;


    public bool GetTouchingGround()
    {
        return touchingGround;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundObjects.Add(collision.gameObject);
            touchingGround = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            for (int i = 0; i < groundObjects.Count; i++)
            {
                if (collision.gameObject == groundObjects[i])
                {
                    groundObjects.RemoveAt(i);
                }
            }
            if (groundObjects.Count < 1)
            {
                touchingGround = false;
            }
        }
    }
}
