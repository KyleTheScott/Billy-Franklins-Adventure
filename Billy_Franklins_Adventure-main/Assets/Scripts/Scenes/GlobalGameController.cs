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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseCurrentLitLanternNum()
    {
        currentLitLanternNum += 1;

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
