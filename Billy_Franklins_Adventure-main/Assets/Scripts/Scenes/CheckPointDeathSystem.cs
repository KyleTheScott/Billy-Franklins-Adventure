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
    //called by external script?
    public void SetCheckpoint()
    {
        //initialize checkpoint
        if(checkPoint == null)
        {
            checkPoint = new CheckPoint();
        }
        //set location and number of charges at checkpoint
        checkPoint.SetCheckPointLocation(player.GetComponent<Transform>().position);
        checkPoint.SetCheckPointCharges(player.GetComponent<Player>().lightCharges);
    }

    public void PlayerDeath()
    {
        //if there is a check point then load it
        if(checkPoint != null)
        {
            player.transform.position = checkPoint.GetCheckPointLocation();
            player.GetComponent<Player>().lightCharges = checkPoint.GetCheckPointCharges();
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
