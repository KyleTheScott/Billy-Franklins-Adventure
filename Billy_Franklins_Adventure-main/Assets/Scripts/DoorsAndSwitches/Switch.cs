using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject highlight; // white square around switch

    [SerializeField] private List<Door> doors = new List<Door>(); // doors connected to the switch

    // called to interact with switch and all connected doors
    public void Interact()
    {
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].GetComponent<Door>().SetDoorState();
        }
    }
    //sets whether the switch is highlighted or not
    public void SetHighlighted(bool state)
    {
        highlight.SetActive(state);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //switch is hit by lightning and sets door movement
        if (collision.CompareTag("Lightning"))
        {
            for (int i = 0; i < doors.Count; i++)
            {
                doors[i].GetComponent<Door>().SetDoorState();
            }
        }
    }
}
