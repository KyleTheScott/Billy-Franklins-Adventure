using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private bool electrified = false;
    [SerializeField] private List<GameObject> connectedGO = new List<GameObject>();

    public bool GetElectrified()
    {
        return electrified;
    }
    public void SetElectrified(bool state)
    {
        electrified = state;
        if (connectedGO.Count > 0 && electrified)
        {
            foreach (GameObject electric in connectedGO)
            {
                //metal
                if (electric.gameObject.layer == 11)
                {
                    electric.GetComponent<Metal>().SetElectrified(true);
                }
                //water
                else if (electric.gameObject.layer == 4)
                {
                    electric.GetComponent<Water>().SetElectrified(true);
                }
            }
        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        //If rope is hit by lightning...
        if (collision.CompareTag("Lightning"))
        {
            if (!electrified)
            {
                SetElectrified(true);
            }
        }
    }

}
