using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject highlight; // white square around switch

    [SerializeField] private List<Door> doors = new List<Door>(); // doors connected to the switch

    [Header("FMOD Settings")]
    [FMODUnity.EventRef]
    [SerializeField]
    private string gateLiftingEventPath;
    private FMOD.Studio.EventInstance gateLiftingEvent;
    [SerializeField]
    private float gateLiftingVolume = 0.8f;

    public void Start()
    {
        gateLiftingEvent = FMODUnity.RuntimeManager.CreateInstance(gateLiftingEventPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(gateLiftingEvent, transform, GetComponent<Rigidbody>());
        gateLiftingEvent.setVolume(gateLiftingVolume);
    }

    // called to interact with switch and all connected doors
    public void Interact()
    {
        gateLiftingEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        gateLiftingEvent.start();
        for (int i = 0; i < doors.Count; i++)
        {
            Debug.LogError("Switch");
            doors[i].GetComponent<Door>().SetDoorState();
        }
    }
    //sets whether the switch is highlighted or not
    public void SetHighlighted(bool state)
    {
        Debug.LogError("Highlight" + state);
        highlight.SetActive(state);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    { 
        //switch is hit by lightning and sets door movement
        if (collision.CompareTag("Lightning"))
        {
            gateLiftingEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            gateLiftingEvent.start();

            for (int i = 0; i < doors.Count; i++)
            {
                doors[i].GetComponent<Door>().SetDoorState();
            }
        }
    }
}
