using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalGameController : MonoBehaviour
{
    #region Singleton
    public static GlobalGameController instance;

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

    [SerializeField] int currentLitLanternNum = 0;
    [SerializeField] private int maxLitLanternNum = 3;


    [HideInInspector]
    public UnityEvent onAllLanternOn; //Invoke when all lantern is on, Lamp will subscribe this

    public void SetLanternAmount(int LanternNum, int CurrentLitLanternNum = 0)
    {
        Debug.Log(LanternNum + " " + CurrentLitLanternNum );
        currentLitLanternNum = CurrentLitLanternNum;
        maxLitLanternNum = LanternNum;
    }

    public void IncreaseCurrentLitLanternNum()
    {
        currentLitLanternNum += 1;
        Debug.Log("current lit: " + currentLitLanternNum);

        //When all latern is on...
        if(currentLitLanternNum == maxLitLanternNum)
        {
            //Invoke event
            if(onAllLanternOn != null)
            {
                onAllLanternOn.Invoke();
            }
        }

        //if(currentLitLanternNum > maxLitLanternNum)
        //{
        //    currentLitLanternNum = maxLitLanternNum;
        //    Debug.LogWarning("Current lit lantern number has exceeded maximum lit lantern number");
        //}
    }
}
