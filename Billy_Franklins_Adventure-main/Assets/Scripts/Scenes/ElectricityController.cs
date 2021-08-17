using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-98)]

public class ElectricityController : MonoBehaviour
{
    #region Singleton

    public static ElectricityController instanceElectrical;

    private void Awake()
    {
        //Make sure there is only one instance
        if (instanceElectrical == null)
        {
            instanceElectrical = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] private GameObject electrifiables;
    //[SerializeField] private GameObject waterMain;

    [SerializeField]
    private List<GameObject> connectedGameObjects = new List<GameObject>(); // list of objects that have connections

    [SerializeField] private int availableNum = 1; // number given to the next brand new group
    [SerializeField] private bool debugging = false; //for debugging
    private Player player;

    private bool started = true;
    private float timer = 0;
    private float time = 3;


    //[SerializeField] private string sceneName = "root_Puzzle10";

    //void Update()
    //{
        //if (!started)
        //{
        //    if (timer >= time)
        //    {
        //        if (electrifiables == null)
        //        {
        //            int currentSceneNum = SceneManager.sceneCount - 1;
        //            string sceneName = "root_" + SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name;

        //            //Debug.LogError("String" + sceneName);
        //            GameObject sceneObject = GameObject.Find(sceneName);
        //            foreach (Transform obj in sceneObject.transform)
        //            {
        //                if (obj.gameObject.name == "Electrifiables")
        //                {
        //                    electrifiables = obj.gameObject;
        //                    break;
        //                }
        //            }
        //        }
        //        //Debug.LogError("Update Electrify");
        //        foreach (Transform obj in electrifiables.transform)
        //        {
        //            if (obj.gameObject.name == "ElectrifiableGroup")
        //            {
        //                foreach (Transform obj2 in obj)
        //                {
        //                    if (obj2.GetComponent<IElectrifiable>() != null)
        //                    {
        //                        List<GameObject> connectedToCurrent = new List<GameObject>();
        //                        connectedToCurrent = obj2.GetComponent<IElectrifiable>().GetConnectedObjects();
        //                        foreach (GameObject objConnected in connectedToCurrent)
        //                        {
        //                            ConnectObjects(obj2.gameObject, objConnected);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (obj.GetComponent<IElectrifiable>() != null)
        //                {
        //                    List<GameObject> connectedToCurrent = new List<GameObject>();
        //                    connectedToCurrent = obj.GetComponent<IElectrifiable>().GetConnectedObjects();
        //                    foreach (GameObject objConnected in connectedToCurrent)
        //                    {
        //                        ConnectObjects(obj.gameObject, objConnected);
        //                    }
        //                }
        //            }
        //        }
        //        timer = 0;
        //        started = true;
        //    }
        //    else
        //    {
        //        timer += Time.deltaTime;
        //    }
        //}
    //}


    public void EmptyObjects()
    {
        connectedGameObjects.Clear();
    }

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        //if (electrifiables == null)
        //{
        //    int currentSceneNum = SceneManager.sceneCount - 1;
        //    string sceneName = "root_" + SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name;

        //    //Debug.LogError("String" + sceneName);
        //    GameObject sceneObject = GameObject.Find(sceneName);
        //    foreach (Transform obj in sceneObject.transform)
        //    {
        //        if (obj.gameObject.name == "Electrifiables")
        //        {
        //            electrifiables = obj.gameObject;
        //            break;
        //        }
        //    }
        //}
    }

    public void SetNewElectrifiableScene()
    {
        //Debug.LogError("Test");
        if (electrifiables != null)
        {
            foreach (Transform obj in electrifiables.transform)
            {
                if (obj.gameObject.name == "ElectrifiableGroup")
                {
                    foreach (Transform obj2 in obj)
                    {
                        obj2.GetComponent<IElectrifiable>().SetIsOldElectrifiable(true);

                    }
                }
                else
                {
                    obj.GetComponent<IElectrifiable>().SetIsOldElectrifiable(true);
                }
            }
        }
        //started = false;
    }


    

    public void ConnectObjects(GameObject object1, GameObject object2)
    {

        if (electrifiables == null)
        {
            int currentSceneNum = SceneManager.sceneCount - 1;
            string sceneName = "root_" + SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name;

            //Debug.LogError("String Connect" + sceneName);
            GameObject sceneObject = GameObject.Find(sceneName);
            foreach (Transform obj in sceneObject.transform)
            {
                if (obj.gameObject.name == "Electrifiables")
                {
                    electrifiables = obj.gameObject;
                    break;
                }
            }
        }
        else
        {
            if (object1.transform.parent.name == "ElectrifiableGroup")
            {
                electrifiables = object1.transform.parent.parent.gameObject;
            }
            else
            {
                electrifiables = object1.transform.parent.gameObject;
            }
            
        }

        //object1 is not in a group
            if (object1.transform.parent == electrifiables.transform)
            {
                //object2 is not in a group
                if (object2.transform.parent == electrifiables.transform)
                {
                    GameObject electrifiableGroup = new GameObject("ElectrifiableGroup");
                    electrifiableGroup.transform.parent = electrifiables.transform;
                    electrifiableGroup.tag = "ElectrifiableGroup";
                    object1.transform.parent = electrifiableGroup.transform;
                    object2.transform.parent = electrifiableGroup.transform;
                }
                //object2 is in a group
                else
                {
                    object1.transform.parent = object2.transform.parent;
                }
            }
            //object1 is in a group
            else
            {
                //object2 is not in a group
                if (object2.transform.parent == electrifiables.transform)
                {
                    object2.transform.parent = object1.transform.parent;
                }
                //object2 is in a group
                else
                {
                    if (object1.transform.parent != object2.transform.parent && object1 != object2)
                    {
                        Transform parentObject2 = object2.transform.parent.gameObject.transform;

                        while (parentObject2.childCount > 0)
                        {
                            for (int i = 0; i < parentObject2.childCount; i++)
                            {
                                parentObject2.GetChild(i).parent = object1.transform.parent;
                            }
                        }

                        List<GameObject> destroyObjects = new List<GameObject>();
                        //object2.transform.parent = object1.transform.parent;
                        for (int i = 0; i < electrifiables.transform.childCount; i++)
                        {
                            if (electrifiables.transform.GetChild(i).name == "ElectrifiableGroup")
                            {
                                if (electrifiables.transform.GetChild(i).transform.childCount < 1)
                                {
                                    destroyObjects.Add(electrifiables.transform.GetChild(i).gameObject);

                                }
                                else if (electrifiables.transform.GetChild(i).transform.childCount == 1)
                                {
                                    electrifiables.transform.GetChild(i).transform.GetChild(0).transform.parent = electrifiables.transform;
                                    destroyObjects.Add(electrifiables.transform.GetChild(i).gameObject);
                                }
                        }
                        }

                        foreach (GameObject obj in destroyObjects)
                        {
                            Destroy(obj);
                        }
                    }
                }
            }

            ElectrifyConnectedFallingObjects(object1);
        
    }

    public void ConnectConnectedObjects()
    {

        int currentSceneNum = SceneManager.sceneCount - 1;
        string sceneName = "root_" + SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name;

        //Debug.LogError("String" + sceneName);
        GameObject sceneObject = GameObject.Find(sceneName);
        foreach (Transform obj in sceneObject.transform)
        {
            if (obj.gameObject.name == "Electrifiables")
            {
                electrifiables = obj.gameObject;
                break;
            }
        }

        //Debug.LogError("connect connected 2");
        if (electrifiables != null)
        {
            if (electrifiables.transform.childCount > 0)
            {
                //Debug.LogError("connect connected 3");
                foreach (Transform obj in electrifiables.transform)
                {
                    if (obj.gameObject.name == "ElectrifiableGroup")
                    {
                        foreach (Transform obj2 in obj.transform)
                        {
                            obj2.transform.parent = electrifiables.transform;
                        }
                    }
                }

                bool disconnect = false;
                while (!disconnect)
                {
                    disconnect = true;
                    foreach (Transform obj in electrifiables.transform)
                    {
                        if (obj.gameObject.name != "ElectrifiableGroup")
                        {
                            GameObject electrifiableGroup = new GameObject("ElectrifiableGroup");
                            electrifiableGroup.transform.parent = electrifiables.transform;
                            obj.transform.parent = electrifiableGroup.transform;
                            electrifiableGroup.tag = "ElectrifiableGroup";
                            disconnect = false;
                        }
                    }
                }

                GameObject currentSubObject = electrifiables.transform.GetChild(0).gameObject;

                int groupPos = 1;
                int connectionNum = 0;
                bool reconnected = false;
                while (!reconnected)
                {
                    connectionNum = 0;

                    //loops through the current sub group
                    foreach (Transform obj in currentSubObject.transform)
                    {
                        //loops through all sub groups
                        foreach (Transform subObj in electrifiables.transform)
                        {
                            //makes sure the subObj is not checking against current sub object that the other groups are being compared against
                            if (subObj.transform != currentSubObject.transform)
                            {
                                //check each electrifiable in the sub group
                                foreach (Transform subObjects in subObj.transform)
                                {
                                    /*make a list to check if the objects that are connected to the current electrifiable are
                                        connected to the objects in current sub group*/
                                    if (subObjects.gameObject.GetComponent<IElectrifiable>() != null)
                                    {
                                        List<GameObject> connectedToCurrentSub = new List<GameObject>();
                                        connectedToCurrentSub = subObjects.gameObject.GetComponent<IElectrifiable>()
                                            .GetConnectedObjects();

                                        //looping through connected objects
                                        foreach (GameObject objConnected in connectedToCurrentSub)
                                        {
                                            // if the sub group electrifiable is connected to the current object in the current sub group
                                            if (obj.gameObject == objConnected)
                                            {
                                                connectionNum++;
                                                subObjects.transform.parent = currentSubObject.transform;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //deleting empty sub groups
                    foreach (Transform obj in electrifiables.transform)
                    {
                        if (obj.childCount < 1)
                        {
                            Destroy(obj.gameObject);
                        }
                    }

                    //checking if any connections were made
                    if (connectionNum == 0)
                    {
                        //checks if it has looped through all the groups and made connections
                        if (groupPos >= electrifiables.transform.childCount)
                        {
                            reconnected = true;
                        }
                        //if not the position is incremented to make more connections
                        else
                        {
                            //changes the current sub object to the next position
                            currentSubObject = electrifiables.transform.GetChild(groupPos).gameObject;
                            groupPos++;
                        }
                    }
                }
                List<GameObject> destroyObjects = new List<GameObject>();
                for (int i = 0; i < electrifiables.transform.childCount; i++)
                {
                    if (electrifiables.transform.GetChild(i).name == "ElectrifiableGroup")
                    {
                        if (electrifiables.transform.GetChild(i).transform.childCount < 1)
                        {
                            destroyObjects.Add(electrifiables.transform.GetChild(i).gameObject);
                        }
                        else if (electrifiables.transform.GetChild(i).transform.childCount == 1)
                        {
                            electrifiables.transform.GetChild(i).transform.GetChild(0).transform.parent = electrifiables.transform;
                            destroyObjects.Add(electrifiables.transform.GetChild(i).gameObject);
                        }
                    }
                }

                foreach (GameObject obj in destroyObjects)
                {
                    Destroy(obj);
                }
            }
        }
    }


    public void DisconnectObjects(GameObject object1, GameObject object2)
    {
        bool bothWater = false;
        if (object1.CompareTag("Water") && object2.CompareTag("Water"))
        {
            bothWater = true;
        }

        if (object1.transform.parent == object2.transform.parent && !bothWater)
        {
            //Debug.LogError("Disconnecting object 1:" + object1.name);
            //Debug.LogError("Disconnecting object 2:" + object2.name);
            
            //object to add subgroups made up of only connected connected objects from the original group 
            GameObject electrifiableSubGroups = new GameObject("ElectrifiableSubGroups");
            //is a child of the main scene in the hierarchy
            electrifiableSubGroups.transform.parent = null;

            //the start group of object1
            GameObject elecrifiableStartGroup = object1.transform.parent.gameObject;

            //current object which is being checked to see if other objects have it as connected objects to add to 
            GameObject currentObject = object1;
            //boolean to keep track of if object1 and object2 are disconnected
            bool disconnected = false;
            while (!disconnected)
            {
                //start a new subGroup
                GameObject electrifiableSubGroup = new GameObject("ElectrifiableSubGroup");
                //the sub group is part of the subGroups parent object
                electrifiableSubGroup.transform.parent = electrifiableSubGroups.transform;
                //loop through the current object's parent which is the group that it is part of
                foreach (Transform obj in currentObject.transform.parent)
                {
                    //check every other object but the current object
                    if (obj != currentObject)
                    {
                        if (obj != null)
                        {
                            //make a list of the objects connected to obj to check if they are connected to the current object
                            List<GameObject> connectedToCurrent = new List<GameObject>();
                            if (obj.gameObject.GetComponent<IElectrifiable>() != null)
                            {
                                connectedToCurrent = obj.gameObject.GetComponent<IElectrifiable>().GetConnectedObjects();
                                foreach (GameObject objConnected in connectedToCurrent)
                                {
                                    /*one of the connected objects is connected to current object so they are
                                     added to the sub group created at the beginning of the while loop*/
                                    if (currentObject == objConnected)
                                    {
                                        obj.gameObject.transform.parent = electrifiableSubGroup.transform;
                                    }
                                }
                            }
                        }
                    }
                }
                //the current object is added to the sub group created at the beginning of the while loop 
                currentObject.transform.parent = electrifiableSubGroup.transform;
                //the start group with object1 is now empty so everything has been added to sub groups
                if (elecrifiableStartGroup.transform.childCount < 1)
                {
                    disconnected = true;
                }
                /*the start group is not empty so the current object is set to the first position of the
                 start group to go through the loop again until the start group is empty*/
                else
                {
                    currentObject = elecrifiableStartGroup.transform.GetChild(0).gameObject;
                }
            }
            //to check if the subgroups are reconnected to other groups that are connected still after disconnecting
            bool reconnected = false;
            //set the current sub object to the first sub group in the sub groups
            GameObject currentSubObject = electrifiableSubGroups.transform.GetChild(0).gameObject;

            int groupPos = 1;
            int connectionNum = 0;
            while (!reconnected)
            {
                connectionNum = 0;

                //loops through the current sub group
                foreach (Transform obj in currentSubObject.transform)
                {
                    //loops through all sub groups
                    foreach (Transform subObj in electrifiableSubGroups.transform)
                    {
                        //makes sure the subObj is not checking against current sub object that the other groups are being compared against
                        if (subObj.transform != currentSubObject.transform)
                        {
                            //check each electrifiable in the sub group
                            foreach (Transform subObjects in subObj.transform)
                            {
                                //if (subObjects.name != "ElectrifiableSubGroup" && subObjects != null)
                                //{ 
                                /*make a list to check if the objects that are connected to the current electrifiable are
                                    connected to the objects in current sub group*/
                                if (subObjects.gameObject.GetComponent<IElectrifiable>() != null)
                                {
                                    List<GameObject> connectedToCurrentSub = new List<GameObject>();
                                    connectedToCurrentSub = subObjects.gameObject.GetComponent<IElectrifiable>().GetConnectedObjects();
                                
                                    //looping through connected objects
                                    foreach (GameObject objConnected in connectedToCurrentSub)
                                    {
                                        // if the sub group electrifiable is connected to the current object in the current sub group
                                        if (obj.gameObject == objConnected)
                                        {
                                            connectionNum++;
                                            subObjects.transform.parent = currentSubObject.transform;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //deleting empty sub groups
                foreach (Transform obj in electrifiableSubGroups.transform)
                {
                    if (obj.childCount < 1)
                    {
                        Destroy(obj.gameObject);
                    }
                }
                //checking if any connections were made
                if (connectionNum == 0)
                {
                    //checks if it has looped through all the groups and made connections
                    if (groupPos >= electrifiableSubGroups.transform.childCount)
                    {       
                        reconnected = true;
                    }
                    //if not the position is incremented to make more connections
                    else
                    {
                        //changes the current sub object to the next position
                        currentSubObject = electrifiableSubGroups.transform.GetChild(groupPos).gameObject;
                        groupPos++;
                    }
                }
            }
            //loops through each sub group to create new electrifiable groups in electrifiables
            foreach (Transform subGroup in electrifiableSubGroups.transform)
            {
                List<GameObject> currentObjectsToAdd = new List<GameObject>();

                foreach (Transform obj in subGroup)
                {
                    currentObjectsToAdd.Add(obj.gameObject);
                }

                GameObject electrifiableGroup = new GameObject("ElectrifiableGroup");
                electrifiableGroup.transform.parent = electrifiables.transform;
                electrifiableGroup.tag = "ElectrifiableGroup";
                foreach (GameObject obj in currentObjectsToAdd)
                {
                    obj.transform.parent = electrifiableGroup.transform;

                }
            }
            //Destroy all the subgroups after moving all the game objects
            Destroy(electrifiableSubGroups);
            /*loop through electrifiables and destroy any empty groups
            groups wth only one object are set to be the child of electrifiables*/
            List<GameObject> destroyGroup = new List<GameObject>();
            foreach (Transform group in electrifiables.transform)
            {
                if (group.CompareTag("ElectrifiableGroup"))
                {
                    if (group.childCount == 1)
                    {
                        destroyGroup.Add(group.gameObject);
                        group.GetChild(0).parent = electrifiables.transform;
                    }
                    else if (group.childCount < 1)
                    {
                        destroyGroup.Add(group.gameObject);
                    }
                }
            }
            foreach (GameObject destroyObj in destroyGroup)
            {
                if (destroyObj.GetComponent<IElectrifiable>() != null)
                {
                    bool connected = false;
                    List<GameObject> connectedToCurrent = new List<GameObject>();
                    connectedToCurrent = destroyObj.GetComponent<IElectrifiable>().GetConnectedObjects();
                    if (connectedToCurrent != null)
                    {
                        foreach (GameObject objConnected in connectedToCurrent)
                        {
                            if (objConnected != null)
                            {
                                if (objConnected.transform.parent.name == "ElectrifiableGroup")
                                {
                                    connected = true;
                                }
                            }
                        }

                        if (!connected)
                        {
                            Destroy(destroyObj);
                        }
                    }
                }
            }
        }
        List<GameObject> destroyObjects = new List<GameObject>();
        //object2.transform.parent = object1.transform.parent;
        for (int i = 0; i < electrifiables.transform.childCount; i++)
        {
            if (electrifiables.transform.GetChild(i).name == "ElectrifiableGroup")
            {
                if (electrifiables.transform.GetChild(i).transform.childCount < 1)
                {
                    destroyObjects.Add(electrifiables.transform.GetChild(i).gameObject);

                }
                else if (electrifiables.transform.GetChild(i).transform.childCount == 1)
                {
                    electrifiables.transform.GetChild(i).transform.GetChild(0).transform.parent = electrifiables.transform;
                    destroyObjects.Add(electrifiables.transform.GetChild(i).gameObject);
                }
            }
        }
        foreach (GameObject obj in destroyObjects)
        {
            Destroy(obj);
        }
    }

    public void ElectrifyConnectedObjects(GameObject object1)
    {
        //checks if object1 is water and if so electrify all the objects in it's group
        if (object1.CompareTag("Water"))
        {
            //electrify the whole group the water is part of
            if (object1.transform.parent.CompareTag("ElectrifiableGroup"))
            {
                foreach (Transform obj in object1.transform.parent.transform)
                {
                    obj.gameObject.GetComponent<IElectrifiable>().SetElectrified(true);
                    //obj.gameObject.GetComponent<Collider2D>().enabled = false;
                }
            }
            // water is not in a group so only it is has to be electrified
            else
            {
                object1.gameObject.GetComponent<IElectrifiable>().SetElectrified(true);
                //object1.gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
        else
        {
            if (object1.transform.parent.CompareTag("ElectrifiableGroup"))
            {
                //checks if there is a water object in the group and if so it electrifies the whole group
                bool electrified = false;
                foreach (Transform obj in object1.transform.parent.transform)
                {
                    if (obj.gameObject.CompareTag("Water"))
                    {
                        electrified = true;
                        break;
                    }
                }
                //electrify everything in the group
                if (electrified)
                {
                    foreach (Transform obj in object1.transform.parent.transform)
                    {
                        obj.gameObject.GetComponent<IElectrifiable>().SetElectrified(true);
                        //obj.gameObject.GetComponent<Collider2D>().enabled = false;
                    }
                }
                //electrify temporarily
                else
                {
                    foreach (Transform obj in object1.transform.parent.transform)
                    {
                        obj.gameObject.GetComponent<Metal>().TemporarilyElectrifyObject();
                    }
                }
            }
            else
            {
                object1.gameObject.GetComponent<Metal>().TemporarilyElectrifyObject();
            }
        }
    }

    public void ElectrifyConnectedFallingObjects(GameObject object1)
    {
        if (object1.transform.parent.CompareTag("ElectrifiableGroup"))
        {
            //checks if there is a water object in the group and if so it electrifies the whole group
            bool electrified = false;
            foreach (Transform obj in object1.transform.parent.transform)
            {
                if (obj.gameObject.CompareTag("Water"))
                {
                    //Debug.Log("Water Electrify");
                    if (obj.GetComponent<Water>().GetElectrified())
                    {
                        electrified = true;
                    }
                    break;
                }
            }
            //electrify everything in the group
            if (electrified)
            {
                foreach (Transform obj in object1.transform.parent.transform)
                {
                    if (obj.GetComponent<IElectrifiable>() != null)
                    {
                        obj.gameObject.GetComponent<IElectrifiable>().SetElectrified(true);
                    }
                }
            }
        }
    }
}

