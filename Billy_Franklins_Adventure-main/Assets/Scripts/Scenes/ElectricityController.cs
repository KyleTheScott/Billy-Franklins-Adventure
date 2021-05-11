using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricityController : MonoBehaviour
{
    #region Singleton
    public static ElectricityController instance;

    private void Awake()
    {
        //Make sure there is only one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    [SerializeField] private List<List<GameObject>> connectedGameObjects = new List<List<GameObject>>();
    [SerializeField] private List<List<GameObject>> electrifiedConnectedGameObjects = new List<List<GameObject>>();


    public void AddObject(GameObject electricalObject)
    {
        
    } 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
