﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Charges : MonoBehaviour
{
    [SerializeField] private int lightCharges = 3; //how many lighting can character use?
    [SerializeField] private int maxLightCharges = 3; //how many lighting can character use?
    [SerializeField] private bool lampOn = false;
    public UnityEvent<int, int> onLightChargesChanged; //DarkBOrder will subscribe, charges text ui will subscribe this

    [FMODUnity.EventRef]
    public string shootSound;

    public void SetLampOn(bool state)
    {
        lampOn = state;
    }

    public bool GetLampOn()
    {
        return lampOn;
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }

    public void SetMaxLightCharges(int chargeNum)
    {
        maxLightCharges = chargeNum;
    }



    public void UseLightCharges()
    {
        lightCharges -= 1;
        FMODUnity.RuntimeManager.PlayOneShot(shootSound);

        if (onLightChargesChanged != null)
        {
            onLightChargesChanged.Invoke(lightCharges, maxLightCharges);
        }
    }
}
