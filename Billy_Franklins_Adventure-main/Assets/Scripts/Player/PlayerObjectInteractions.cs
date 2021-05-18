using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectInteractions : MonoBehaviour
{
    #region Singleton
    public static PlayerObjectInteractions playerObjectIInstance;

    private void Awake()
    {
        //Make sure there is only one instance
        if (playerObjectIInstance == null)
        {
            playerObjectIInstance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    [SerializeField] private List<GameObject> toggleObjects = new List<GameObject>();
    [SerializeField] private GameObject currentToggleObject;
    private int togglePos = 0; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleObjects()
    {
        if (toggleObjects.Count > 1)
        {
            if (togglePos == toggleObjects.Count - 1)
            {
                togglePos = 0;
            }
            else
            {
                togglePos++;
            }
        }

        currentToggleObject = toggleObjects[togglePos];
    }

    public GameObject GetCurrentObject()
    {
        return currentToggleObject;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            if (toggleObjects.Count <= 0)
            {
                currentToggleObject = collision.gameObject;
                togglePos = 0;
            }

            toggleObjects.Add(collision.gameObject);
            
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == currentToggleObject)
        {
            currentToggleObject = null;
        }
        if (collision.gameObject.layer == 9)
        {
            for (int i = 0; i < toggleObjects.Count; i++)
            {
                if (collision.gameObject == toggleObjects[i])
                {
                    toggleObjects.RemoveAt(i);
                    if (toggleObjects.Count >= 1 && currentToggleObject == null)
                    {
                        togglePos = 0;
                        currentToggleObject = toggleObjects[togglePos];
                    }
                }
            }
        }
    }
}