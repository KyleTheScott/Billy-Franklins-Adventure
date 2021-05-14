using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour, IElectrifiable
{
    [SerializeField] private bool electrified = false;
    [SerializeField] List<GameObject> connectedGameObjects = new List<GameObject>();
    [SerializeField] private BoxCollider2D waterCollider;
    [SerializeField] private bool colliderStayCheck = false;
    [SerializeField] private bool waterByItself;
    [SerializeField] private GameObject lanternInWater = null;

    private Animator waterAnimator;

    public bool GetElectrified()
    {
        return electrified;
    }

    void Start()
    {
        waterAnimator = gameObject.GetComponent<Animator>();
        waterCollider = gameObject.GetComponent<BoxCollider2D>();
        if (waterByItself)
        {
            waterAnimator.SetBool("WaterSpilt", true);
        }
    }

    public void SpillWater(bool facingRight)
    {
        if (!facingRight)
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
        waterAnimator.SetBool("WaterSpilt", true);

    }


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

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Metal"))
        {
            Debug.Log("Disconnect before 1");
            bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
            ElectricityController.instanceElectrical.DisconnectObjects(gameObject, waterCollider, electrified,
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
