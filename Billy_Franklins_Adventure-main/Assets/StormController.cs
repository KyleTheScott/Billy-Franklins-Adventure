using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class StormController : MonoBehaviour
{
    [SerializeField] private float stormTimerMin = 2;
    [SerializeField] private float stormTimerMax = 10;
    [SerializeField] private float currentStormTimerMax = 10;

    [SerializeField] private float stormTimer = 0;

    [SerializeField] private float rainCounter = 10;

    [SerializeField] private UnityEvent BlowWind;
    [SerializeField] private UnityEvent CalmWind;
    [SerializeField] private UnityEvent Rain;
    [SerializeField] private UnityEvent CalmRain;
    [SerializeField] private UnityEvent StrikeLightning;
    [SerializeField] private UnityEvent CalmLightning;
    

    private enum StormState
    {
        CALM,
        WIND,
        RAIN
    }

    [SerializeField] private StormState stormState = StormState.CALM;

    private enum CalmState
    {
        NOT_CALM,
        CALM,
        CALM_LIGHTNING
    }

    [SerializeField] private CalmState calmState = CalmState.NOT_CALM;
    [SerializeField] private CalmState lastCalmState = CalmState.NOT_CALM;

    private enum WindState
    {
        NO_WIND,
        WIND,
        WIND_LIGHTNING,
        WIND_RAIN,
        WIND_RAIN_LIGHTNING
    }

    [SerializeField] private WindState windState = WindState.NO_WIND;
    [SerializeField] private WindState lastWindState = WindState.NO_WIND;

    private enum RainState
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
                ChooseStormCalm();
            break;
            case StormState.WIND:
                ChooseStormWind();
            break;
            case StormState.RAIN:
                ChooseStormRain();
            break;
        }
    }
    //Deciding what storm state to switch to
    private void DecideStormCalm()
    {
        switch (Random.Range(0, 4))
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
        switch (Random.Range(0, 4))
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
        switch (Random.Range(0, 4))
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
    //BlowWind;
    //CalmWind;
    //Rain;
    //CalmRain;
    //StrikeLightning;
    //CalmLightning;

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
                        CalmLightning.Invoke();
                    }
                    calmState = CalmState.CALM;
                }
                if (windState != WindState.NO_WIND)
                {
                    windState = WindState.NO_WIND;
                    CalmWind.Invoke();
                }
                if (rainState != RainState.NO_RAIN)
                {
                    rainState = RainState.NO_RAIN;
                    CalmRain.Invoke();
                }
                break;
            case 2:
                if (calmState != CalmState.CALM_LIGHTNING)
                {
                    calmState = CalmState.CALM_LIGHTNING;
                    StrikeLightning.Invoke();
                }
                if (windState != WindState.NO_WIND)
                {
                    windState = WindState.NO_WIND;
                    CalmWind.Invoke();
                }
                if (rainState != RainState.NO_RAIN)
                {
                    rainState = RainState.NO_RAIN;
                    CalmRain.Invoke();
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
                break;
            case 2:
                rainState = RainState.NO_RAIN;
                windState = WindState.WIND_LIGHTNING;
                break;
            case 3:
                rainState = RainState.RAIN;
                windState = WindState.WIND_RAIN;
                break;
            case 4:
                rainState = RainState.RAIN_LIGHTNING;
                windState = WindState.WIND_RAIN_LIGHTNING;
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
                break;
            case RainState.RAIN:
                break;
            case RainState.RAIN_LIGHTNING:
                break;
        }
    }
}
