using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class LightningController : MonoBehaviour
{
    #region Singleton
    public static LightningController instance;

    private void Awake()
    {
        //Make sure there is only one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    public UnityEvent StrikeLightningEvent;
    public UnityEvent CalmLightningEvent;

    //[SerializeField] private int lightningAmount;
    //[SerializeField] private int lightningCount;

    

    public enum LightningState
    {
        NO_LIGHTNING,
        LIGHT_LIGHTNING,
        MEDIUM_LIGHTNING,
        HEAVY_LIGHTNING
    }

    [SerializeField] private LightningState lightningState = LightningState.LIGHT_LIGHTNING;

    public LightningState GetLightningState()
    {
        return lightningState;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void StrikeLightning()
    {
        switch (lightningState)
        {
            case LightningState.NO_LIGHTNING:
                StrikeNoLightning();
                break;
            case LightningState.LIGHT_LIGHTNING:
                StrikeLightLightning();
                break;
            case LightningState.MEDIUM_LIGHTNING:
                StrikeMediumLightning();
                break;
        }
        StrikeLightningEvent.Invoke();
    }
    public void CalmLightning()
    {
        switch (lightningState)
        {
            case LightningState.LIGHT_LIGHTNING:
                CalmLightLightning();
                break;
            case LightningState.MEDIUM_LIGHTNING:
                CalmMediumLightning();
                break;
            case LightningState.HEAVY_LIGHTNING:
                CalmHeavyLightning();
                break;
        }
        CalmLightningEvent.Invoke();
    }

    private void StrikeNoLightning()
    {
        switch (Random.Range(0,6))
        {
            case 0:
            case 1:
            case 2:
                lightningState = LightningState.LIGHT_LIGHTNING;
                break;
            case 3:
            case 4:
                lightningState = LightningState.MEDIUM_LIGHTNING;
                break;
            case 5:
                lightningState = LightningState.HEAVY_LIGHTNING;
                break;
        }
    }

    private void StrikeLightLightning()
    {
        switch (Random.Range(0, 6))
        {
            case 0:
            case 1:
            case 2:
                lightningState = LightningState.MEDIUM_LIGHTNING;
                break;
            case 3:
            case 4:
                lightningState = LightningState.HEAVY_LIGHTNING;
                break;
            case 5:
                lightningState = LightningState.LIGHT_LIGHTNING;
                break;
        }
    }
    private void StrikeMediumLightning()
    {
        switch (Random.Range(0, 5))
        {
            case 0:
            case 1:
            case 2:
                lightningState = LightningState.HEAVY_LIGHTNING;
                break;
            case 3:
            case 4:
                lightningState = LightningState.MEDIUM_LIGHTNING;
                break;
        }
    }

    private void CalmLightLightning()
    {
        switch (Random.Range(0, 5))
        {
            case 0:
            case 1:
            case 2:
                lightningState = LightningState.NO_LIGHTNING;
                break;
            case 3:
            case 4:
                lightningState = LightningState.LIGHT_LIGHTNING;
                break;
        }
    }
    private void CalmMediumLightning()
    {
        switch (Random.Range(0, 6))
        {
            case 0:
            case 1:
            case 2:
                lightningState = LightningState.MEDIUM_LIGHTNING;
                break;
            case 3:
            case 4:
                lightningState = LightningState.LIGHT_LIGHTNING;
                break;
            case 5:
                lightningState = LightningState.NO_LIGHTNING;
                break;
        }
    }
    private void CalmHeavyLightning()
    {
        switch (Random.Range(0, 6))
        {
            case 0:
            case 1:
            case 2:
                lightningState = LightningState.MEDIUM_LIGHTNING;
                break;
            case 3:
            case 4:
                lightningState = LightningState.LIGHT_LIGHTNING;
                break;
            case 5:
                lightningState = LightningState.NO_LIGHTNING;
                break;
        }
    }
}
