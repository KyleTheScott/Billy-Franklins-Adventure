using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Water : MonoBehaviour, IElectrifiable
{
    [Header("Electrical")]
    [SerializeField] private bool electrified = false;
    [SerializeField] List<GameObject> connectedGameObjects = new List<GameObject>();

    [Header("General")]
    [SerializeField] private BoxCollider2D waterCollider;
    [SerializeField] private bool colliderStayCheck = true;
    [SerializeField] private bool waterByItself;
    [SerializeField] private List<GameObject> lanternInWater = new List<GameObject>();
    private Animator waterAnimator;
    [SerializeField] private bool oldElectrifiable;
    [SerializeField] private bool started;
    [SerializeField] private bool starting;

    [Header("FMOD Settings")]
    [SerializeField]
    [FMODUnity.EventRef]
    private string bucketKickEventRef;
    [SerializeField]
    private float bucketKickVolume = 0.8f;
    [SerializeField]
    [FMODUnity.EventRef]
    private string electricWaterEventRef;
    private FMOD.Studio.EventInstance electricWaterEvent;
    [SerializeField]
    private float electricWaterVolume = 0.8f;

    [SerializeField] private int groupNum = 0;

    //[SerializeField] private ElectricityController electricityController;

    //[SerializeField] private bool startingOut = true;
    //[SerializeField] private bool starting = false;
    //[SerializeField] private bool started = false;


    //public void SetStarted()
    //{
    //    started = true;
    //}
    //public bool GetStarted()
    //{
    //    return started;
    //}

    public bool IsOldElectrifiable()
    {
        return oldElectrifiable;
    }

    public void SetIsOldElectrifiable(bool state)
    {
        oldElectrifiable = state;
    }


    void Start()
    {
        waterAnimator = gameObject.GetComponent<Animator>();
        waterCollider = gameObject.GetComponent<BoxCollider2D>();
        if (waterByItself)
        {
            waterAnimator.SetBool("WaterSpilt", true);
        }
        electricWaterEvent = FMODUnity.RuntimeManager.CreateInstance(electricWaterEventRef);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(electricWaterEvent, transform, GetComponent<Rigidbody>());
        electricWaterEvent.setVolume(electricWaterVolume);

        //int currentSceneNum = SceneManager.sceneCount - 1;
        //string sceneName = "root_" + SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name;

        //GameObject sceneObject = GameObject.Find(sceneName);
        //Debug.LogError("String" + sceneName + sceneObject.name);
        //foreach (Transform obj in sceneObject.transform)
        //{
        //    if (obj.gameObject.name == "ElectricityController")
        //    {
        //        electricityController = obj.gameObject.GetComponent<ElectricityController>();
        //        break;
        //    }
        //}
        //starting = true;
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
        waterByItself = true;
        FMODUnity.RuntimeManager.PlayOneShot(bucketKickEventRef, bucketKickVolume);
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
        if (electrified)
        {
            electricWaterEvent.start();
        }
        else
        {
            electricWaterEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
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
        if (connectedGameObjects == null)
        {
            return null;
        }
        else
        {
            return connectedGameObjects;
        }
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
        if (waterByItself)
        {
            //elecrify water
            if (collision.CompareTag("Lightning"))
            {
                ElectricityController.instanceElectrical.ElectrifyConnectedObjects(gameObject);
                colliderStayCheck = false;
            }
            //connected game objects 
            else if (collision.CompareTag("Metal"))
            {
                if (!IsOldElectrifiable() && !collision.GetComponent<IElectrifiable>().IsOldElectrifiable())
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
                    ElectricityController.instanceElectrical.ConnectObjects(gameObject, collision.gameObject);
                    colliderStayCheck = false;
                }
            }
            else if (collision.CompareTag("Water") && collision.gameObject.GetComponent<Water>().GetWaterByItself())
            {
                if (!IsOldElectrifiable() && !collision.GetComponent<IElectrifiable>().IsOldElectrifiable())
                {
                    connectedGameObjects.Add(collision.gameObject);
                    bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
                    int object2GroupNum = collision.gameObject.GetComponent<Water>().GetGroupNum();
                    ElectricityController.instanceElectrical.ConnectObjects(gameObject, collision.gameObject);
                    colliderStayCheck = false;
                }
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
                    ElectricityController.instanceElectrical.ElectrifyConnectedObjects(gameObject);
                    colliderStayCheck = false;
                    //startingOut = false;
                }
                else if (collision.CompareTag("Metal"))
                {
                    if (!IsOldElectrifiable() && !collision.GetComponent<IElectrifiable>().IsOldElectrifiable())
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
                        ElectricityController.instanceElectrical.ConnectObjects(gameObject, collision.gameObject);
                        colliderStayCheck = false;
                    }
                    //startingOut = false;
                }
                else if (collision.CompareTag("Water") && collision.gameObject.GetComponent<Water>().GetWaterByItself())
                {
                    if (!IsOldElectrifiable() && !collision.GetComponent<IElectrifiable>().IsOldElectrifiable())
                    {
                        connectedGameObjects.Add(collision.gameObject);
                        bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
                        int object2GroupNum = collision.gameObject.GetComponent<Water>().GetGroupNum();
                        ElectricityController.instanceElectrical.ConnectObjects(gameObject, collision.gameObject);
                        colliderStayCheck = false;
                    }
                    //startingOut = false;
                }
            }
        }
    }

    //disconnecting game object and removing from the list of directly connected objects
    //might still be in the same grouping
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (waterByItself)
        {
            if (collision.CompareTag("Metal"))
            {
                if (!IsOldElectrifiable() && !collision.GetComponent<IElectrifiable>().IsOldElectrifiable())
                {
                    RemoveDisconnectedObject(collision.gameObject);
                    collision.gameObject.GetComponent<Metal>().RemoveDisconnectedObject(gameObject);
                    bool object2Electrified = collision.gameObject.GetComponent<Metal>().GetElectrified();
                    int object2GroupNum = collision.gameObject.GetComponent<Metal>().GetGroupNum();
                    ElectricityController.instanceElectrical.DisconnectObjects(gameObject, collision.gameObject);
                    GameObject tempGameObject = gameObject;
                }
            }
            else if (collision.CompareTag("Water") && collision.gameObject.GetComponent<Water>().GetWaterByItself())
            {
                if (!IsOldElectrifiable() && !collision.GetComponent<IElectrifiable>().IsOldElectrifiable())
                {
                    RemoveDisconnectedObject(collision.gameObject);
                    collision.gameObject.GetComponent<Water>().RemoveDisconnectedObject(gameObject);
                    bool object2Electrified = collision.gameObject.GetComponent<Water>().GetElectrified();
                    int object2GroupNum = collision.gameObject.GetComponent<Water>().GetGroupNum();
                    ElectricityController.instanceElectrical.DisconnectObjects(gameObject, collision.gameObject);
                    GameObject tempGameObject = collision.gameObject;
                }
            }
        }
    }
}
