using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float lightningTimer = 0;
    [SerializeField] private float maxLightningTimer = 10;

    [SerializeField] private float minLightLightningTimer = 5;
    [SerializeField] private float maxLightLightningTimer = 20;

    [SerializeField] private float minMediumLightningTimer = 2;
    [SerializeField] private float maxMediumLightningTimer = 10;

    [SerializeField] private float minHeavyLightningTimer = 0;
    [SerializeField] private float maxHeavyLightningTimer = 5;

    private bool noLightning;

    private void Start()
    {
        LightningController.instance.StrikeLightningEvent.AddListener(SetLightning);
        LightningController.instance.CalmLightningEvent.AddListener(SetLightning);
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
                noLightning = false;
                break;
            case LightningController.LightningState.LIGHT_LIGHTNING:
                maxLightningTimer = Random.Range(minLightLightningTimer, maxLightLightningTimer);
                lightningTimer = 0;
                break;
            case LightningController.LightningState.MEDIUM_LIGHTNING:
                maxLightningTimer = Random.Range(minMediumLightningTimer, maxMediumLightningTimer);
                lightningTimer = 0;
                break;
            case LightningController.LightningState.HEAVY_LIGHTNING:
                maxLightningTimer = Random.Range(minHeavyLightningTimer, maxHeavyLightningTimer);
                lightningTimer = 0;
                break;
        }
    }

    private void StrikeLightning()
    {
        SetLightning();

        //put lightning code here
        Debug.Log("LIGHTNING");
    }

}
