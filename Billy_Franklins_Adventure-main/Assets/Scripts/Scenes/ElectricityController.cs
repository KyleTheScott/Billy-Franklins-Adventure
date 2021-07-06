using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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


    [SerializeField] private List<GameObject> connectedGameObjects = new List<GameObject>(); // list of objects that have connections
    [SerializeField] private int availableNum = 1; // number given to the next brand new group
    [SerializeField] private bool debugging = false; //for debugging

    public void EmptyObjects()
    {
        connectedGameObjects.Clear();
    }


    public void ConnectObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1, int groupNum1,
        GameObject object2, Collider2D gameObjectCollider2, bool electricState2, int groupNum2)
    {
        //neither object in a group so create a new one
        if (groupNum1 == 0 && groupNum2 == 0)
        {
            //Debug.LogWarning("NEITHER OBJECT GROUPED");
            if (electricState1)
            {
                object2.GetComponent<IElectrifiable>().SetElectrified(true);
            }
            else if (electricState2)
            {
                object1.GetComponent<IElectrifiable>().SetElectrified(true);
            }
            object1.GetComponent<IElectrifiable>().SetGroupNum(availableNum);
            object2.GetComponent<IElectrifiable>().SetGroupNum(availableNum);
            availableNum++;
            connectedGameObjects.Add(object1);
            connectedGameObjects.Add(object2);
        }
        //both in the same group
        else if (groupNum1 == groupNum2)
        {
            //nothing should be needed here
        }
        //both in separate groups
        else if (groupNum1 > 0 && groupNum2 > 0)
        {
            //Debug.LogWarning("BOTH OBJECTS GROUPED SEPARATELY");
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                //going through to find each group 2 object to set it to group 1 and setting electricity
                if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum2)
                {
                    connectedGameObjects[i].GetComponent<IElectrifiable>().SetGroupNum(groupNum1);
                    if (electricState1 && !electricState2)
                    {
                        connectedGameObjects[i].GetComponent<IElectrifiable>().SetElectrified(true);
                    }
                }
                //checks each group 1 object to set electricity
                if (!electricState1 && electricState2)
                {
                    if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum1)
                    {
                        connectedGameObjects[i].GetComponent<IElectrifiable>().SetElectrified(true);
                    }
                }
            }
        }
        //the first object is in a group but the second one is not
        else if (groupNum1 > 0)
        {
            //Debug.LogWarning("FIRST OBJECT GROUPED AND SECOND OBJECT NOT");
            // electrifies object 2 because object 1 is electrified
            if (electricState1 && !electricState2)
            {
                object2.GetComponent<IElectrifiable>().SetElectrified(true);
            }
            // electrifies groupNum1 objects because object 2 is electrified
            else if (!electricState1 && electricState2)
            {
                for (int i = 0; i < connectedGameObjects.Count; i++)
                {
                    if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum1)
                    {
                        connectedGameObjects[i].GetComponent<IElectrifiable>().SetElectrified(true);
                    }
                }
            }
            //sets object 2 to groupNum1
            object2.GetComponent<IElectrifiable>().SetGroupNum(groupNum1);
            connectedGameObjects.Add(object2);
        }
        //second object is in a group but first is not
        else if (groupNum2 > 0)
        {
            //Debug.LogWarning("SECOND OBJECT GROUPED AND FIRST OBJECT NOT");
            if (!electricState1 && electricState2)
            {
                object1.GetComponent<IElectrifiable>().SetElectrified(true);
            }
            else if (electricState1 && !electricState2)
            {
                for (int i = 0; i < connectedGameObjects.Count; i++)
                {
                    if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum2)
                    {
                        connectedGameObjects[i].GetComponent<IElectrifiable>().SetElectrified(true);
                    }
                }
            }
            object1.GetComponent<IElectrifiable>().SetGroupNum(groupNum2);
            connectedGameObjects.Add(object1);
        }
        else
        {
            //Debug.LogError("Shouldn't reach here when connecting");
        }

        List<GameObject> tempList = new List<GameObject>(connectedGameObjects);
        connectedGameObjects = tempList.Distinct().ToList();

        //int listLength = connectedGameObjects.Count;
        //int counter = 0;

        //while (counter < listLength)
        //{
        //    if (connectedGameObjects[counter].GetComponent<IElectrifiable>().GetGroupNum() == 0)
        //    {
        //        connectedGameObjects.RemoveAt(counter);
        //        listLength--;
        //    }
        //    else
        //    {
        //        counter++;
        //    }
        //}

        //Debugging
        //if (debugging)
        //{
        //    Debug.Log("Connected List");
        //    Debug.Log("--------------------");
        //    Debug.Log("--------------------");
        //    for (int i = 0; i < connectedGameObjects.Count; i++)
        //    {
        //        Debug.Log("Object: " + connectedGameObjects[i]);
        //        Debug.Log("Objects Group: " + connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum());
        //        Debug.Log("--------------------");
        //    }
        //    Debug.Log("--------------------");
        //}
    }

    public void DisconnectObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1, int groupNum1,
        GameObject object2, Collider2D gameObjectCollider2, bool electricState2, int groupNum2)
    {
        if (groupNum1 == groupNum2)
        {

            //Debug.LogWarning("FIRST TIME DISCONNECTING");
            //lists for the directly connected objects to each object
            List<GameObject> tempConnectedGameObjects1 = new List<GameObject>();
            List<GameObject> tempConnectedGameObjects2 = new List<GameObject>();
            
            tempConnectedGameObjects1 = object1.GetComponent<IElectrifiable>().GetConnectedObjects();
            tempConnectedGameObjects2 = object2.GetComponent<IElectrifiable>().GetConnectedObjects();
            
            bool objectsStillConnected = false; // set to true if objects are still connected later
            /*
             go through objects connected to object 1 and 2 and see if they have any that are still connected
             to each other and object 1 and 2
            */
            for (int i = 0; i < tempConnectedGameObjects1.Count; i++)
            {
                for (int k = 0; k < tempConnectedGameObjects2.Count; k++)
                {
                    if (tempConnectedGameObjects1[i] == tempConnectedGameObjects2[k])
                    {
                        objectsStillConnected = true;
                    }
                }
            }
            // if the object are not still connected through other objects
            if (!objectsStillConnected)
            {
                //everything was electrified
                if (electricState1)
                {
                    //set if the objects are still connected to water therefore still electrified
                    bool isWater1 = false;
                    bool isWater2 = false;
                    if (object1.CompareTag("Water"))
                    {
                        isWater1 = true;
                    }
                    else
                    {
                        for (int i = 0; i < tempConnectedGameObjects1.Count; i++)
                        {
                            if (tempConnectedGameObjects1[i] != object2)
                            {
                                if (tempConnectedGameObjects1[i].CompareTag("Water"))
                                {
                                    isWater1 = true;
                                }
                            }
                        }
                    }

                    if (object2.CompareTag("Water"))
                    {
                        isWater2 = true;
                    }
                    else
                    {
                        for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
                        {
                            if (tempConnectedGameObjects2[i] != object1)
                            {
                                if (tempConnectedGameObjects2[i].CompareTag("Water"))
                                {
                                    isWater2 = true;
                                }
                            }
                        }
                    }

                    /*
                     if the objects are no longer connected to water set electrified false for the
                     object and all directly connected objects
                    */
                    if (!isWater1)
                    {
                        for (int i = 0; i < tempConnectedGameObjects1.Count; i++)
                        {
                            if (tempConnectedGameObjects1[i] != object2)
                            {
                                tempConnectedGameObjects1[i].GetComponent<IElectrifiable>().SetElectrified(false);
                            }
                        }
                        object1.GetComponent<IElectrifiable>().SetElectrified(false);
                    }
                    if (!isWater2)
                    {
                        for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
                        {
                            if (tempConnectedGameObjects2[i] != object1)
                            {
                                tempConnectedGameObjects2[i].GetComponent<IElectrifiable>().SetElectrified(false);
                            }
                        }
                        object2.GetComponent<IElectrifiable>().SetElectrified(false);
                    }

                    //checks if the objects are still in groups after disconnecting
                    bool objectGrouped1 = false;
                    bool objectGrouped2 = false;
                    if (tempConnectedGameObjects1.Count <= 1)
                    {
                        int removePos = -1;
                        object1.GetComponent<IElectrifiable>().SetGroupNum(0);
                        for (int i = 0; i < connectedGameObjects.Count; i++)
                        {
                            if (connectedGameObjects[i] == object1)
                            {
                                removePos = i;
                            }
                        }

                        if (removePos > -1)
                        {
                            connectedGameObjects.RemoveAt(removePos);
                        }
                        else
                        {
                            //Debug.LogError("The pos was out of range");
                        }
                    }
                    else
                    {
                        objectGrouped1 = true;
                    }
                    if (tempConnectedGameObjects2.Count <= 1)
                    {
                        int removePos = -1;
                        object2.GetComponent<IElectrifiable>().SetGroupNum(0);
                        for (int i = 0; i < connectedGameObjects.Count; i++)
                        {
                            if (connectedGameObjects[i] == object2)
                            {
                                removePos = i;
                            }
                        }
                        if (removePos > -1)
                        {
                            connectedGameObjects.RemoveAt(removePos);
                        }
                        else
                        {
                            //Debug.LogError("The pos was out of range");
                        }
                    }
                    else
                    {
                        objectGrouped2 = true;
                    }
                    //if both objects are still in groups after disconnecting create a new group
                    if (objectGrouped1 && objectGrouped2)
                    {
                        for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
                        {
                            if (tempConnectedGameObjects2[i] != object1)
                            {
                                tempConnectedGameObjects2[i].GetComponent<IElectrifiable>().SetGroupNum(availableNum);
                            }
                        }
                        object2.GetComponent<IElectrifiable>().SetGroupNum(availableNum);
                        availableNum++;
                    }

                }
                //nothing is electrified
                else
                {
                    bool objectGrouped1 = false;
                    bool objectGrouped2 = false;
                    if (tempConnectedGameObjects1.Count <= 1)
                    {
                        object1.GetComponent<IElectrifiable>().SetGroupNum(0);
                        int removePos = -1;
                        for (int i = 0; i < connectedGameObjects.Count; i++)
                        {
                            if (connectedGameObjects[i] == object1)
                            {
                                removePos = i;
                            }
                        }
                        if (removePos > -1)
                        {
                            connectedGameObjects.RemoveAt(removePos);
                        }
                        else
                        {
                            //Debug.LogError("The pos was out of range");
                        }
                        //should maybe remove from list here
                    }
                    else
                    {
                        objectGrouped1 = true;
                    }
                    if (tempConnectedGameObjects2.Count <= 1)
                    {
                        object2.GetComponent<IElectrifiable>().SetGroupNum(0);
                        int removePos = -1;
                        for (int i = 0; i < connectedGameObjects.Count; i++)
                        {
                            if (connectedGameObjects[i] == object2)
                            {
                                removePos = i;
                            }
                        }
                        if (removePos > -1)
                        {
                            connectedGameObjects.RemoveAt(removePos);
                        }
                        else
                        {
                            //Debug.LogError("The pos was out of range");
                        }
                        //should maybe remove from list here
                    }
                    else
                    {
                        objectGrouped2 = true;
                    }
                    if (objectGrouped1 && objectGrouped2)
                    {
                        bool atEnd = false;
                        bool exitEarly = false;
                        GameObject currentGameObject = object2;
                        while (!atEnd)
                        {
                            List<GameObject> tempConnectedTo2GameObjects = new List<GameObject>();
                            tempConnectedTo2GameObjects = currentGameObject.GetComponent<IElectrifiable>().GetConnectedObjects();

                            if (tempConnectedTo2GameObjects.Count > 0)
                            {
                                for (int k = 0; k < tempConnectedTo2GameObjects.Count; k++)
                                {
                                    if (tempConnectedTo2GameObjects[k] != object1 &&
                                        tempConnectedTo2GameObjects[k] != object2)
                                    {
                                        if (tempConnectedTo2GameObjects[k].GetComponent<IElectrifiable>().GetGroupNum() != availableNum)
                                        {
                                            tempConnectedTo2GameObjects[k].GetComponent<IElectrifiable>().SetGroupNum(availableNum);
                                            currentGameObject = tempConnectedTo2GameObjects[k];
                                            exitEarly = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (exitEarly)
                            {
                                exitEarly = false;
                            }
                            else
                            {
                                atEnd = true;
                                currentGameObject.GetComponent<IElectrifiable>().SetGroupNum(availableNum);
                            }
                        }

                        //for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
                        //{
                        //    if (tempConnectedGameObjects2[i] != object1)
                        //    {
                        //        List<GameObject> tempConnectedTo2GameObjects = new List<GameObject>();
                        //        tempConnectedTo2GameObjects = tempConnectedGameObjects2[i].GetComponent<IElectrifiable>().GetConnectedObjects();
                                
                              
                        //    }
                        //}

                        object2.GetComponent<IElectrifiable>().SetGroupNum(availableNum);
                        availableNum++;
                        //for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
                        //{
                        //    if (tempConnectedGameObjects2[i] != object1)
                        //    {
                        //        List<GameObject> tempConnectedTo2GameObjects = new List<GameObject>();
                        //        tempConnectedTo2GameObjects = tempConnectedGameObjects2[i].GetComponent<IElectrifiable>().GetConnectedObjects();
                        //        if (tempConnectedTo2GameObjects.Count > 0)
                        //        {
                        //            for (int k = 0; k < tempConnectedTo2GameObjects.Count; k++)
                        //            {
                        //                if (tempConnectedTo2GameObjects[k] != object1 &&
                        //                    tempConnectedTo2GameObjects[k] != object2)
                        //                {
                        //                    tempConnectedTo2GameObjects[k].GetComponent<IElectrifiable>().SetGroupNum(availableNum);
                        //                }
                        //            }
                        //        }
                        //        tempConnectedGameObjects2[i].GetComponent<IElectrifiable>().SetGroupNum(availableNum);
                        //    }
                        //}

                    }
                }
            }
            //for (int i = 0; i < connectedGameObjects.Count; i++)
            //{
            //    if (connectedGameObjects[i] != object1)
            //    {
            //        if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum1)
            //        {
            //            for (int k = 0; k < tempConnectedGameObjects1.Count; k++)
            //            {
            //                if 
            //            }
            //        }
            //    }
            //}
        }
        else
        {
            //Debug.LogError("These objects should have been connected");
            //Debug.LogError("Object 1: " + object1);
            //Debug.LogError("Object 1 Group: " + object1.GetComponent<IElectrifiable>().GetGroupNum());
            //Debug.LogError("Object 2: " + object2);
            //Debug.LogError("Object 2 Group: " + object2.GetComponent<IElectrifiable>().GetGroupNum());
        }
        int listLength = connectedGameObjects.Count;
        int counter = 0;

        //while (counter < listLength)
        //{
        //    if (connectedGameObjects[counter].GetComponent<IElectrifiable>().GetGroupNum() == 0)
        //    {
        //        connectedGameObjects.RemoveAt(counter);
        //        listLength--;
        //    }
        //    else
        //    {
        //        counter++;
        //    }
        //}

    }

    public void ElectrifyConnectedObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1, int groupNum1)
    {
        if (!electricState1)
        {
            bool isWater = object1.CompareTag("Water");
            if (!isWater)
            {
                for (int i = 0; i < connectedGameObjects.Count; i++)
                {
                    if (connectedGameObjects[i] != null)
                    {
                        if (connectedGameObjects[i] != null)
                        {
                            if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum1)
                            {
                                if (connectedGameObjects[i].CompareTag("Water"))
                                {
                                    isWater = true;
                                }
                            }
                        }
                    }
                }
            }
            if (isWater)
            {
                if (groupNum1 == 0)
                {
                    object1.GetComponent<IElectrifiable>().SetElectrified(true);
                }
                else
                { 
                    for (int i = 0; i < connectedGameObjects.Count; i++)
                    {
                        if (connectedGameObjects[i] != null)
                        {
                            if (connectedGameObjects[i].GetComponent<IElectrifiable>().GetGroupNum() == groupNum1)
                            {
                                connectedGameObjects[i].GetComponent<IElectrifiable>().SetElectrified(true);
                            }
                        }
                    }
                }
            }
        }
    }
}