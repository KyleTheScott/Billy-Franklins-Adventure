using System.Collections.Generic;
using UnityEngine;

public class MovingObjectsCollision : MonoBehaviour
{
    //private GameObject player = null;

    //private CheckPointSystem checkPointSystem = null;
    //public List<GameObject> checkForElectric = new List<GameObject>();


    //private void Start()
    //{
    //    player = player = FindObjectOfType<Player>().gameObject;
    //    checkPointSystem = GameObject.Find("GlobalGameController").GetComponent<CheckPointSystem>();
    //}

    //private void Update()
    //{
    //    if(checkForElectric.Count > 0)
    //    {
    //        foreach (GameObject item in checkForElectric)
    //        {
    //            //water
    //            if (item.CompareTag("Water") && item.GetComponent<Water>().GetElectrified())
    //            {
    //                EmptyObjects();
    //                checkPointSystem.PlayerDeath();
    //                break;
    //            }
    //            //metal
    //            else if (item.CompareTag("Metal") && item.GetComponent<Metal>().GetElectrified())
    //            {
    //                if (item.GetComponent<Metal>().GetMovable())
    //                {
    //                    EmptyObjects();
    //                    checkPointSystem.PlayerDeath();
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}

    //public void EmptyObjects()
    //{
    //    checkForElectric.Clear();
    //    FindObjectOfType<PlayerObjectInteractions>().EmptyObjects();
    //    FindObjectOfType<ElectricityController>().EmptyObjects();
    //}


    //public void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Water") || collision.CompareTag("Metal"))
    //    {
    //        checkForElectric.Add(collision.gameObject);
    //    }
    //}
    //public void OnTriggerExit2D(Collider2D collision)
    //{
    //    if(collision.CompareTag("Water") || collision.CompareTag("Metal"))
    //    {
    //        checkForElectric.Remove(collision.gameObject);
    //    }
    //}
}
