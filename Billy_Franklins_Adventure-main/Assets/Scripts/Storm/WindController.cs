using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

//comment to merge
public class WindController : MonoBehaviour
{
    #region Singleton
    public static WindController instance;

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

    [SerializeField] private float windSpeed = -20;

    public UnityEvent BlowWindEvent;
    public UnityEvent CalmWindEvent;

    [Header("FMOD Settings")]
    [SerializeField] [FMODUnity.EventRef]
    private string windSoundEventPath;
    private FMOD.Studio.EventInstance windSoundEvent;
    [SerializeField]
    private float lowWindVolume = 0.2f;
    [SerializeField]
    private float highWindVolume = 1.2f;
    private float windVolume = 0.001f;
    [SerializeField]
    private float volumeChangeRate = 1.0f;

    public void Start()
    {
        windSoundEvent = FMODUnity.RuntimeManager.CreateInstance(windSoundEventPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(windSoundEvent, transform, GetComponent<Rigidbody>());
        windSoundEvent.setVolume(windVolume);
        windSoundEvent.start();
    }
    public float GetWindSpeed()
    {
        return windSpeed;
    }

    public void BlowWind()
    {
        if (windSpeed >= 0)
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                case 1:
                case 2:
                    BlowWindRight();
                    break;
                case 3:
                    BlowWindLeft();
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
                    BlowWindLeft();
                    break;
                case 3:
                    BlowWindRight();
                    break;
            }
        }
        BlowWindEvent.Invoke();
        StartCoroutine(ChangeWindVolume(highWindVolume));
    }

    private void BlowWindRight()
    {
        if (windSpeed >= 0 && windSpeed <= 25)
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                case 1:
                case 2:
                    windSpeed = Random.Range(0, 26);
                    break;
                case 3:
                    windSpeed = Random.Range(26, 51);
                    break;
                case 4:
                    windSpeed = Random.Range(-25, 0);
                    break;
            }
        }
        else
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                case 1:
                    windSpeed = Random.Range(0, 26);
                    break;
                case 2:
                    windSpeed = Random.Range(26, 50);
                    break;
            }
        }
    }

    private void BlowWindLeft()
    {
        if (windSpeed >= -25 && windSpeed < 0)
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                case 1:
                case 2:
                    windSpeed = Random.Range(-25, 0);
                    break;
                case 3:
                    windSpeed = Random.Range(-50, -26);
                    break;
                case 4:
                    windSpeed = Random.Range(0, 26);
                    break;
            }
        }
        else
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                case 1:
                    windSpeed = Random.Range(-25, 0);
                    break;
                case 2:
                    windSpeed = Random.Range(-50, -25);
                    break;
            }
        }
    }
    public void CalmWind()
    {
        if (windSpeed >= 0)
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                case 1:
                case 2:
                    CalmWindRight();
                    break;
                case 3:
                    CalmWindLeft();
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
                    CalmWindLeft();
                    break;
                case 3:
                    CalmWindRight();
                    break;
            }
        }
        CalmWindEvent.Invoke();
        StartCoroutine(ChangeWindVolume(lowWindVolume));
    }

    private void CalmWindRight()
    {

        if (windSpeed >= 0 && windSpeed <= 25)
        {
            windSpeed = Random.Range(0, 10);
            
        }
        else
        {
            windSpeed = Random.Range(0, 20);
        }
    }

    private void CalmWindLeft()
    {
        if (windSpeed >= -25 && windSpeed < 0)
        {
            windSpeed = Random.Range(-10, 0);
        }
        else
        {
            windSpeed = Random.Range(-20, 0);
        }
    }

    private IEnumerator ChangeWindVolume(float newVolume)
    {
        if (newVolume == windVolume) // If the newVolume then stop the Coroutine
        {
            StopCoroutine(ChangeWindVolume(newVolume));
        }
        bool increasingVolume = windVolume < newVolume;

        while (increasingVolume ? windVolume < newVolume : windVolume > newVolume)
        {
            windVolume = (increasingVolume ? windVolume + volumeChangeRate * Time.deltaTime : windVolume - volumeChangeRate * Time.deltaTime);
            windSoundEvent.setVolume(windVolume);
            yield return new WaitForEndOfFrame();
        }

        windVolume = newVolume;
        windSoundEvent.setVolume(windVolume);
    }
}
