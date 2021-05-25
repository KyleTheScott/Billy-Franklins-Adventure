using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    //left the SerializeField in to show a 2D list will not show up in the inspector 
    [SerializeField] private List<List<GameObject>> connectedGameObjects = new List<List<GameObject>>(10);
    [SerializeField] private bool debugging = true;


    // connects to objects and groups them accordingly
    public void ConnectObjects(GameObject object1, Collider2D gameObjectCollider1, bool electricState1,
        GameObject object2, Collider2D gameObjectCollider2, bool electricState2)
    {
        // keeps track of where the objects are in the connectedGameObjects list or
        // stays -1 if the objects are not in the list 
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
                    if (electricState1 || electricState2)
                    {
                        
                        if (!electricState1)
                        {
                            //object1.GetComponent<IElectrifiable>().SetElectrified(true);
                            object1.GetComponent<IElectrifiable>().ElectrifyConnectedObjects();
                        }

                        if (!electricState2)
                        {
                            //object2.GetComponent<IElectrifiable>().SetElectrified(true);
                            object1.GetComponent<IElectrifiable>().ElectrifyConnectedObjects();
                        }
                    }
                }
                //neither object is in a previous group
                //starts a new group and sets their electrified status
                else
                {
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
                //sets the electrified status for each of the groups the 2 objects are in
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
            else if (object1Pos == -1 && object2Pos > -1)
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
            Debug.Log("Connected Groups");
            Debug.Log("--------------------");
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
        //I will probably remove these variables that are not used but I might need them in the future
        //used to keep track of the position of object 1 positions
        int object1PosI = -1;
        int object1PosK = -1;
        //used to keep track of the position of object 2 positions
        int object2PosI = -1;
        int object2PosK = -1;

        //to keep track of what game objects are connected to directly not just what they are grouped with
        List<GameObject> tempConnectedGameObjects1 = new List<GameObject>();
        List<GameObject> tempConnectedGameObjects2 = new List<GameObject>();

        tempConnectedGameObjects1 = object1.GetComponent<IElectrifiable>().GetConnectedObjects();
        tempConnectedGameObjects2 = object2.GetComponent<IElectrifiable>().GetConnectedObjects();

        //groups of objects already exists
        //checking the objects positions within connectedGameObjects list to set the variables
        if (connectedGameObjects.Count > 0)
        {
            for (int i = 0; i < connectedGameObjects.Count; i++)
            {
                if (connectedGameObjects[i].Count > 0)
                {
                    for (int k = 0; k < connectedGameObjects[i].Count; k++)
                    {
                        //finds the positions of object1
                        if (connectedGameObjects[i][k] == object1)
                        {
                            object1PosI = i;
                            object1PosK = k;
                        }
                        //finds the positions of object2
                        if (connectedGameObjects[i][k] == object2)
                        {
                            object2PosI = i;
                            object2PosK = k;
                        }
                    }
                }
            }
        }

        bool objectsStillConnected = false;
        bool object1StillConnectedToWater = false;
        bool object2StillConnectedToWater = false;

        //checks if objects are still connected in the same grouping
        for (int i = 0; i < tempConnectedGameObjects1.Count; i++)
        {
            for (int k = 0; k < tempConnectedGameObjects2.Count; k++)
            {
                if (tempConnectedGameObjects1[i] == tempConnectedGameObjects2[k])
                {
                    if (tempConnectedGameObjects1[i] != object2 && tempConnectedGameObjects2[k] != object1)
                    {
                        objectsStillConnected = true;
                    }
                }
            }
        }

        if (!objectsStillConnected)
        {
            //checks if each object is still connected to water still after being separated
            if (gameObjectCollider1.CompareTag("Water"))
            {
                object1StillConnectedToWater = true;
            }
            else
            {
                for (int i = 0; i < tempConnectedGameObjects1.Count; i++)
                {
                    if (tempConnectedGameObjects1[i] != object2)
                    {
                        if (tempConnectedGameObjects1[i].gameObject.GetComponent<BoxCollider2D>().CompareTag("Water"))
                        {
                            object1StillConnectedToWater = true;
                        }
                    }
                }
            }
            
            if (gameObjectCollider2.CompareTag("Water"))
            {
                object2StillConnectedToWater = true;
            }
            else
            {
                for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
                {
                    if (tempConnectedGameObjects2[i] != object1)
                    {
                        if (tempConnectedGameObjects2[i].gameObject.GetComponent<BoxCollider2D>().CompareTag("Water"))
                        {
                            object2StillConnectedToWater = true;
                        }
                    }
                }
            }

            //checks if each object is still connected to water and if not turns off electricity for itself and other connect objects
            if (electricState1)
            {
                if (!object1StillConnectedToWater)
                {
                    for (int i = 0; i < tempConnectedGameObjects1.Count; i++)
                    {
                        if (tempConnectedGameObjects1[i] != object2)
                        {
                            tempConnectedGameObjects1[i].GetComponent<IElectrifiable>().SetElectrified(false);
                        }
                    }
                }
            }
            if (electricState2)
            {
                if (!object2StillConnectedToWater)
                {
                    for (int i = 0; i < tempConnectedGameObjects2.Count; i++)
                    {
                        if (tempConnectedGameObjects2[i] != object1)
                        {
                            tempConnectedGameObjects2[i].GetComponent<IElectrifiable>().SetElectrified(false);
                        }
                    }
                }
            }

            if (object1PosI != -1)
            {
                connectedGameObjects.RemoveAt(object1PosI);
            }
        }

        

        
        //Debugging
        if (debugging)
        {
            Debug.Log("Disconnected Groups");
            Debug.Log("--------------------");
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

    //electrifies objects as they are shot with lightning
    public void ElectrifyConnectedObjects(GameObject object1, Collider2D gameObjectCollider1)
    {
        //stores the position of object in connectedGameObjects list 
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
        //checks if the object is water or is in a grouping in connectedGameObjects list that has water
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
        
        //checks if object is water or object is in a group in connectedGameObjects list that has water
        //it then electrifies the grouping if true
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
                        connectedGameObjects[object1Pos][i].GetComponent<IElectrifiable>().ElectrifyConnectedObjects();
                    }
                }
            }
        }
    }
}