using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class RainController : MonoBehaviour
{
    #region Singleton
    public static RainController instance;

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
    public enum RainAmountState
    {
        NO_RAIN,
        LIGHT_RAIN,
        MEDIUM_RAIN,
        HEAVY_RAIN
    }

    [SerializeField] private RainAmountState rainAmountState = RainAmountState.LIGHT_RAIN;


    public UnityEvent RainEvent;
    public UnityEvent CalmRainEvent;

    [SerializeField] private float rainAmount = 100;

    [Header("FMOD Settings")]
    [SerializeField] [FMODUnity.EventRef] 
    private string rainSoundEventPath;
    private FMOD.Studio.EventInstance rainSoundEvent;
    private float rainVolume;
    [SerializeField]
    private float lightRainVolume = 0.2f;
    [SerializeField]
    private float mediumRainVolume = 0.6f;
    [SerializeField]
    private float heavyRainVolume = 1.2f;
    [SerializeField]
    private float volumeChangeRate = 1.0f;

    public void Start()
    {
        rainSoundEvent = FMODUnity.RuntimeManager.CreateInstance(rainSoundEventPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(rainSoundEvent, transform, GetComponent<Rigidbody>());
        switch (rainAmountState)
        {
            case (RainAmountState.NO_RAIN):
                rainVolume = 0;
                break;

            case (RainAmountState.LIGHT_RAIN):
                rainVolume = lightRainVolume;
                break;

            case (RainAmountState.MEDIUM_RAIN):
                rainVolume = mediumRainVolume;
                break;

            case (RainAmountState.HEAVY_RAIN):
                rainVolume = heavyRainVolume;
                break;
        }

        rainSoundEvent.setVolume(rainVolume);
        rainSoundEvent.start();
    }


    public float GetRainAmount()
    {
        return rainAmount;
    }

    public RainAmountState GetRainState()
    {
        return rainAmountState;
    }

    public void Rain()
    {
        switch (rainAmountState)
        {
            case RainAmountState.NO_RAIN:
                SetNoRain(false);
                break;
            case RainAmountState.LIGHT_RAIN:
                SetLightRain(false);
                break;
            case RainAmountState.MEDIUM_RAIN:
                SetMediumRain(false);
                break;
            case RainAmountState.HEAVY_RAIN:
                SetHeavyRain(false);
                break;
        }

        SetRainAmount();
        RainEvent.Invoke();
    }


    public void CalmRain()
    {
        switch (rainAmountState)
        {
            case RainAmountState.NO_RAIN:
                SetNoRain(true);
                break;
            case RainAmountState.LIGHT_RAIN:
                SetLightRain(true);
                break;
            case RainAmountState.MEDIUM_RAIN:
                SetMediumRain(true);
                break;
            case RainAmountState.HEAVY_RAIN:
                SetHeavyRain(true);
                break;
        }

        SetRainAmount();
        CalmRainEvent.Invoke();
    }

    private void SetRainAmount()
    {
        rainAmount = 100;
        switch (rainAmountState)
        {
            case RainAmountState.NO_RAIN:
                StartCoroutine(ChangeRainVolume(0.001f));
                break;
            case RainAmountState.LIGHT_RAIN:
                rainAmount = Random.Range(0, 50);
                StartCoroutine(ChangeRainVolume(lightRainVolume));
                break;
            case RainAmountState.MEDIUM_RAIN:
                rainAmount = Random.Range(50, 100);
                StartCoroutine(ChangeRainVolume(mediumRainVolume));
                break;
            case RainAmountState.HEAVY_RAIN:
                StartCoroutine(ChangeRainVolume(heavyRainVolume));
                rainAmount = Random.Range(100, 200);
                break;
        }
    }

    private void SetNoRain(bool calm)
    {
        if (calm)
        {
            rainAmountState = RainAmountState.NO_RAIN;
        }
        else
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                case 1:
                case 2:
                    rainAmountState = RainAmountState.LIGHT_RAIN;
                    break;
                case 3:
                    rainAmountState = RainAmountState.NO_RAIN;
                    break;
            }
        }
    }

    private void SetLightRain(bool calm)
    {
        if (calm)
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                    rainAmountState = RainAmountState.NO_RAIN;
                    break;
                case 1:
                case 2:
                case 3:
                    rainAmountState = RainAmountState.LIGHT_RAIN;
                    break;
            }
        }
        else
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                case 1:
                case 2:
                    rainAmountState = RainAmountState.MEDIUM_RAIN;
                    break;
                case 3:
                    rainAmountState = RainAmountState.LIGHT_RAIN;
                    break;
                case 4:
                    rainAmountState = RainAmountState.HEAVY_RAIN;
                    break;
            }
        }
    }
    private void SetMediumRain(bool calm)
    {
        if (calm)
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                case 1:
                case 2:
                    rainAmountState = RainAmountState.LIGHT_RAIN;
                    break;
                case 3:
                    rainAmountState = RainAmountState.MEDIUM_RAIN;
                    break;
                case 4:
                    rainAmountState = RainAmountState.NO_RAIN;
                    break;
            }
        }
        else
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                case 1:
                case 2:
                    rainAmountState = RainAmountState.HEAVY_RAIN;
                    break;
                case 3:
                    rainAmountState = RainAmountState.MEDIUM_RAIN;
                    break;
            }
        }
    }
    private void SetHeavyRain(bool calm)
    {
        if (calm)
        {
            switch (Random.Range(0, 9))
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    rainAmountState = RainAmountState.MEDIUM_RAIN;
                    break;
                case 4:
                case 5:
                    rainAmountState = RainAmountState.HEAVY_RAIN;
                    break;
                case 6:
                case 7:
                    rainAmountState = RainAmountState.LIGHT_RAIN;
                    break;
                case 8:
                    rainAmountState = RainAmountState.NO_RAIN;
                    break;
            }
        }
        else
        {
            rainAmountState = RainAmountState.HEAVY_RAIN;
        }
    }

    private IEnumerator ChangeRainVolume(float newVolume)
    {
        if (newVolume == rainVolume) // If the newVolume then stop the Coroutine
        {
            StopCoroutine(ChangeRainVolume(newVolume));
        }
        bool increasingVolume = rainVolume < newVolume;
        
        while (increasingVolume ? rainVolume < newVolume : rainVolume > newVolume)
        {
            rainVolume = (increasingVolume ? rainVolume + volumeChangeRate * Time.deltaTime : rainVolume - volumeChangeRate * Time.deltaTime);
            rainSoundEvent.setVolume(rainVolume);
            yield return new WaitForEndOfFrame();
        }

        rainVolume = newVolume;
        rainSoundEvent.setVolume(rainVolume);
    }
}
