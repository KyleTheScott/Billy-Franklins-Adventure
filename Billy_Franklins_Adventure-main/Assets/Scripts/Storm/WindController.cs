using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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
}
