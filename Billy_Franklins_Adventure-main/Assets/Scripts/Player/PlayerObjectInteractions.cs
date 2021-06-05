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

    [SerializeField] private List<GameObject> toggleObjects = new List<GameObject>(); // list of interactable objects in interactable circle 
    [SerializeField] private GameObject currentToggleObject; // current selected interactable object in interactable circle
    private int togglePos = 0; // position of selected interactable object in the list of interactable objects 
    
    //toggles through objects in the list of interactable objects
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
            currentToggleObject.GetComponent<IInteractable>().SetHighlighted(false);
            currentToggleObject = toggleObjects[togglePos];
            currentToggleObject.GetComponent<IInteractable>().SetHighlighted(true);
        }
        
        
    }

    //returns selected object when interacting with interactable objects
    public GameObject GetCurrentObject()
    {
        return currentToggleObject;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // checks if collision is with the interactable layer 9 and not rope
        if (collision.gameObject.layer == 9 && !collision.CompareTag("Rope"))
        {
            //adds to the list of interactable objects in the interactable circle
            //sets the selected interactable object to the one collided with if the list is empty
            if (toggleObjects.Count < 1)
            {
                currentToggleObject = collision.gameObject;
                currentToggleObject.GetComponent<IInteractable>().SetHighlighted(true);
                togglePos = 0;
            }

            toggleObjects.Add(collision.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //collider that is exiting is the selected objects collider 
        //deselects selected object
        if (collision.gameObject == currentToggleObject)
        {
            currentToggleObject.GetComponent<IInteractable>().SetHighlighted(false);
            currentToggleObject = null;
        }
        if (collision.gameObject.layer == 9 && !collision.CompareTag("Rope"))
        {
            
            for (int i = 0; i < toggleObjects.Count; i++)
            {
                if (collision.gameObject == toggleObjects[i])
                {
                    if (collision.gameObject.CompareTag("Metal"))
                    {
                        if (collision.gameObject.GetComponent<Metal>().IsMoving())
                        {
                            collision.gameObject.GetComponent<Metal>().SetMoving(false);
                        }
                    }
                    toggleObjects.RemoveAt(i);
                }
            }
            if (toggleObjects.Count >= 1 && currentToggleObject == null)
            {
                togglePos = 0;
                currentToggleObject = toggleObjects[togglePos];
                currentToggleObject.GetComponent<IInteractable>().SetHighlighted(true);
            }
        }
    }
}