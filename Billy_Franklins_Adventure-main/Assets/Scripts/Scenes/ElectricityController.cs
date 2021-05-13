using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    //left the SerializeField in to sho a 2D list will not show up in the inspector 
    [SerializeField] private List<List<GameObject>> connectedGameObjects = new List<List<GameObject>>(10);
    [SerializeField] private bool debugging = true;


    public void ConnectObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1,
        GameObject object2, Collider2D gameObjectCollider2, bool electricState2)
    {
        Debug.Log("Enter Connect Objects");
        int object1Pos = -1;
        int object2Pos = -1;


        //groups of objects already exists
        if (connectedGameObjects.Count > 0)
        {
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i].Count > 0)
                {
                    for (int k = 0; k < connectedGameObjects[i].Count; k++)
                    {
                        if (connectedGameObjects[i][k] == object1)
                        {
                            object1Pos = i;
                        }

                        if (connectedGameObjects[i][k] == object2)
                        {
                            object2Pos = i;
                        }
                    }
                }
            }

            //if both objects are in the same pos 
            //could be they are in the same group or neither are in a group 
            if (object1Pos == object2Pos)
            {
                //both objects are already in the same previous group
                //they should already have determined if they are electrified
                if (object1Pos > -1)
                {

                }
                //neither object is in a previous group
                else
                {
                    Debug.Log( "Neither are in a group" );
                    if (electricState1 || electricState2)
                    {
                        if (!electricState1)
                        {
                            object1.GetComponent<IElectrifiable>().SetElectrified(true);
                        }

                        if (!electricState2)
                        {
                            object2.GetComponent<IElectrifiable>().SetElectrified(true);
                        }
                    }
                    List<GameObject> tempConnectedGameObjects = new List<GameObject>();
                    tempConnectedGameObjects.Add(object1);
                    tempConnectedGameObjects.Add(object2);
                    connectedGameObjects.Add(tempConnectedGameObjects);
                }
            }
            //each object is in a separate group
            else if (object1Pos > -1 && object2Pos > -1)
            {
                if (electricState1 && !electricState2)
                {
                    for (int i = 0; i < connectedGameObjects[object2Pos].Count; i++)
                    {
                        connectedGameObjects[object2Pos][i].GetComponent<IElectrifiable>().SetElectrified(true);
                    }
                }

                if (!electricState1 && electricState2)
                {
                    for (int i = 0; i < connectedGameObjects[object1Pos].Count; i++)
                    {
                        connectedGameObjects[object1Pos][i].GetComponent<IElectrifiable>().SetElectrified(true);
                    }
                }

                //adds all the objects from group 2 into group 1
                //it sets the electrified true for group 2 if group 1 is electrified
                for (int i = 0; i < connectedGameObjects[object2Pos].Count; i++)
                {
                    connectedGameObjects[object1Pos].Add(connectedGameObjects[object2Pos][i]);
                }

                connectedGameObjects.RemoveAt(object2Pos);
            }
            //object 1 is in a group and object 2 is not in a group
            else if (object1Pos > -1 && object2Pos == -1)
            {
                if (electricState1 && !electricState2)
                {
                    object2.GetComponent<IElectrifiable>().SetElectrified(true);
                }

                if (!electricState1 && electricState2)
                {
                    for (int i = 0; i < connectedGameObjects[object1Pos].Count; i++)
                    {
                        connectedGameObjects[object1Pos][i].GetComponent<IElectrifiable>().SetElectrified(true);
                    }
                }

                connectedGameObjects[object1Pos].Add(object2);
            }
            //object 2 is in a group and object 1 is not in a group
            else
            {
                if (!electricState1 && electricState2)
                {
                    object1.GetComponent<IElectrifiable>().SetElectrified(true);
                }

                if (electricState1 && !electricState2)
                {
                    for (int i = 0; i < connectedGameObjects[object2Pos].Count; i++)
                    {
                        connectedGameObjects[object2Pos][i].GetComponent<IElectrifiable>().SetElectrified(true);
                    }
                }

                connectedGameObjects[object2Pos].Add(object1);
            }
        }
        //isn't any groups of connected objects
        else
        {
            Debug.Log("No Groups");
            if (electricState1 || electricState2)
            {
                if (!electricState1)
                {
                    object1.GetComponent<IElectrifiable>().SetElectrified(true);
                }

                if (!electricState2)
                {
                    object2.GetComponent<IElectrifiable>().SetElectrified(true);
                }
            }

            List<GameObject> tempConnectedGameObjects = new List<GameObject>();
            tempConnectedGameObjects.Add(object1);
            tempConnectedGameObjects.Add(object2);
            connectedGameObjects.Add(tempConnectedGameObjects);
        }

        //Debugging
        if (debugging)
        {
            if (connectedGameObjects.Count > 0)
            {
                for (int i = 0; i < connectedGameObjects.Count; i++)
                {
                    Debug.Log("Group" + i + ":");
                    if (connectedGameObjects[i].Count > 0)
                    {
                        for (int k = 0; k < connectedGameObjects[i].Count; k++)
                        {
                            Debug.Log("Game Object " + k + ": " + connectedGameObjects[i][k]);
                        }
                    }
                }
            }
        }
    }

    public void DisconnectObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1,
        GameObject object2, Collider2D gameObjectCollider2, bool electricState2)
    {

        Debug.Log("Disconnect Objects 1");
        int object1Pos = -1;
        int object2Pos = -1;

        bool isObject2Water = false;
        List<GameObject> tempConnectedGameObjects = new List<GameObject>();
        //groups of objects already exists
        Debug.Log("Disconnect Objects 2");
        if (connectedGameObjects.Count > 0)
        {
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i].Count > 0)
                {
                    for (int k = 0; k < connectedGameObjects[i].Count; k++)
                    {
                        if (connectedGameObjects[i][k] == object1)
                        {
                            object1Pos = i;
                        }
                        if (connectedGameObjects[i][k] == object2)
                        {
                            object2Pos = k;
                            if (gameObjectCollider2.CompareTag("Water"))
                            {
                                isObject2Water = true;
                            }

                            tempConnectedGameObjects = object2.GetComponent<IElectrifiable>().GetConnectedObjects();
                        }
                    }
                }
            }
        }
        Debug.Log("Disconnect Objects 3");
        bool object2StillConnected = false;
        if (object1Pos != -1)
        {
            for (int i = 0; i < connectedGameObjects[object1Pos].Count; i++)
            {
                for (int k = 0; k < tempConnectedGameObjects.Count; k++)
                {
                    if (connectedGameObjects[object1Pos][i] == tempConnectedGameObjects[k])
                    {
                        if (tempConnectedGameObjects[k] != object1)
                        {
                            object2StillConnected = true;
                        }
                    }
                }
            }
        }

        if (object1Pos != -1)
        {
            Debug.Log("Disconnect Objects 4");
            if (!object2StillConnected)
            {
                if (isObject2Water)
                {
                    connectedGameObjects[object1Pos].RemoveAt(object2Pos);
                }
                else
                {
                    object2.GetComponent<IElectrifiable>().SetElectrified(false);
                    connectedGameObjects[object1Pos].RemoveAt(object2Pos);
                }
            }
        }
    }

    public void ElectrifyConnectedObjects(GameObject object1, Collider2D gameObjectCollider1)
    {
       Debug.Log("Electrifying Objects 1");
        int object1Pos = -1;
        if (connectedGameObjects.Count > 0)
        {
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i].Count > 0)
                {
                    for (int k = 0; k < connectedGameObjects[i].Count; k++)
                    {
                        if (connectedGameObjects[i][k] == object1)
                        {
                            object1Pos = i;
                        }
                    }
                }
            }
        }
        Debug.Log("Electrifying Objects 2");
        bool isWater = false;

        if (gameObjectCollider1.CompareTag("Water"))
        {
            isWater = true;
        }
        else
        {
            if (object1Pos != -1)
            {
                for (int i = 0; i < connectedGameObjects[object1Pos].Count; i++)
                {
                    if (connectedGameObjects[object1Pos][i].GetComponent<BoxCollider2D>().CompareTag("Water"))
                    {
                        isWater = true;
                        break;
                    }
                }
            }
        }
        Debug.Log("Electrifying Objects 3");
        if (isWater)
        {
            if (object1Pos == -1)
            {
                object1.GetComponent<IElectrifiable>().SetElectrified(true);
            }
            else
            {
                if (object1Pos != -1)
                {
                    for (int i = 0; i < connectedGameObjects[object1Pos].Count; i++)
                    {
                        connectedGameObjects[object1Pos][i].GetComponent<IElectrifiable>().SetElectrified(true);
                    }
                }
            }
        }
        Debug.Log("Electrifying Objects 4");
    }
}

