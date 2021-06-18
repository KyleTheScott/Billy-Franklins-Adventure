using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class StormController : MonoBehaviour
{
    #region Singleton
    public static StormController instance;

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

    [SerializeField] private float stormTimerMin = 2;
    [SerializeField] private float stormTimerMax = 10;
    [SerializeField] private float currentStormTimerMax = 10;

    [SerializeField] private float stormTimer = 2;

    [SerializeField] private float rainCounter = 10;

    public enum StormState
    {
        CALM,
        WIND,
        RAIN
    }

    [SerializeField] private StormState stormState = StormState.CALM;

    public enum CalmState
    {
        NOT_CALM,
        CALM,
        CALM_LIGHTNING
    }

    [SerializeField] private CalmState calmState = CalmState.NOT_CALM;
    [SerializeField] private CalmState lastCalmState = CalmState.NOT_CALM;

    public enum WindState
    {
        NO_WIND,
        WIND,
        WIND_LIGHTNING,
        WIND_RAIN,
        WIND_RAIN_LIGHTNING
    }

    [SerializeField] private WindState windState = WindState.NO_WIND;
    [SerializeField] private WindState lastWindState = WindState.NO_WIND;

    public enum RainState
    {
        NO_RAIN,
        RAIN,
        RAIN_LIGHTNING
    }

    [SerializeField] private RainState rainState = RainState.NO_RAIN;
    [SerializeField] private RainState lastRainState = RainState.NO_RAIN;

    // Start is called before the first frame update
    void Start()
    {
        SetStormTimer();
    }

    // Update is called once per frame
    void Update()
    {
        StormCounter();
    }

    private void SetStormTimer()
    {
        stormTimer = 0;
        currentStormTimerMax = Random.Range(stormTimerMin, stormTimerMax);
    }
    private void StormCounter()
    {
        if (stormTimer >= currentStormTimerMax)
        {
            ChooseStormType();
            SetStormTimer();
        }
        stormTimer += Time.deltaTime;
    }
    private void ChooseStormType()
    {
        switch (stormState)
        {
            case StormState.CALM:
                DecideStormCalm();
            break;
            case StormState.WIND:
                DecideStormWind();
            break;
            case StormState.RAIN:
                DecideStormRain();
            break;
        }
    }
    //Deciding what storm state to switch to
    private void DecideStormCalm()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                ChooseStormWind();
                break;
            case 1:
                ChooseStormRain();
                break;
            case 2:
            case 3:
            case 4:
                ChooseStormCalm();
                break;
        }
    }
    private void DecideStormWind()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                ChooseStormCalm();
                break;
            case 1:
                ChooseStormRain();
                break;
            case 2:
            case 3:
            case 4:
                ChooseStormWind();
                break;
        }
    }
    private void DecideStormRain()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                ChooseStormCalm();
                break;
            case 1:
                ChooseStormWind();
                break;
            case 2:
            case 3:
            case 4:
                ChooseStormRain();
                break;
        }
    }

    //choosing the subcategories
    private void ChooseStormCalm()
    {
        rainState = RainState.NO_RAIN;
        windState = WindState.NO_WIND;
        stormState = StormState.CALM;
        switch (Random.Range(0, 3))
        {
            case 0:
            case 1:
                if (calmState != CalmState.CALM)
                {
                    if (calmState == CalmState.CALM_LIGHTNING)
                    {
                        LightningController.instance.CalmLightning();
                    }
                    calmState = CalmState.CALM;
                }
                if (windState != WindState.NO_WIND)
                {
                    windState = WindState.NO_WIND;
                    WindController.instance.CalmWind();
                }
                if (rainState != RainState.NO_RAIN)
                {
                    rainState = RainState.NO_RAIN;
                    RainController.instance.CalmRain();
                }
                break;
            case 2:
                if (calmState != CalmState.CALM_LIGHTNING)
                {
                    calmState = CalmState.CALM_LIGHTNING;
                    LightningController.instance.CalmLightning();
                }
                if (windState != WindState.NO_WIND)
                {
                    windState = WindState.NO_WIND;
                    WindController.instance.CalmWind();
                }
                if (rainState != RainState.NO_RAIN)
                {
                    rainState = RainState.NO_RAIN;
                    RainController.instance.CalmRain();
                }
                break;
        }
    }
    private void ChooseStormWind()
    {
        calmState = CalmState.NOT_CALM;
        stormState = StormState.WIND;
        switch (Random.Range(0, 5))
        {
            case 0:
            case 1:
                rainState = RainState.NO_RAIN;
                windState = WindState.WIND;
                WindController.instance.BlowWind();
                RainController.instance.CalmRain();
                LightningController.instance.CalmLightning();
                break;
            case 2:
                rainState = RainState.NO_RAIN;
                windState = WindState.WIND_LIGHTNING;
                WindController.instance.BlowWind();
                RainController.instance.CalmRain();
                LightningController.instance.StrikeLightning();
                break;
            case 3:
                rainState = RainState.RAIN;
                windState = WindState.WIND_RAIN;
                WindController.instance.BlowWind();
                RainController.instance.Rain();
                LightningController.instance.CalmLightning();
                break;
            case 4:
                rainState = RainState.RAIN_LIGHTNING;
                windState = WindState.WIND_RAIN_LIGHTNING;
                WindController.instance.BlowWind();
                RainController.instance.Rain();
                LightningController.instance.StrikeLightning();
                break;
        }
    }
    private void ChooseStormRain()
    {
        calmState = CalmState.NOT_CALM;
        windState = WindState.NO_WIND;
        stormState = StormState.RAIN;
        switch (rainState)
        {
            case RainState.NO_RAIN:
                WindController.instance.CalmWind();
                RainController.instance.CalmRain();
                LightningController.instance.CalmLightning();
                break;
            case RainState.RAIN:
                WindController.instance.CalmWind();
                RainController.instance.Rain();
                LightningController.instance.CalmLightning();
                break;
            case RainState.RAIN_LIGHTNING:
                WindController.instance.CalmWind();
                RainController.instance.Rain();
                LightningController.instance.StrikeLightning();
                break;
        }
    }
}
