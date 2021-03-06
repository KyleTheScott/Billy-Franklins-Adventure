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
    [SerializeField] private GameObject metalLeftPos; // position the player walks toward on the left side of the metal    
    [SerializeField] private GameObject metalRightPos; // position the player walks toward on right side of the metal 
    [SerializeField] private Player player;
    [SerializeField] private int togglePos = 0; // position of selected interactable object in the list of interactable objects 
    [SerializeField] private bool interacting = false;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // sets if the player is in th middle of interacting with the current toggle object
    public void SetInteracting(bool state)
    {
        interacting = state;
    }

    /*set the highlight of the current toggle object
    */
    public void SetCurrentHighLighted(bool state)
    {
        if (currentToggleObject != null)
        {
            if (!currentToggleObject.CompareTag("Switch"))
            {
                //Debug.LogError("highlight Set interacting");
                currentToggleObject.GetComponent<IInteractable>().SetHighlighted(state);
            }
        }
    }


    //toggles through objects in the list of interactable objects
    public void ToggleObjects()
    {
        //if there is more than 1 object to toggle through
        if (toggleObjects.Count > 1)
        {
            if (!interacting)
            {
                if (currentToggleObject.GetComponent<Collider2D>().CompareTag("Metal"))
                {
                    if (currentToggleObject.GetComponent<Metal>().IsMoving())
                    {
                        return;
                    }
                }
                for(int i = 0; i < toggleObjects.Count; i++)
                {
                    if (toggleObjects[i] == currentToggleObject)
                    {
                        togglePos = i;
                    }
                }
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
                if (currentToggleObject.GetComponent<Collider2D>().CompareTag("Metal"))
                {
                    metalLeftPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointLeft();
                    metalRightPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointRight();
                }
                currentToggleObject.GetComponent<IInteractable>().SetHighlighted(true);
            }
        }
    }

    //left side of the metal position that the player is walking toward
    public GameObject GetMetalLeftPos()
    {
        if (metalLeftPos == null)
        {
            metalLeftPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointLeft();
        }
        return metalLeftPos;
    }

    //right side of the metal position that the player is walking toward
    public GameObject GetMetalRightPos()
    {
        if (metalRightPos == null)
        {
            metalRightPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointRight();
        }
        return metalRightPos;
    }

    //sets the pickup points so the player can walk towards metal points during automated animations
    public void ConnectMetalToPlayer()
    {
        SetMetalPos();
        currentToggleObject.GetComponent<Metal>().ConnectMetalToPlayer();
    }

    //set the left or right positions of the metal the player can walk toward to pick up
    private void SetMetalPos()
    {
        metalLeftPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointLeft();
        metalRightPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointRight();
    }

    //returns selected object when interacting with interactable objects
    public GameObject GetCurrentObject()
    {
        return currentToggleObject;
    }

    //reset everything
    public void EmptyObjects()
    {
        toggleObjects.Clear();
        metalLeftPos = null;
        metalRightPos = null;
        interacting = false;
        currentToggleObject = null;
    }

    //new object becomes current toggle object
    private void SetNewToggleObjectToCurrent(GameObject currentObject)
    {
        if (currentToggleObject != null)
        {
            //Debug.LogError("Highlight off");
            currentToggleObject.GetComponent<IInteractable>().SetHighlighted(false);
        }
        currentToggleObject = currentObject;
        currentToggleObject.GetComponent<IInteractable>().SetHighlighted(true);
    }

    //change which current toggle object is the current object at exit
    public void ChangeToggleObject()
    {
        if (toggleObjects.Count > 1)
        {
            if (currentToggleObject != null)
            {
                currentToggleObject.GetComponent<IInteractable>().SetHighlighted(false);
            }

            for (int i = 0; i < toggleObjects.Count; i++)
            {
                //if a certain toggle object is the exiting object
                if (currentToggleObject.gameObject != toggleObjects[i])
                {
                    currentToggleObject = toggleObjects[i];
                    break;
                }
            }

            currentToggleObject.GetComponent<IInteractable>().SetHighlighted(true);
            if (currentToggleObject.CompareTag("Metal"))
            {
                SetMetalPos();
            }
        }
        else
        {
            if (currentToggleObject != null)
            {
                currentToggleObject.GetComponent<IInteractable>().SetHighlighted(false);
            }
            currentToggleObject = null;
        }
    }

    private void DisconnectMetal ()
    {
        metalLeftPos = null;
        metalRightPos = null;
    }

    //remove a a certain toggle object and calls function the changes current toggle object if current toggle object is being removed
    public void DisconnectObject(GameObject obj)
    {
        ChangeToggleObject();
        toggleObjects.Remove(obj);
        FindNewToggleObject();
    }

    //highlight and set new current toggle object if there is one
    public void FindNewToggleObject()
    {
        if (toggleObjects.Count >= 1)
        {
            SetNewToggleObjectToCurrent(toggleObjects[0]);
            Debug.LogError("New highlight");
            toggleObjects[0].GetComponent<IInteractable>().SetHighlighted(true);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // checks if collision is with the interactable layer 9 and not rope
        if (collision.gameObject.layer == 9 && !collision.CompareTag("Rope"))
        {
            //adds to the list of interactable objects in the interactable circle
            //sets the selected interactable object to the one collided with if the list is empty
            if (collision.gameObject.CompareTag("Metal"))
            {
                if (collision.gameObject.GetComponent<Metal>().GetMovable())
                {
                    //toggle objects is empty so add the object colliding
                    if (toggleObjects.Count < 1)
                    {
                        //should be false but just in case
                        if (!interacting)
                        {
                            SetNewToggleObjectToCurrent(collision.gameObject);
                            SetMetalPos();
                        }
                    }
                    //toggle objects is not empty so there are checks to see if player is interacting
                    else
                    {
                        if (!interacting)
                        {
                            SetNewToggleObjectToCurrent(collision.gameObject);
                            SetMetalPos();
                        }
                    }
                    //add the object to the list of toggle objects
                    toggleObjects.Add(collision.gameObject);
                }
            }
            //colliding with a bucket
            else if (collision.gameObject.CompareTag("Bucket"))
            {
                //toggle objects is empty so add the object colliding
                if (toggleObjects.Count < 1)
                {
                    //should be false but just in case
                    if (!interacting)
                    {
                        if (!collision.gameObject.GetComponent<Bucket>().GetTippedOver())
                        {
                            SetNewToggleObjectToCurrent(collision.gameObject);
                        }
                    }
                }
                //toggle objects is not empty so there are checks to see if player is interacting
                else
                {
                    if (!interacting)
                    {
                        if (!collision.gameObject.GetComponent<Bucket>().GetTippedOver())
                        {
                            SetNewToggleObjectToCurrent(collision.gameObject);
                        }
                    }
                }
                //add the object to the list of toggle objects
                toggleObjects.Add(collision.gameObject);
            }
            //colliding with other objects
            else
            {
                if (toggleObjects.Count < 1)
                {
                    SetNewToggleObjectToCurrent(collision.gameObject);
                }
                else
                {
                    if (!interacting)
                    {
                        SetNewToggleObjectToCurrent(collision.gameObject); 
                    }
                }
                //add the object to the list of toggle objects
                toggleObjects.Add(collision.gameObject);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //collider that is exiting is the selected objects collider 
        //deselects selected object
        if (collision.gameObject.layer == 9 && !collision.CompareTag("Rope"))
        {
            //looping through all the toggle objects
            for (int i = 0; i < toggleObjects.Count; i++)
            {
                //if a certain toggle object is the exiting object
                if (collision.gameObject == toggleObjects[i])
                {
                    //if the exiting object is metal
                    if (collision.gameObject.CompareTag("Metal"))
                    {
                        if (toggleObjects[i] == currentToggleObject)
                        {
                            //Debug.LogError("highlight false");
                            toggleObjects[i].GetComponent<IInteractable>().SetHighlighted(false);
                            interacting = false;
                            DisconnectMetal();
                            ChangeToggleObject();
                            toggleObjects.RemoveAt(i);
                            if (toggleObjects.Count < 1)
                            {
                                currentToggleObject = null;
                            }
                        }
                        else
                        {
                            //Debug.LogError("highlight false");
                            toggleObjects[i].GetComponent<IInteractable>().SetHighlighted(false);
                            toggleObjects.RemoveAt(i);
                            if (toggleObjects.Count < 1)
                            {
                                currentToggleObject = null;
                            }
                        }
                        if (collision.gameObject.GetComponent<Metal>().IsMoving())
                        {
                            collision.gameObject.GetComponent<Metal>().SetMoving(false);
                        }
                    }
                    else 
                    {
                        if (toggleObjects[i] == currentToggleObject)
                        {
                            //Debug.LogError("highlight false");
                            toggleObjects[i].GetComponent<IInteractable>().SetHighlighted(false);
                            interacting = false;
                            ChangeToggleObject();
                            toggleObjects.RemoveAt(i);
                            if (toggleObjects.Count < 1)
                            {
                                currentToggleObject = null;
                            }
                        }
                        else
                        {
                            //Debug.LogError("highlight false");
                            toggleObjects[i].GetComponent<IInteractable>().SetHighlighted(false);
                            toggleObjects.RemoveAt(i);
                            if (toggleObjects.Count < 1)
                            {
                                currentToggleObject = null;
                            }
                        }
                    }
                }
            }
        }
    }
}