using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour, IElectrifiable
{
    [Header("Electrical")]
    [SerializeField] private bool electrified = false;
    [SerializeField] List<GameObject> connectedGameObjects = new List<GameObject>();

    [Header("General")]
    [SerializeField] private BoxCollider2D waterCollider;
    [SerializeField] private bool colliderStayCheck = false;
    [SerializeField] private bool waterByItself;
    [SerializeField] private GameObject lanternInWater = null;
    private Animator waterAnimator;

   

    void Start()
    {
        waterAnimator = gameObject.GetComponent<Animator>();
        waterCollider = gameObject.GetComponent<BoxCollider2D>();
        if (waterByItself)
        {
            waterAnimator.SetBool("WaterSpilt", true);
        }
    }

    //this is called from the bucket and spills the water out of the tipped bucket
    public void SpillWater(bool facingRight)
    {
        if (!facingRight)
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
        waterAnimator.SetBool("WaterSpilt", true);

    }

    public bool GetElectrified()
    {
        return electrified;
    }
    //set whether the water is electrified and changes animation
    public void SetElectrified(bool state)
    {
        electrified = state;
        waterAnimator.SetBool("Electrified", true);
        if (lanternInWater != null)
        {
            lanternInWater.GetComponent<Lantern>().LanternToggle();
            GlobalGameController.instance.IncreaseCurrentLitLanternNum();
        }
    }

    //gets all the objects directly connected
    public List<GameObject> GetConnectedObjects()
    {
        return connectedGameObjects;
    }

    //called by an animation event to make sure the water collider collides when enabled and already in collision area
    public void SetColliderStayCheck()
    {
        colliderStayCheck = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //elecrify water
        if (collision.CompareTag("Lightning"))
        {
            ElectricityController.instanceElectrical.ElectrifyConnectedObjects(gameObject, waterCollider);
            colliderStayCheck = false;
        }
        //connected game objects 
        else if (collision.CompareTag("Metal"))
        {
            connectedGameObjects.Add(collision.gameObject);
            bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
            ElectricityController.instanceElectrical.ConnectObjects(gameObject, waterCollider, electrified,
                collision.gameObject, collision, object2Electrified);
            colliderStayCheck = false;
        }
        else if (collision.CompareTag("Water"))
        {
            connectedGameObjects.Add(collision.gameObject);
            bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
            ElectricityController.instanceElectrical.ConnectObjects(gameObject, waterCollider, electrified,
                collision.gameObject, collision, object2Electrified);
            colliderStayCheck = false;
        }
    }
    //used for if the collider is enabled and already is colliding and On trigger enter doesn't get called 
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (colliderStayCheck)
        {
            if (collision.CompareTag("Lightning"))
            {
                ElectricityController.instanceElectrical.ElectrifyConnectedObjects(gameObject, waterCollider);
                colliderStayCheck = false;
            }
            else if (collision.CompareTag("Metal"))
            {
                connectedGameObjects.Add(collision.gameObject);
                bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
                ElectricityController.instanceElectrical.ConnectObjects(gameObject, waterCollider, electrified,
                    collision.gameObject, collision, object2Electrified);
                colliderStayCheck = false;
            }
            else if (collision.CompareTag("Water"))
            {
                connectedGameObjects.Add(collision.gameObject);
                bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
                ElectricityController.instanceElectrical.ConnectObjects(gameObject, waterCollider, electrified,
                    collision.gameObject, collision, object2Electrified);
                colliderStayCheck = false;
            }
        }
    }

    //disconnecting game object and removing from the list of direcdtly connected objects
    //might still be in the same grouping
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Metal"))
        {
            bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
            ElectricityController.instanceElectrical.DisconnectObjects(gameObject, waterCollider, electrified,
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
            ElectricityController.instanceElectrical.DisconnectObjects(gameObject, waterCollider, electrified,
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
    }
}
