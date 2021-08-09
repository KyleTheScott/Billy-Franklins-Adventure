using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float lightningTimer = 0;
    [SerializeField] private float maxLightningTimer = 10;

    [SerializeField] private float minLightLightningTimer = 6;
    [SerializeField] private float maxLightLightningTimer = 8;

    [SerializeField] private float minMediumLightningTimer = 4;
    [SerializeField] private float maxMediumLightningTimer = 6;

    [SerializeField] private float minHeavyLightningTimer = 2;
    [SerializeField] private float maxHeavyLightningTimer = 4;

    private bool noLightning;

    private GameObject lightningStrikes = null;
    [SerializeField] private List<LightningGroup> lightningGroups = new List<LightningGroup>();


    private void Start()
    {
        LightningController.instance.StrikeLightningEvent.AddListener(SetLightning);
        LightningController.instance.CalmLightningEvent.AddListener(SetLightning);

        lightningStrikes = transform.Find("LightningStrikes").gameObject;
        foreach (Transform l in lightningStrikes.transform)
        {
            if (l.name == "LightningGroup")
            {
                lightningGroups.Add(l.GetComponent<LightningGroup>());
            }
        }
    }

    private void Update()
    {
        if (!noLightning && lightningTimer >= maxLightningTimer)
        {
            lightningTimer = 0;
            StrikeLightning();
        }
        lightningTimer += Time.deltaTime;
    }

    public void SetLightning()
    {
        switch (LightningController.instance.GetLightningState())
        {
            case LightningController.LightningState.NO_LIGHTNING:
                noLightning = true;
                break;
            case LightningController.LightningState.LIGHT_LIGHTNING:
                maxLightningTimer = Random.Range(minLightLightningTimer, maxLightLightningTimer);
                lightningTimer = 0;
                noLightning = false;
                break;
            case LightningController.LightningState.MEDIUM_LIGHTNING:
                maxLightningTimer = Random.Range(minMediumLightningTimer, maxMediumLightningTimer);
                lightningTimer = 0;
                noLightning = false;
                break;
            case LightningController.LightningState.HEAVY_LIGHTNING:
                maxLightningTimer = Random.Range(minHeavyLightningTimer, maxHeavyLightningTimer);
                lightningTimer = 0;
                noLightning = false;
                break;
        }
    }

    private void StrikeLightning()
    {
        SetLightning();
        
        int lightningAmount = Random.Range(0, lightningGroups.Count) + 1;//number of lightning strikes
        List<int> lightningPositions = new List<int>();

        for (int i = 0; i < lightningGroups.Count; i++)
        {
            lightningPositions.Add(i);
        }
        List<int> lightningChosenPositions = new List<int>();
        for (int i = 0; i < lightningAmount; i++)
        {
            int currentPos = Random.Range(0, lightningPositions.Count); 
            lightningChosenPositions.Add(lightningPositions[currentPos]); 
            lightningPositions.RemoveAt(currentPos);
        }
        for (int i = 0; i < lightningChosenPositions.Count; i++)
        {
            lightningGroups[lightningChosenPositions[i]].StrikeLightning();
        }

        //put lightning code here
        //Debug.Log("LIGHTNING");
    }

}
