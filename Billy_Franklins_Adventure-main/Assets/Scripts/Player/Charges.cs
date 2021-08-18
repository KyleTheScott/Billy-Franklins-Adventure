using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Charges : MonoBehaviour
{
    [SerializeField] private int lightCharges = 3; //how many lighting can character use?
    [SerializeField] private int maxLightCharges = 3; //how many lighting can character use?
    [SerializeField] private bool lampOn = false;
    public UnityEvent<int, int> onLightChargesChanged; //DarkBorder will subscribe, charges text ui will subscribe this

    public List<GameObject> keyChargesList = new List<GameObject>();

    [FMODUnity.EventRef]
    public string shootSound;
    [SerializeField]
    private float shootVolume = 0.8f;

    public void SetLampOn(bool state)
    {
        lampOn = state;
    }

    public bool GetLampOn()
    {
        return lampOn;
    }


    public int GetLightCharges()
    {
        return lightCharges;
    }

    public int GetMaxLightCharges()
    {
        return maxLightCharges;
    }

    public void SetLightCharges(int chargeNum)
    {
        lightCharges = chargeNum;

        //add keys to list
        if(keyChargesList.Count == 0)
        {
            for(byte i=0; i < 12; i++)
            {
                keyChargesList.Add(GameObject.Find("KeyImage" + (i + 1)));
                //Debug.Log("keyChargesList[" + i + "] set. KeyImage" + (i + 1));
            }
        }
        for(int i=0; i < 12; i++)
        {
            if (i < chargeNum)
            {
                keyChargesList[i].SetActive(true);
            }
            else
            {
                keyChargesList[i].SetActive(false);
            }
        }
    }

    public void SetMaxLightCharges(int chargeNum)
    {
        maxLightCharges = chargeNum;
    }



    public void UseLightCharges()
    {
        if (lightCharges <= 0)
        {
            return;
        }
        lightCharges -= 1;
        FMODUnity.RuntimeManager.PlayOneShot(shootSound, shootVolume);

        for (int i = 0; i < 12; i++)
        {
            if (i < lightCharges)
            {
                keyChargesList[i].SetActive(true);
            }
            else
            {
                keyChargesList[i].SetActive(false);
            }
        }

        if (onLightChargesChanged != null)
        {
            onLightChargesChanged.Invoke(lightCharges, maxLightCharges);
        }

        //if there are no charges left then player has died
        if (lightCharges <= 0)
        {
            StartCoroutine(WaitForLatern());
        }
    }

    private IEnumerator WaitForLatern()
    {
        yield return new WaitForSeconds(1.0f);
        
        if (!lampOn)
        {
            //Debug.LogError("Out of charges");
            FindObjectOfType<Player>().StartPlayerOutOfChargesDeath();
           
        }

    }
}
