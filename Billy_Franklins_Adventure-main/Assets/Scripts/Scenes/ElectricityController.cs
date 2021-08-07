using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-99)]

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

    [SerializeField] private string sceneName = "root_Puzzle10";

    public void EmptyObjects()
    {
        connectedGameObjects.Clear();
    }

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
    }

    public void SetNewScene()
    {
        //GameObject sceneObject = GameObject.Find(sceneName);
        //foreach (Transform obj in sceneObject.transform)
        //{

        //}

        if (electrifiables != null)
        {
            foreach (Transform obj in electrifiables.transform)
            {
                Destroy(obj.gameObject);
            }
            electrifiables = null;
        }

     
        //int currentSceneNum = SceneManager.sceneCount - 1;
        //sceneName = "root_" + SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name;

        //Debug.LogError("String" + sceneName);
        //GameObject sceneObject = GameObject.Find(sceneName);
        //foreach (Transform obj in sceneObject.transform)
        //{
        //    if (obj.gameObject.name == "Electrifiables")
        //    {
        //        electrifiables = obj.gameObject;
        //        break;
        //    }
        //}

        //string tempSceneName = "root_" + SceneManager.GetSceneAt(0).name;
        //GameObject tempSceneObject = GameObject.Find(tempSceneName);
        //foreach (Transform obj in tempSceneObject.transform)
        //{
        //    if (obj.gameObject.name == "Electrifiables")
        //    {
        //        foreach (Transform tempObj in obj)
        //        {
        //            tempObj.gameObject.name = "OldElectrifiables";
        //            tempObj.parent = tempSceneObject.transform;
        //        }
        //        break;
        //    }
        //}

        //sceneName = "root_" + name;

        //Debug.LogError("String3" + sceneName);
        //GameObject sceneObject = GameObject.Find(sceneName);
        //foreach (Transform obj in sceneObject.transform)
        //{
        //    if (obj.gameObject.name == "Electrifiables")
        //    {
        //        electrifiables = obj.gameObject;
        //        break;
        //    }
        //}
    }

    //List<GameObject> FindSceneObjects(string sceneName)
    //{
    //    List<GameObject> objs = new List<GameObject>();
    //    foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
    //    {
    //        if (objs.scene.name.CompareTo(sceneName) == 0)
    //        {
    //            objs.Add(objs);
    //        }
    //    }
    //    return objs;
    //}

    public void ConnectObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1, int groupNum1,
        GameObject object2, Collider2D gameObjectCollider2, bool electricState2, int groupNum2)
    {

        if (electrifiables == null)
        {
            int currentSceneNum = SceneManager.sceneCount - 1;
            sceneName = "root_" + SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name;

            Debug.Log("String" + sceneName);
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
                //Debug.LogWarning("Actual Object 1:" + object1.name);
                //Debug.LogWarning("Actual Object 2:" + object2.name);

                //foreach (Transform obj in object1.transform.parent.gameObject.transform)
                //{
                //    Debug.Log("Object 1: " + obj.gameObject.name);
                //}

                //Debug.LogWarning("Actual Object 2:" + object2.name);
                //foreach (Transform obj in object2.transform.parent.gameObject.transform)
                //{
                //    Debug.Log("Object 2: " + obj.gameObject.name);
                //}

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

                    //object2.transform.parent = object1.transform.parent;
                    for (int i = 0; i < electrifiables.transform.childCount; i++)
                    {
                        if (electrifiables.transform.GetChild(i).transform.childCount < 1)
                        {
                            Destroy(electrifiables.transform.GetChild(i).gameObject);
                            break;
                        }
                    }
                }
            }
        }
    }

    //private bool ObjectConnection(GameObject object1, GameObject object2, GameObject currentObject)
    //{



    //    return true;
    //}

    public void DisconnectObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1,
        int groupNum1,
        GameObject object2, Collider2D gameObjectCollider2, bool electricState2, int groupNum2)
    {
        //Debug.LogError("Object 2 Test:" + object2.name);
        //Debug.LogError("Object 1 Test:" + object1.name);

        if (object1.transform.parent == object2.transform.parent)
        {
            //Debug.LogError("Object 2 Test:" + object2.name);
            //Debug.LogError("Object 1 Test:" + object1.name);
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
                //loop through the current object's parent
                foreach (Transform obj in currentObject.transform.parent)
                {
                    //check every other object but the current object
                    if (obj != currentObject)
                    {
                        //make a list of the objects connected to obj to check if they are connected to the current object
                        List<GameObject> connectedToCurrent = new List<GameObject>();
                        connectedToCurrent = obj.GetComponent<IElectrifiable>().GetConnectedObjects();
                        foreach (GameObject objConnected in connectedToCurrent)
                        {
                            /*one of the connected objects is connected to current object so they are
                             added to the sub group created at the beginning of the while loop*/
                            if (currentObject == objConnected)
                            {
                                obj.transform.parent = electrifiableSubGroup.transform;
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
                                /*make a list to check if the objects that are connected to the current electrifiable are
                                    connected to the objects in current sub group*/
                                List<GameObject> connectedToCurrentSub = new List<GameObject>();
                                connectedToCurrentSub = subObjects.GetComponent<IElectrifiable>().GetConnectedObjects();
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
                Destroy(destroyObj);
            }
        }
    }

    public void ElectrifyConnectedObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1,
        int groupNum1)
    {
        //checks if object1 is water and if so electrify all the objects in it's group
        if (gameObjectCollider1.CompareTag("Water"))
        {
            //electrify the whole group the water is part of
            if (object1.transform.parent.CompareTag("ElectrifiableGroup"))
            {
                foreach (Transform obj in object1.transform.parent.transform)
                {
                    obj.gameObject.GetComponent<IElectrifiable>().SetElectrified(true);
                }
            }
            // water is not in a group so only it is has to be electrified
            else
            {
                object1.gameObject.GetComponent<IElectrifiable>().SetElectrified(true);
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
}




    //---------------------------------------------------------------------

        //    Debug.LogError("Object 2 Test:" + object2.name);
        //    Debug.LogError("Object 1 Test:" + object1.name);

        //    List<GameObject> connectedGroup1 = new List<GameObject>();

        //    List<int> connectedGroupNums = new List<int>();


        //    if (object1.transform.parent == object2.transform.parent)
        //    {
        //        GameObject currentObject = object1;
        //        bool sortingGroups = true;

        //        //parent transform of current object
        //        Transform object1Group = currentObject.transform.parent.transform;
        //        //searching through the whole group
        //        for (int k = 0; k < object1Group.childCount; k++)
        //        {
        //            if (object1Group.GetChild(k) != object2)
        //            {
        //                //create a new list
        //                List<GameObject> currentConnected = new List<GameObject>();
        //                //make list filled with connected objects in current object in current objects parent transform
        //                currentConnected = object1Group.GetChild(k).GetComponent<IElectrifiable>()
        //                    .GetConnectedObjects();
        //                //go through it's connected
        //                for (int i = 0; i < currentConnected.Count; i++)
        //                {
        //                    //if the current object is connected add to list of positions within the transform 
        //                    if (currentConnected[i].transform == currentObject.transform)
        //                    {
        //                        if (currentConnected[i].transform != object2.transform)
        //                        {
        //                            connectedGroupNums.Add(k);
        //                        }
        //                    }

        //                    //get the position within the transform of current connected
        //                    int pos = 0;
        //                    foreach (Transform posCheck in object1Group)
        //                    {
        //                        if (posCheck.gameObject == currentConnected[i].transform.gameObject)
        //                        {
        //                            break;
        //                        }

        //                        pos++;
        //                    }

        //                    for (int l = 0; l < connectedGroupNums.Count; l++)
        //                    {
        //                        if (connectedGroupNums[l] == pos)
        //                        {
        //                            int pos2 = 0;
        //                            foreach (Transform posCheck in object1Group)
        //                            {
        //                                if (posCheck.gameObject == currentConnected[i].transform.gameObject)
        //                                {
        //                                    break;
        //                                }

        //                                pos2++;
        //                            }

        //                            if (currentConnected[i].transform != object2.transform)
        //                            {
        //                                connectedGroupNums.Add(k);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        GameObject electrifiableGroup = new GameObject("ElectrifiableGroup");
        //        electrifiableGroup.transform.parent = electrifiables.transform;
        //        List<GameObject> currentConnectedObjects = new List<GameObject>();
        //        for (int m = 0; m < connectedGroupNums.Count; m++)
        //        {
        //            currentConnectedObjects.Add(object1Group.GetChild(connectedGroupNums[m]).gameObject);
        //        }
        //        foreach (GameObject moveObj in currentConnectedObjects)
        //        {
        //            if (player.GetCurrentMovingObject() && object1 == player.GetCurrentMovingObject())
        //            {
        //                moveObj.transform.parent = electrifiableGroup.transform;
        //            }
        //        }
        //        Debug.LogError("Object 2 Test:" + object2.name);
        //        Debug.LogError("Object 1 Test:" + object1.name);

        //        if (electrifiableGroup.transform.childCount == 1)
        //        {
        //            if (player.GetCurrentMovingObject() != null)
        //            {
        //                if (object1 == player.GetCurrentMovingObject())
        //                {
        //                    object1.transform.parent = electrifiables.transform;
        //                }
        //                else
        //                {
        //                    object1.transform.parent = object1Group;
        //                }
        //            }
        //            else
        //            {
        //                object1.transform.parent = object1Group;
        //            }

        //        }
        //        else if (electrifiableGroup.transform.childCount == 2)
        //        {
        //            if (electrifiableGroup.transform.GetChild(0) == object2.transform)
        //            {
        //                object2.transform.parent = object1Group;
        //            }
        //            else if (electrifiableGroup.transform.GetChild(0) == object1.transform)
        //            {
        //                if (player.GetCurrentMovingObject() != null)
        //                {
        //                    if (object1 == player.GetCurrentMovingObject())
        //                    {
        //                        object1.transform.parent = electrifiables.transform;
        //                    }
        //                    else
        //                    {
        //                        object1.transform.parent = object1Group;
        //                    }
        //                }
        //                else
        //                {
        //                    object1.transform.parent = object1Group;
        //                }
        //            }

        //            if (electrifiableGroup.transform.GetChild(1) == object2.transform)
        //            {
        //                object2.transform.parent = object1Group;
        //            }
        //            else if (electrifiableGroup.transform.GetChild(1) == object1.transform)
        //            {
        //                if (player.GetCurrentMovingObject() != null)
        //                {
        //                    if (object1 == player.GetCurrentMovingObject())
        //                    {
        //                        object1.transform.parent = electrifiables.transform;
        //                    }
        //                    else
        //                    {
        //                        object1.transform.parent = object1Group;
        //                    }
        //                }
        //                else
        //                {
        //                    object1.transform.parent = object1Group;
        //                }
        //            }


        //        }
        //        if (electrifiableGroup.transform.childCount > 0)
        //        {
        //            object2.transform.parent = object1Group;
        //        }
        //        else
        //        {
        //            object2.transform.parent = object1Group;
        //            if (player.GetCurrentMovingObject() != null)
        //            {
        //                if (object1 == player.GetCurrentMovingObject())
        //                {
        //                    object1.transform.parent = electrifiables.transform;
        //                }
        //                else
        //                {
        //                    object1.transform.parent = object1Group;
        //                }
        //            }
        //            else
        //            {
        //                object1.transform.parent = object1Group;
        //            }
        //        }

        //        if (object1Group.childCount == 1)
        //        {
        //            object1Group.GetChild(0).transform.parent = electrifiables.transform;
        //            sortingGroups = false;
        //        }
        //        else if (object1Group.childCount < 1)
        //        {
        //            Destroy(object1Group.gameObject);
        //            sortingGroups = false;
        //        }
        //        else
        //        {
        //            //currentObject = object1.transform.GetChild(0).gameObject;
        //        }
        //        for (int i = 0; i < electrifiables.transform.childCount; i++)
        //        {
        //            if (electrifiables.transform.GetChild(i).transform.childCount < 1)
        //            {
        //                Destroy(electrifiables.transform.GetChild(i).gameObject);
        //                break;
        //            }
        //        }


        //        if (sortingGroups)
        //        {
        //            //Add separate code that works with what is left
        //        }

        //    }
        //}

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

        //GameObject electrifiableGroup = new GameObject("ElectrifiableGroup");
        //Debug.LogError("Disconnecting");
        //Debug.LogError("Object 1:" + object1.name);
        //Debug.LogError("Object 2:" + object2.name);

        //if (object1.transform.parent == object2.transform.parent)
        //{
        //    //object1 not connected to other objects than object2
        //    if (object1.GetComponent<IElectrifiable>().GetConnectedObjects().Count <= 1)
        //{
        //    object1.transform.parent = electrifiables.transform;
        //}
        ////object2 not connected to other objects than object1
        //else if (object2.GetComponent<IElectrifiable>().GetConnectedObjects().Count <= 1)
        //{
        //    object2.transform.parent = electrifiables.transform;
        //}
        ////both object are connected to other objects
        //else
        //{

        //        //new electrifiableGroup
        //        GameObject electrifiableGroup = new GameObject("ElectrifiableGroup");
        //        electrifiableGroup.transform.parent = electrifiables.transform;


        //        GameObject currentGameObject = object2;

        //        bool stillConnected = false;
        //        bool stillSearching = true;

        //        List<GameObject> tempConnectedTo2GameObjects = new List<GameObject>();
        //        tempConnectedTo2GameObjects = currentGameObject.GetComponent<IElectrifiable>().GetConnectedObjects();

        //        while (stillSearching)
        //        {
        //            tempConnectedTo2GameObjects = currentGameObject.GetComponent<IElectrifiable>().GetConnectedObjects();
        //            stillSearching = false;
        //            for (int i = 0; i < tempConnectedTo2GameObjects.Count; i++)
        //            {
        //                if (tempConnectedTo2GameObjects[i] == object1)
        //                {
        //                    if (currentGameObject != object2)
        //                    {
        //                        stillConnected = true;
        //                        stillSearching = false;
        //                    }
        //                }
        //                else if (tempConnectedTo2GameObjects[i] != object2)
        //                {
        //                    tempConnectedTo2GameObjects[i].transform.parent = electrifiableGroup.transform;
        //                    currentGameObject = tempConnectedTo2GameObjects[i];
        //                    stillSearching = true;
        //                }

        //            }
        //            tempConnectedTo2GameObjects.Clear();
        //            if (object1.transform.parent != object2.transform.parent && object1 != object2)
        //            {
        //                Transform parentObject2 = object2.transform.parent.gameObject.transform;

        //                while (parentObject2.childCount > 0)
        //                {
        //                    for (int i = 0; i < parentObject2.childCount; i++)
        //                    {
        //                        parentObject2.GetChild(i).parent = object1.transform.parent;
        //                    }
        //                }

        //                //object2.transform.parent = object1.transform.parent;
        //                for (int i = 0; i < electrifiables.transform.childCount; i++)
        //                {
        //                    if (electrifiables.transform.GetChild(i).transform.childCount < 1)
        //                    {
        //                        Destroy(electrifiables.transform.GetChild(i).gameObject);
        //                        break;
        //                    }
        //                }
        //            }

        //        }
        //    }








        //bool stillConnected = false; //to keep track if object 1 and 2 are still connected 
        //bool atEnd = false; //the connection loop is ending
        //bool stopExit = false; //to stop exiting if there are more connections
        //GameObject currentGameObject = object2; // current object being checked for connections

        //while (!atEnd)
        //{
        //    Debug.Log("Stuck In Disconnect");

        //    //a list of connected 
        //    List<GameObject> tempConnectedTo2GameObjects = new List<GameObject>();
        //    tempConnectedTo2GameObjects = currentGameObject.GetComponent<IElectrifiable>().GetConnectedObjects();

        //    if (tempConnectedTo2GameObjects.Count > 0)
        //    {
        //        for (int k = 0; k < tempConnectedTo2GameObjects.Count; k++)
        //        {
        //            if (tempConnectedTo2GameObjects[k] == object1)
        //            {
        //                if (currentGameObject != object2)
        //                {
        //                    stillConnected = true;
        //                    break;
        //                }
        //            }
        //            else if (tempConnectedTo2GameObjects[k] != object2 && tempConnectedTo2GameObjects[k] != currentGameObject)
        //            {

        //                currentGameObject.transform.parent = electrifiableGroup.transform;
        //                currentGameObject = tempConnectedTo2GameObjects[k];
        //                stopExit = true;
        //                break;

        //            }
        //        }
        //    }
        //    else
        //    {
        //        atEnd = true;
        //    }
        //    if (stopExit)
        //    {
        //        stopExit = false;
        //    }
        //    else
        //    {
        //        atEnd = true;
        //        if (!stillConnected)
        //        {
        //            currentGameObject.transform.parent = electrifiableGroup.transform;
        //        }
        //    }
        //    tempConnectedTo2GameObjects.Clear();
        //}
        //object2.transform.parent = electrifiableGroup.transform;
        //if (stillConnected)
        //{
        //    if (object1.transform.parent != object2.transform.parent && object1 != object2)
        //    {
        //        Transform parentObject2 = object2.transform.parent.gameObject.transform;

        //        while (parentObject2.childCount > 0)
        //        {
        //            for (int i = 0; i < parentObject2.childCount; i++)
        //            {
        //                parentObject2.GetChild(i).parent = object1.transform.parent;
        //            }
        //        }

        //        //object2.transform.parent = object1.transform.parent;
        //        for (int i = 0; i < electrifiables.transform.childCount; i++)
        //        {
        //            if (electrifiables.transform.GetChild(i).transform.childCount < 1)
        //            {
        //                Destroy(electrifiables.transform.GetChild(i).gameObject);
        //                break;
        //            }
        //        }
        //    }
        //}







        //    if (electrifiables == null)
        //    {
        //        electrifiables = GameObject.Find("Electrifiables");
        //    }

        //    //object1 is metal
        //    if (object1.GetComponent<Collider2D>().CompareTag("Metal"))
        //    {
        //        //object2 is metal
        //        if (object2.GetComponent<Collider2D>().CompareTag("Metal"))
        //        {
        //            //object1 is metal and is not in a group  
        //            if (object1.transform.parent == electrifiables.transform)
        //            {
        //                //object2 is metal and is not in a group  
        //                if (object2.transform.parent == electrifiables.transform)
        //                {
        //                    GameObject electrifiableGroup = new GameObject("ElectrifiableGroup");
        //                    electrifiableGroup.transform.parent = electrifiables.transform;
        //                    object1.transform.parent = electrifiableGroup.transform;
        //                    object2.transform.parent = electrifiableGroup.transform;
        //                }
        //                //object2 is metal and is in a group  
        //                else
        //                {
        //                    object1.transform.parent = object2.transform.parent;
        //                }
        //            }
        //            //object1 is metal and is in a group  
        //            else
        //            {
        //                //object2 is metal and is not in a group  
        //                if (object2.transform.parent == electrifiables.transform)
        //                {
        //                    object2.transform.parent = object1.transform.parent;
        //                }
        //                //object2 is metal and is in a group  
        //                else
        //                {
        //                    //Debug.LogWarning("Actual Object 1:" + object1.name);
        //                    //Debug.LogWarning("Actual Object 2:" + object2.name);

        //                    //foreach (Transform obj in object1.transform.parent.gameObject.transform)
        //                    //{
        //                    //    Debug.Log("Object 1: " + obj.gameObject.name);
        //                    //}

        //                    //Debug.LogWarning("Actual Object 2:" + object2.name);
        //                    //foreach (Transform obj in object2.transform.parent.gameObject.transform)
        //                    //{
        //                    //    Debug.Log("Object 2: " + obj.gameObject.name);
        //                    //}

        //                    if (object1.transform.parent != object2.transform.parent && object1 != object2)
        //                    {
        //                        Transform parentObject2 = object2.transform.parent.gameObject.transform;

        //                        while (parentObject2.childCount > 0)
        //                        {
        //                            for (int i = 0; i < parentObject2.childCount; i++)
        //                            {
        //                                parentObject2.GetChild(i).parent = object1.transform.parent;
        //                            }
        //                        }

        //                        //object2.transform.parent = object1.transform.parent;
        //                    }
        //                }
        //            }
        //        }
        //        //object2 is water
        //        else if (object2.GetComponent<Collider2D>().CompareTag("Water"))
        //        {
        //            //object1 is metal and is not in a group  
        //            if (object1.transform.parent == electrifiables.transform)
        //            {
        //                //object2 is water and is not in a group  
        //                if (object2.transform.parent.parent == electrifiables.transform)
        //                {
        //                    GameObject electrifiableGroup = new GameObject("ElectrifiableGroup");
        //                    electrifiableGroup.transform.parent = electrifiables.transform;
        //                    object1.transform.parent = electrifiableGroup.transform;
        //                    object2.transform.parent.parent = electrifiableGroup.transform;
        //                }
        //                //object2 is water and is in a group  
        //                else
        //                {
        //                    object1.transform.parent = object2.transform.parent.parent;
        //                }
        //            }
        //            //object1 is metal and is in a group 
        //            else
        //            {
        //                //object2 is water and is not in a group  
        //                if (object2.transform.parent == electrifiables.transform)
        //                {
        //                    object2.transform.parent.parent = object1.transform.parent;
        //                }
        //                //object2 is water and is in a group  
        //                else
        //                {
        //                    //Debug.LogWarning("Actual Object 1:" + object1.name);
        //                    //Debug.LogWarning("Actual Object 2:" + object2.name);

        //                    //foreach (Transform obj in object1.transform.parent.gameObject.transform)
        //                    //{
        //                    //    Debug.Log("Object 1: " + obj.gameObject.name);
        //                    //}

        //                    //Debug.LogWarning("Actual Object 2:" + object2.name);
        //                    //foreach (Transform obj in object2.transform.parent.gameObject.transform)
        //                    //{
        //                    //    Debug.Log("Object 2: " + obj.gameObject.name);
        //                    //}

        //                    if (object1.transform.parent != object2.transform.parent.parent && object1 != object2)
        //                    {
        //                        Transform parentObject2 = object2.transform.parent.parent.gameObject.transform;
        //                        while (parentObject2.childCount > 0)
        //                        {
        //                            for (int i = 0; i < parentObject2.childCount; i++)
        //                            {
        //                                parentObject2.GetChild(i).parent = object1.transform.parent;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    //object1 is water
        //    if (object1.GetComponent<Collider2D>().CompareTag("Water"))
        //    {
        //        //object2 is metal
        //        if (object2.GetComponent<Collider2D>().CompareTag("Metal"))
        //        {
        //            //object1 is water and is not in a group  
        //            if (object1.transform.parent.parent == electrifiables.transform)
        //            {
        //                //object2 is metal and is not in a group  
        //                if (object2.transform.parent == electrifiables.transform)
        //                {
        //                    GameObject electrifiableGroup = new GameObject("ElectrifiableGroup");
        //                    electrifiableGroup.transform.parent = electrifiables.transform;
        //                    object1.transform.parent.parent = electrifiableGroup.transform;
        //                    object2.transform.parent = electrifiableGroup.transform;
        //                }
        //                //object2 is metal and is in a group  
        //                else
        //                {
        //                    object1.transform.parent.parent = object2.transform.parent;
        //                }
        //            }
        //            //object1 is water and is in a group  
        //            else
        //            {
        //                //object2 is metal and is not in a group  
        //                if (object2.transform.parent == electrifiables.transform)
        //                {
        //                    object2.transform.parent = object1.transform.parent.parent;
        //                }
        //                //object2 is metal and is in a group  
        //                else
        //                {
        //                    //Debug.LogWarning("Actual Object 1:" + object1.name);
        //                    //Debug.LogWarning("Actual Object 2:" + object2.name);

        //                    //foreach (Transform obj in object1.transform.parent.gameObject.transform)
        //                    //{
        //                    //    Debug.Log("Object 1: " + obj.gameObject.name);
        //                    //}

        //                    //Debug.LogWarning("Actual Object 2:" + object2.name);
        //                    //foreach (Transform obj in object2.transform.parent.gameObject.transform)
        //                    //{
        //                    //    Debug.Log("Object 2: " + obj.gameObject.name);
        //                    //}

        //                    if (object1.transform.parent.parent != object2.transform.parent && object1 != object2)
        //                    {
        //                        Transform parentObject2 = object2.transform.parent.gameObject.transform;

        //                        while (parentObject2.childCount > 0)
        //                        {
        //                            for (int i = 0; i < parentObject2.childCount; i++)
        //                            {
        //                                parentObject2.GetChild(i).parent = object1.transform.parent.parent;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        //object2 is water
        //        else if (object2.GetComponent<Collider2D>().CompareTag("Water"))
        //        {
        //            //object1 is water and is not in a group  
        //            if (object1.transform.parent.parent == electrifiables.transform)
        //            {
        //                //object2 is water and is not in a group  
        //                if (object2.transform.parent.parent == electrifiables.transform)
        //                {
        //                    GameObject electrifiableGroup = new GameObject("ElectrifiableGroup");
        //                    electrifiableGroup.transform.parent = electrifiables.transform;
        //                    object1.transform.parent.parent = electrifiableGroup.transform;
        //                    object2.transform.parent.parent = electrifiableGroup.transform;
        //                }
        //                //object2 is water and is in a group  
        //                else
        //                {
        //                    object1.transform.parent.parent = object2.transform.parent.parent;
        //                }
        //            }
        //            //object1 is water and is in a group 
        //            else
        //            {
        //                //object2 is water and is not in a group  
        //                if (object2.transform.parent == electrifiables.transform)
        //                {
        //                    object2.transform.parent.parent = object1.transform.parent.parent;
        //                }
        //                //object2 is water and is in a group  
        //                else
        //                {
        //                    //Debug.LogWarning("Actual Object 1:" + object1.name);
        //                    //Debug.LogWarning("Actual Object 2:" + object2.name);

        //                    //foreach (Transform obj in object1.transform.parent.gameObject.transform)
        //                    //{
        //                    //    Debug.Log("Object 1: " + obj.gameObject.name);
        //                    //}

        //                    //Debug.LogWarning("Actual Object 2:" + object2.name);
        //                    //foreach (Transform obj in object2.transform.parent.gameObject.transform)
        //                    //{
        //                    //    Debug.Log("Object 2: " + obj.gameObject.name);
        //                    //}

        //                    if (object1.transform.parent.parent != object2.transform.parent.parent && object1 != object2)
        //                    {
        //                        Transform parentObject2 = object2.transform.parent.parent.gameObject.transform;
        //                        while (parentObject2.childCount > 0)
        //                        {
        //                            for (int i = 0; i < parentObject2.childCount; i++)
        //                            {
        //                                parentObject2.GetChild(i).parent = object1.transform.parent.parent;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("There is a problem");
        //    }
        //    for (int i = 0; i < electrifiables.transform.childCount; i++)
        //    {
        //        if (electrifiables.transform.GetChild(i).transform.childCount < 1)
        //        {
        //            Destroy(electrifiables.transform.GetChild(i).gameObject);
        //            break;
        //        }
        //    }

        //}


        //----------------------------------------------------------------
        //----------------------------------------------------------------
        //----------------------------------------------------------------


        ////object2 is not in a group
        //    if (object2.transform.parent == electrifiables.transform)
        //    {
        //        if (object1.GetComponent<Collider2D>().CompareTag("Water"))
        //        {

        //        }
        //        else
        //        {

        //        }
        //        if (object2.GetComponent<Collider2D>().CompareTag("Water"))
        //        {

        //        }
        //        else
        //        {

        //        }
        //    }
        //    //object2 is in a group
        //    else
        //    {

        //    }
        //}
        ////object1 is in a group
        //else
        //{

        //}

        ////object2 is not in a group but is metal
        //    if (object2.transform.parent == metalMain.transform)
        //    {
        //        GameObject metalGroup = new GameObject("MetalGroup");
        //        metalGroup.transform.parent = metalMain.transform;
        //        object1.transform.parent = metalGroup.transform;
        //        object2.transform.parent = metalGroup.transform;
        //    }
        //    //object 2 is water
        //    else if (object2.transform.parent.parent == waterMain.transform)
        //    {
        //        //connect water
        //        List<GameObject> waterConnectedObjects = new List<GameObject>();
        //        waterConnectedObjects = object2.transform.gameObject.GetComponent<Water>().GetConnectedObjects();

        //        foreach (GameObject obj in waterConnectedObjects)
        //        {
        //            //if an object connected to the water is connected to other metal
        //            if (obj.transform.parent.parent != waterMain && obj.transform.parent != metalMain)
        //            {
        //                object1.transform.parent = obj.transform.parent;
        //                break;
        //            }
        //            if (obj.transform.parent.parent != waterMain && obj.transform.parent == metalMain)
        //            {
        //                GameObject metalGroup = new GameObject("MetalGroup");
        //                metalGroup.transform.parent = metalMain.transform;
        //                object1.transform.parent = metalGroup.transform;
        //                obj.transform.parent = metalGroup.transform;
        //                break;
        //            }
        //        }
        //    }
        //    //object2 is in a group
        //    else
        //    {
        //        object1.transform.parent = object2.transform.parent;
        //    }
        //}
        ////object 1 is water
        //else if (object1.transform.parent.parent == waterMain.transform)
        //{
        //    //object2 is not in a group but is metal
        //    if (object2.transform.parent == metalMain.transform)
        //    {
        //        //connect water
        //        List<GameObject> waterConnectedObjects = new List<GameObject>();
        //        waterConnectedObjects = object1.transform.gameObject.GetComponent<Water>().GetConnectedObjects();
        //        foreach (GameObject obj in waterConnectedObjects)
        //        {
        //            //if an object connected to the water is connected to other metal
        //            if (obj.transform.parent.parent != waterMain && obj.transform.parent != metalMain)
        //            {
        //                object2.transform.parent = obj.transform.parent;
        //                break;
        //            }
        //            if (obj.transform.parent.parent != waterMain && obj.transform.parent == metalMain)
        //            {
        //                GameObject metalGroup = new GameObject("MetalGroup");
        //                metalGroup.transform.parent = metalMain.transform;
        //                object2.transform.parent = metalGroup.transform;
        //                obj.transform.parent = metalGroup.transform;
        //                break;
        //            }
        //        }
        //    }
        //    //object 2 is water
        //    else if (object2.transform.parent.parent == waterMain.transform)
        //    {
        //        //already connected
        //    }
        //    //object2 is in a group
        //    else
        //    {
        //        //connect water
        //        List<GameObject> waterConnectedObjects = new List<GameObject>();
        //        waterConnectedObjects = object1.transform.gameObject.GetComponent<Water>().GetConnectedObjects();
        //        foreach (GameObject obj in waterConnectedObjects)
        //        {
        //            //if an object connected to the water is connected to other metal
        //            if (obj.transform.parent.parent != waterMain )
        //            {
        //                obj.transform.parent = object2.transform.parent;

        //            }
        //        }
        //    }
        //}
        ////object1 is in a group
        //else
        //{
        //    //object2 is not in a group
        //    if (object2.transform.parent == metalMain.transform)
        //    {

        //        object2.transform.parent = object1.transform.parent;
        //    }
        //    //object 2 is water
        //    else if (object2.transform.parent.parent == waterMain.transform)
        //    {
        //        //connect water
        //        List<GameObject> waterConnectedObjects = new List<GameObject>();
        //        waterConnectedObjects = object2.transform.gameObject.GetComponent<Water>().GetConnectedObjects();
        //        foreach (GameObject obj in waterConnectedObjects)
        //        {
        //            //if an object connected to the water is connected to other metal
        //            if (obj.transform.parent.parent != waterMain)
        //            {
        //                obj.transform.parent = object1.transform.parent;

        //            }
        //        }
        //    }
        //    //object2 is in a group
        //    else
        //    {
        //        //Debug.LogWarning("Actual Object 1:" + object1.name);

        //        //foreach (Transform obj in object1.transform.parent.gameObject.transform)
        //        //{
        //        //    Debug.Log("Object 1: " + obj.gameObject.name);
        //        //}

        //        //Debug.LogWarning("Actual Object 2:" + object2.name);
        //        //foreach (Transform obj in object2.transform.parent.gameObject.transform)
        //        //{
        //        //    Debug.Log("Object 2: " + obj.gameObject.name);
        //        //}

        //        if (object1.transform.parent != object2.transform.parent && object1 != object2)
        //        {
        //            Transform parentObject2 = object2.transform.parent.gameObject.transform;

        //            while (parentObject2.childCount > 0)
        //            {
        //                for (int i = 0; i < parentObject2.childCount; i++)
        //                {
        //                    parentObject2.GetChild(i).parent = object1.transform.parent;
        //                }
        //            }

        //            //object2.transform.parent = object1.transform.parent;
        //        }
        //    }
        //}
        //for (int i = 0; i < metalMain.transform.childCount; i++)
        //{
        //    if (metalMain.transform.GetChild(i).transform.childCount < 1)
        //    {
        //        Destroy(metalMain.transform.GetChild(i).gameObject);
        //        break;
        //    }
        //}


        //public void ConnectObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1, int groupNum1,
        //    GameObject object2, Collider2D gameObjectCollider2, bool electricState2, int groupNum2)
        //{
        //    Debug.Log(connectedGameObjects.)
        //    //neither object in a group so create a new one
        //    if (groupNum1 == 0 && groupNum2 == 0)
        //    {
        //        //Debug.LogWarning("NEITHER OBJECT GROUPED");
        //        if (electricState1)
        //        {
        //            object2.GetComponent<IElectrifiable>().SetElectrified(true);
        //        }
        //        else if (electricState2)
        //        {
        //            object1.GetComponent<IElectrifiable>().SetElectrified(true);
        //        }
        //        object1.GetComponent<IElectrifiable>().SetGroupNum(availableNum);
        //        object2.GetComponent<IElectrifiable>().SetGroupNum(availableNum);
        //        availableNum++;
        //        connectedGameObjects.Add(object1);
        //        connectedGameObjects.Add(object2);
        //    }
        //    //both in the same group
        //    else if (groupNum1 == groupNum2)
        //    {
        //        //nothing should be needed here
        //    }
        //    //both in separate groups
        //    else if (groupNum1 > 0 && groupNum2 > 0)
        //    {
        //        //Debug.LogWarning("BOTH OBJECTS GROUPED SEPARATELY");
        //        for (int i = 0; i < connectedGameObjects.Count; i++)
        //        {
        //            //going through to find each group 2 object to set it to group 1 and setting electricity
        //            if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum2)
        //            {
        //                connectedGameObjects[i].GetComponent<IElectrifiable>().SetGroupNum(groupNum1);
        //                if (electricState1 && !electricState2)
        //                {
        //                    connectedGameObjects[i].GetComponent<IElectrifiable>().SetElectrified(true);
        //                }
        //            }
        //            //checks each group 1 object to set electricity
        //            if (!electricState1 && electricState2)
        //            {
        //                if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum1)
        //                {
        //                    connectedGameObjects[i].GetComponent<IElectrifiable>().SetElectrified(true);
        //                }
        //            }
        //        }
        //    }
        //    //the first object is in a group but the second one is not
        //    else if (groupNum1 > 0)
        //    {
        //        //Debug.LogWarning("FIRST OBJECT GROUPED AND SECOND OBJECT NOT");
        //        // electrifies object 2 because object 1 is electrified
        //        if (electricState1 && !electricState2)
        //        {
        //            object2.GetComponent<IElectrifiable>().SetElectrified(true);
        //        }
        //        // electrifies groupNum1 objects because object 2 is electrified
        //        else if (!electricState1 && electricState2)
        //        {
        //            for (int i = 0; i < connectedGameObjects.Count; i++)
        //            {
        //                if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum1)
        //                {
        //                    connectedGameObjects[i].GetComponent<IElectrifiable>().SetElectrified(true);
        //                }
        //            }
        //        }
        //        //sets object 2 to groupNum1
        //        object2.GetComponent<IElectrifiable>().SetGroupNum(groupNum1);
        //        connectedGameObjects.Add(object2);
        //    }
        //    //second object is in a group but first is not
        //    else if (groupNum2 > 0)
        //    {
        //        //Debug.LogWarning("SECOND OBJECT GROUPED AND FIRST OBJECT NOT");
        //        if (!electricState1 && electricState2)
        //        {
        //            object1.GetComponent<IElectrifiable>().SetElectrified(true);
        //        }
        //        else if (electricState1 && !electricState2)
        //        {
        //            for (int i = 0; i < connectedGameObjects.Count; i++)
        //            {
        //                if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum2)
        //                {
        //                    connectedGameObjects[i].GetComponent<IElectrifiable>().SetElectrified(true);
        //                }
        //            }
        //        }
        //        object1.GetComponent<IElectrifiable>().SetGroupNum(groupNum2);
        //        connectedGameObjects.Add(object1);
        //    }
        //    else
        //    {
        //        //Debug.LogError("Shouldn't reach here when connecting");
        //    }

        //    List<GameObject> tempList = new List<GameObject>(connectedGameObjects);
        //    connectedGameObjects = tempList.Distinct().ToList();

        //    //int listLength = connectedGameObjects.Count;
        //    //int counter = 0;

        //    //while (counter < listLength)
        //    //{
        //    //    if (connectedGameObjects[counter].GetComponent<IElectrifiable>().GetGroupNum() == 0)
        //    //    {
        //    //        connectedGameObjects.RemoveAt(counter);
        //    //        listLength--;
        //    //    }
        //    //    else
        //    //    {
        //    //        counter++;
        //    //    }
        //    //}

        //    //Debugging
        //    //if (debugging)
        //    //{
        //    //    Debug.Log("Connected List");
        //    //    Debug.Log("--------------------");
        //    //    Debug.Log("--------------------");
        //    //    for (int i = 0; i < connectedGameObjects.Count; i++)
        //    //    {
        //    //        Debug.Log("Object: " + connectedGameObjects[i]);
        //    //        Debug.Log("Objects Group: " + connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum());
        //    //        Debug.Log("--------------------");
        //    //    }
        //    //    Debug.Log("--------------------");
        //    //}
        //}
        //    public void DisconnectObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1, int groupNum1,
        //    GameObject object2, Collider2D gameObjectCollider2, bool electricState2, int groupNum2)
        //{
        //    if (groupNum1 == groupNum2)
        //    {

        //        //Debug.LogWarning("FIRST TIME DISCONNECTING");
        //        //lists for the directly connected objects to each object
        //        List<GameObject> tempConnectedGameObjects1 = new List<GameObject>();
        //        List<GameObject> tempConnectedGameObjects2 = new List<GameObject>();

        //        tempConnectedGameObjects1 = object1.GetComponent<IElectrifiable>().GetConnectedObjects();
        //        tempConnectedGameObjects2 = object2.GetComponent<IElectrifiable>().GetConnectedObjects();

        //        bool objectsStillConnected = false; // set to true if objects are still connected later
        //        /*
        //         go through objects connected to object 1 and 2 and see if they have any that are still connected
        //         to each other and object 1 and 2
        //        */
        //        for (int i = 0; i < tempConnectedGameObjects1.Count; i++)
        //        {
        //            for (int k = 0; k < tempConnectedGameObjects2.Count; k++)
        //            {
        //                if (tempConnectedGameObjects1[i] == tempConnectedGameObjects2[k])
        //                {
        //                    objectsStillConnected = true;
        //                }
        //            }
        //        }
        //        // if the object are not still connected through other objects
        //        if (!objectsStillConnected)
        //        {
        //            //everything was electrified
        //            if (electricState1)
        //            {
        //                //set if the objects are still connected to water therefore still electrified
        //                bool isWater1 = false;
        //                bool isWater2 = false;
        //                if (object1.CompareTag("Water"))
        //                {
        //                    isWater1 = true;
        //                }
        //                else
        //                {
        //                    for (int i = 0; i < tempConnectedGameObjects1.Count; i++)
        //                    {
        //                        if (tempConnectedGameObjects1[i] != object2)
        //                        {
        //                            if (tempConnectedGameObjects1[i].CompareTag("Water"))
        //                            {
        //                                isWater1 = true;
        //                            }
        //                        }
        //                    }
        //                }

        //                if (object2.CompareTag("Water"))
        //                {
        //                    isWater2 = true;
        //                }
        //                else
        //                {
        //                    for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
        //                    {
        //                        if (tempConnectedGameObjects2[i] != object1)
        //                        {
        //                            if (tempConnectedGameObjects2[i].CompareTag("Water"))
        //                            {
        //                                isWater2 = true;
        //                            }
        //                        }
        //                    }
        //                }

        //                /*
        //                 if the objects are no longer connected to water set electrified false for the
        //                 object and all directly connected objects
        //                */
        //                if (!isWater1)
        //                {
        //                    for (int i = 0; i < tempConnectedGameObjects1.Count; i++)
        //                    {
        //                        if (tempConnectedGameObjects1[i] != object2)
        //                        {
        //                            tempConnectedGameObjects1[i].GetComponent<IElectrifiable>().SetElectrified(false);
        //                        }
        //                    }
        //                    object1.GetComponent<IElectrifiable>().SetElectrified(false);
        //                }
        //                if (!isWater2)
        //                {
        //                    for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
        //                    {
        //                        if (tempConnectedGameObjects2[i] != object1)
        //                        {
        //                            tempConnectedGameObjects2[i].GetComponent<IElectrifiable>().SetElectrified(false);
        //                        }
        //                    }
        //                    object2.GetComponent<IElectrifiable>().SetElectrified(false);
        //                }

        //                //checks if the objects are still in groups after disconnecting
        //                bool objectGrouped1 = false;
        //                bool objectGrouped2 = false;
        //                if (tempConnectedGameObjects1.Count <= 1)
        //                {
        //                    int removePos = -1;
        //                    object1.GetComponent<IElectrifiable>().SetGroupNum(0);
        //                    for (int i = 0; i < connectedGameObjects.Count; i++)
        //                    {
        //                        if (connectedGameObjects[i] == object1)
        //                        {
        //                            removePos = i;
        //                        }
        //                    }

        //                    if (removePos > -1)
        //                    {
        //                        connectedGameObjects.RemoveAt(removePos);
        //                    }
        //                    else
        //                    {
        //                        //Debug.LogError("The pos was out of range");
        //                    }
        //                }
        //                else
        //                {
        //                    objectGrouped1 = true;
        //                }
        //                if (tempConnectedGameObjects2.Count <= 1)
        //                {
        //                    int removePos = -1;
        //                    object2.GetComponent<IElectrifiable>().SetGroupNum(0);
        //                    for (int i = 0; i < connectedGameObjects.Count; i++)
        //                    {
        //                        if (connectedGameObjects[i] == object2)
        //                        {
        //                            removePos = i;
        //                        }
        //                    }
        //                    if (removePos > -1)
        //                    {
        //                        connectedGameObjects.RemoveAt(removePos);
        //                    }
        //                    else
        //                    {
        //                        //Debug.LogError("The pos was out of range");
        //                    }
        //                }
        //                else
        //                {
        //                    objectGrouped2 = true;
        //                }
        //                //if both objects are still in groups after disconnecting create a new group
        //                if (objectGrouped1 && objectGrouped2)
        //                {
        //                    for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
        //                    {
        //                        if (tempConnectedGameObjects2[i] != object1)
        //                        {
        //                            tempConnectedGameObjects2[i].GetComponent<IElectrifiable>().SetGroupNum(availableNum);
        //                        }
        //                    }
        //                    object2.GetComponent<IElectrifiable>().SetGroupNum(availableNum);
        //                    availableNum++;
        //                }

        //            }
        //            //nothing is electrified
        //            else
        //            {
        //                bool objectGrouped1 = false;
        //                bool objectGrouped2 = false;
        //                if (tempConnectedGameObjects1.Count <= 1)
        //                {
        //                    object1.GetComponent<IElectrifiable>().SetGroupNum(0);
        //                    int removePos = -1;
        //                    for (int i = 0; i < connectedGameObjects.Count; i++)
        //                    {
        //                        if (connectedGameObjects[i] == object1)
        //                        {
        //                            removePos = i;
        //                        }
        //                    }
        //                    if (removePos > -1)
        //                    {
        //                        connectedGameObjects.RemoveAt(removePos);
        //                    }
        //                    else
        //                    {
        //                        //Debug.LogError("The pos was out of range");
        //                    }
        //                    //should maybe remove from list here
        //                }
        //                else
        //                {
        //                    objectGrouped1 = true;
        //                }
        //                if (tempConnectedGameObjects2.Count <= 1)
        //                {
        //                    object2.GetComponent<IElectrifiable>().SetGroupNum(0);
        //                    int removePos = -1;
        //                    for (int i = 0; i < connectedGameObjects.Count; i++)
        //                    {
        //                        if (connectedGameObjects[i] == object2)
        //                        {
        //                            removePos = i;
        //                        }
        //                    }
        //                    if (removePos > -1)
        //                    {
        //                        connectedGameObjects.RemoveAt(removePos);
        //                    }
        //                    else
        //                    {
        //                        //Debug.LogError("The pos was out of range");
        //                    }
        //                    //should maybe remove from list here
        //                }
        //                else
        //                {
        //                    objectGrouped2 = true;
        //                }
        //                if (objectGrouped1 && objectGrouped2)
        //                {
        //                    bool atEnd = false;
        //                    bool exitEarly = false;
        //                    GameObject currentGameObject = object2;
        //                    while (!atEnd)
        //                    {
        //                        List<GameObject> tempConnectedTo2GameObjects = new List<GameObject>();
        //                        tempConnectedTo2GameObjects = currentGameObject.GetComponent<IElectrifiable>().GetConnectedObjects();

        //                        if (tempConnectedTo2GameObjects.Count > 0)
        //                        {
        //                            for (int k = 0; k < tempConnectedTo2GameObjects.Count; k++)
        //                            {
        //                                if (tempConnectedTo2GameObjects[k] != object1 &&
        //                                    tempConnectedTo2GameObjects[k] != object2)
        //                                {
        //                                    if (tempConnectedTo2GameObjects[k].GetComponent<IElectrifiable>().GetGroupNum() != availableNum)
        //                                    {
        //                                        tempConnectedTo2GameObjects[k].GetComponent<IElectrifiable>().SetGroupNum(availableNum);
        //                                        currentGameObject = tempConnectedTo2GameObjects[k];
        //                                        exitEarly = true;
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        if (exitEarly)
        //                        {
        //                            exitEarly = false;
        //                        }
        //                        else
        //                        {
        //                            atEnd = true;
        //                            currentGameObject.GetComponent<IElectrifiable>().SetGroupNum(availableNum);
        //                        }
        //                    }

        //                    //for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
        //                    //{
        //                    //    if (tempConnectedGameObjects2[i] != object1)
        //                    //    {
        //                    //        List<GameObject> tempConnectedTo2GameObjects = new List<GameObject>();
        //                    //        tempConnectedTo2GameObjects = tempConnectedGameObjects2[i].GetComponent<IElectrifiable>().GetConnectedObjects();


        //                    //    }
        //                    //}

        //                    object2.GetComponent<IElectrifiable>().SetGroupNum(availableNum);
        //                    availableNum++;
        //                    //for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
        //                    //{
        //                    //    if (tempConnectedGameObjects2[i] != object1)
        //                    //    {
        //                    //        List<GameObject> tempConnectedTo2GameObjects = new List<GameObject>();
        //                    //        tempConnectedTo2GameObjects = tempConnectedGameObjects2[i].GetComponent<IElectrifiable>().GetConnectedObjects();
        //                    //        if (tempConnectedTo2GameObjects.Count > 0)
        //                    //        {
        //                    //            for (int k = 0; k < tempConnectedTo2GameObjects.Count; k++)
        //                    //            {
        //                    //                if (tempConnectedTo2GameObjects[k] != object1 &&
        //                    //                    tempConnectedTo2GameObjects[k] != object2)
        //                    //                {
        //                    //                    tempConnectedTo2GameObjects[k].GetComponent<IElectrifiable>().SetGroupNum(availableNum);
        //                    //                }
        //                    //            }
        //                    //        }
        //                    //        tempConnectedGameObjects2[i].GetComponent<IElectrifiable>().SetGroupNum(availableNum);
        //                    //    }
        //                    //}

        //                }
        //            }
        //        }
        //        //for (int i = 0; i < connectedGameObjects.Count; i++)
        //        //{
        //        //    if (connectedGameObjects[i] != object1)
        //        //    {
        //        //        if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum1)
        //        //        {
        //        //            for (int k = 0; k < tempConnectedGameObjects1.Count; k++)
        //        //            {
        //        //                if 
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //    }
        //    else
        //    {
        //        //Debug.LogError("These objects should have been connected");
        //        //Debug.LogError("Object 1: " + object1);
        //        //Debug.LogError("Object 1 Group: " + object1.GetComponent<IElectrifiable>().GetGroupNum());
        //        //Debug.LogError("Object 2: " + object2);
        //        //Debug.LogError("Object 2 Group: " + object2.GetComponent<IElectrifiable>().GetGroupNum());
        //    }
        //    int listLength = connectedGameObjects.Count;
        //    int counter = 0;

        //    //while (counter < listLength)
        //    //{
        //    //    if (connectedGameObjects[counter].GetComponent<IElectrifiable>().GetGroupNum() == 0)
        //    //    {
        //    //        connectedGameObjects.RemoveAt(counter);
        //    //        listLength--;
        //    //    }
        //    //    else
        //    //    {
        //    //        counter++;
        //    //    }
        //    //}

        //}

        //    public void ElectrifyConnectedObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1, int groupNum1)
        //{
        //    if (!electricState1)
        //    {
        //        bool isWater = object1.CompareTag("Water");
        //        if (!isWater)
        //        {
        //            for (int i = 0; i < connectedGameObjects.Count; i++)
        //            {
        //                if (connectedGameObjects[i] != null)
        //                {
        //                    if (connectedGameObjects[i] != null)
        //                    {
        //                        if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum1)
        //                        {
        //                            if (connectedGameObjects[i].CompareTag("Water"))
        //                            {
        //                                isWater = true;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (isWater)
        //        {
        //            if (groupNum1 == 0)
        //            {
        //                object1.GetComponent<IElectrifiable>().SetElectrified(true);
        //            }
        //            else
        //            {
        //                for (int i = 0; i < connectedGameObjects.Count; i++)
        //                {
        //                    if (connectedGameObjects[i] != null)
        //                    {
        //                        if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum1)
        //                        {
        //                            connectedGameObjects[i].GetComponent<IElectrifiable>().SetElectrified(true);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    