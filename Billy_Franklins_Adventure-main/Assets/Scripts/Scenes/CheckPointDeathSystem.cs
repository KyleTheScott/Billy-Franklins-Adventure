using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointDeathSystem : MonoBehaviour
{
    private GameObject player = null;
    private CheckPoint checkPoint = null;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    public void SetCheckpoint()
    {
        //Debug.Log("SetCheckpoint() called");

        //initialize checkpoint
        if(checkPoint == null)
        {
            checkPoint = new CheckPoint();
        }
        //set location and number of charges at checkpoint
        checkPoint.SetCheckPointLocation(player.GetComponent<Transform>().localPosition);
        checkPoint.SetCheckPointCharges(player.GetComponent<Player>().lightCharges);

        //Debug.Log("Checkpoint staus (Set): Location= " + checkPoint.GetCheckPointLocation() + ", charges= " + checkPoint.GetCheckPointCharges());
    }

    public void PlayerDeath()
    {
        //if there is a check point then load it
        if(checkPoint != null)
        {
            //Debug.Log("Checkpoint staus (Death): Location= " + checkPoint.GetCheckPointLocation() + ", charges= " + checkPoint.GetCheckPointCharges());
            player.transform.localPosition = checkPoint.GetCheckPointLocation();
            player.GetComponent<Player>().lightCharges = checkPoint.GetCheckPointCharges() + 1;
            player.GetComponent<Player>().UseLightCharges();
        }
        //if no checkpoint then reload level?
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    private class CheckPoint
    {
        private Vector2 location = Vector2.zero;
        private int charges = 0;

        //used to set location variable in class
        public void SetCheckPointLocation(Vector2 position)
        {
            location = position;
        }
        //used to set charges variable in class
        public void SetCheckPointCharges(int numCharges)
        {
            charges = numCharges;
        }
        public Vector2 GetCheckPointLocation()
        {
            return location;
        }
        public int GetCheckPointCharges()
        {
            return charges;
        }

    }
}
