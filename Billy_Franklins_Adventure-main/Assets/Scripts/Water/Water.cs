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
    [SerializeField] private List<GameObject> lanternInWater = new List<GameObject>();
    private Animator waterAnimator;

    [SerializeField] private int groupNum = 0;



    void Start()
    {
        waterAnimator = gameObject.GetComponent<Animator>();
        waterCollider = gameObject.GetComponent<BoxCollider2D>();
        if (waterByItself)
        {
            waterAnimator.SetBool("WaterSpilt", true);
        }
    }
    //public void SetCollider()
    //{
       
    //}


    //this is called from the bucket and spills the water out of the tipped bucket
    public void SpillWater(bool facingRight)
    {
        if (!facingRight)
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
        waterAnimator.SetBool("WaterSpilt", true);

    }

    //for number of objects group
    public int GetGroupNum()
    {
        return groupNum;
    }
    public void SetGroupNum(int num)
    {
        groupNum = num;
    }

    //set if water is in a bucket or by itself
    public bool GetWaterByItself()
    {
        return waterByItself;
    }
    public void SetWaterByItself(bool state)
    {
        waterByItself = state;
    }

    // get if water is electrified
    public bool GetElectrified()
    {
        return electrified;
    }
    //set whether the water is electrified and changes animation
    public void SetElectrified(bool state)
    {
        electrified = state;
        waterAnimator.SetBool("Electrified", true);
        if (lanternInWater.Count >= 1)
        {
            foreach (GameObject l in lanternInWater)
            {
                l.GetComponent<Lantern>().LanternToggle();
                GlobalGameController.instance.IncreaseCurrentLitLanternNum();
            }
            
        }
    }

    //gets all the objects directly connected
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


    //called by an animation event to make sure the water collider collides when enabled and already in collision area
    public void SetColliderStayCheck()
    {
        colliderStayCheck = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (waterByItself)
        {
            //elecrify water
            if (collision.CompareTag("Lightning"))
            {
                ElectricityController.instanceElectrical.ElectrifyConnectedObjects(
                    gameObject, waterCollider, electrified, groupNum);
                colliderStayCheck = false;
            }
            //connected game objects 
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
                    gameObject, waterCollider, electrified, groupNum,
                    collision.gameObject, collision, object2Electrified, object2GroupNum);
                colliderStayCheck = false;
            }
            else if (collision.CompareTag("Water") && collision.gameObject.GetComponent<Water>().GetWaterByItself())
            {
                connectedGameObjects.Add(collision.gameObject);
                bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
                int object2GroupNum = collision.gameObject.GetComponent<Water>().GetGroupNum();
                ElectricityController.instanceElectrical.ConnectObjects(
                    gameObject, waterCollider, electrified, groupNum,
                    collision.gameObject, collision, object2Electrified, object2GroupNum);
                colliderStayCheck = false;
            }
        }
    }
    //used for if the collider is enabled and already is colliding and On trigger enter doesn't get called 
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (waterByItself)
        { 
            if (colliderStayCheck)
            {
                if (collision.CompareTag("Lightning"))
                {
                    ElectricityController.instanceElectrical.ElectrifyConnectedObjects(
                        gameObject, waterCollider, electrified, groupNum);
                    colliderStayCheck = false;
                }
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
                        gameObject, waterCollider, electrified, groupNum,
                        collision.gameObject, collision, object2Electrified, object2GroupNum);
                    colliderStayCheck = false;
                }
                else if (collision.CompareTag("Water") && collision.gameObject.GetComponent<Water>().GetWaterByItself())
                {
                    connectedGameObjects.Add(collision.gameObject);
                    bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
                    int object2GroupNum = collision.gameObject.GetComponent<Water>().GetGroupNum();
                    ElectricityController.instanceElectrical.ConnectObjects(
                        gameObject, waterCollider, electrified, groupNum,
                        collision.gameObject, collision, object2Electrified, object2GroupNum);
                    colliderStayCheck = false;
                }
            }
        }
    }

    //disconnecting game object and removing from the list of direcdtly connected objects
    //might still be in the same grouping
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (waterByItself)
        {
            if (collision.CompareTag("Metal"))
            {
                bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
                int object2GroupNum = collision.gameObject.GetComponent<Metal>().GetGroupNum();
                ElectricityController.instanceElectrical.DisconnectObjects(
                    gameObject, waterCollider, electrified, groupNum,
                    collision.gameObject, collision, object2Electrified, object2GroupNum);
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
                    gameObject, waterCollider, electrified, groupNum,
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
        }
    }
}
