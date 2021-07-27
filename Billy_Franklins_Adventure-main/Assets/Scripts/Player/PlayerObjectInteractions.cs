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
    [SerializeField] private GameObject metalLeftPos;
    [SerializeField] private GameObject metalRightPos;
    [SerializeField] private Player player;



    private int togglePos = 0; // position of selected interactable object in the list of interactable objects 

    void Start()
    {
        player = FindObjectOfType<Player>();

    }



    //toggles through objects in the list of interactable objects
    public void ToggleObjects()
    {
        if (toggleObjects.Count > 1)
        {
            if (currentToggleObject.GetComponent<Collider2D>().CompareTag("Metal"))
            {
                if (currentToggleObject.GetComponent<Metal>().IsMoving())
                {
                    return;
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

    public GameObject GetMetalLeftPos()
    {
        if (metalLeftPos == null)
        {
            metalLeftPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointLeft();
        }
        return metalLeftPos;
    }

    public GameObject GetMetalRightPos()
    {
        if (metalRightPos == null)
        {
            metalRightPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointRight();
        }
        return metalRightPos;
    }

    public void ConnectMetalToPlayer()
    {
        //currentToggleObject.transform.parent = player.transform;
        currentToggleObject.GetComponent<Metal>().ConnectMetalToPlayer();
    }

    //returns selected object when interacting with interactable objects
    public GameObject GetCurrentObject()
    {
        return currentToggleObject;
    }

    public void EmptyObjects()
    {
        toggleObjects.Clear();
        currentToggleObject = null;
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
                //if (!player.GetAnimationSwitch())
                //{
                if (collision.gameObject.GetComponent<Metal>().GetMovable())
                {
                    if (toggleObjects.Count < 1)
                    {
                        currentToggleObject = collision.gameObject;
                        //if (currentToggleObject.GetComponent<Collider2D>().CompareTag("Metal"))
                        //{
                        metalLeftPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointLeft();
                        metalRightPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointRight();
                        //}

                        currentToggleObject.GetComponent<IInteractable>().SetHighlighted(true);
                        togglePos = 0;
                    }

                    toggleObjects.Add(collision.gameObject);
                }
                //}
            }
            else
            {
                if (toggleObjects.Count < 1)
                {
                    currentToggleObject = collision.gameObject;
                    //if (currentToggleObject.GetComponent<Collider2D>().CompareTag("Metal"))
                    //{
                    //    metalLeftPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointLeft();
                    //    metalRightPos = currentToggleObject.GetComponent<Metal>().GetPickUpPointRight();
                    //}

                    if (currentToggleObject != null)
                    {
                        currentToggleObject.GetComponent<IInteractable>().SetHighlighted(true);
                    }

                    togglePos = 0;
                }
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
            
            for (int i = 0; i < toggleObjects.Count; i++)
            {
                if (collision.gameObject == toggleObjects[i])
                {
                    if (collision.gameObject.CompareTag("Metal"))
                    {
                        if (toggleObjects[i] == currentToggleObject)
                        {
                            toggleObjects[i].GetComponent<IInteractable>().SetHighlighted(false);
                            toggleObjects.RemoveAt(i);
                            currentToggleObject = null;
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
                            toggleObjects[i].GetComponent<IInteractable>().SetHighlighted(false);
                            currentToggleObject = null;
                        }
                        toggleObjects.RemoveAt(i);
                    }
                    //}
                }
            }

            if (toggleObjects.Count >= 1 && currentToggleObject == null)
            {
                togglePos = 0;

                currentToggleObject = toggleObjects[togglePos];
                if (currentToggleObject != null)
                { 
                    currentToggleObject.GetComponent<IInteractable>().SetHighlighted(true);
                }
            }
        }
    }
}