using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField] private int lightningAmount;
    [SerializeField] private int lightningCount;

    private enum LightningState
    {
        NO_LIGHTNING,
        LIGHT_LIGHTNING,
        MEDIUM_LIGHTNING,
        HEAVY_LIGHTNING
    }

    [SerializeField] private LightningState lightningState = LightningState.LIGHT_LIGHTNING;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StrikeLightning()
    {

    }
    public void CalmLightning()
    {

    }

}
