using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KiteLightning : MonoBehaviour
{
    private Animator kiteAnimator;

    [SerializeField]
    [FMODUnity.EventRef]
    private string kiteLightingEventRef;
    [SerializeField]
    private float kiteLightingVolume = 0.8f;

    void Start()
    {
        kiteAnimator = gameObject.GetComponent<Animator>();
    }

    public void KiteElectrifyStart()
    {
        kiteAnimator.SetBool("StrikeLightning", true);
        FMODUnity.RuntimeManager.PlayOneShot(kiteLightingEventRef, kiteLightingVolume);
    }

    public void KiteElectrifyEnd()
    {
        kiteAnimator.SetBool("StrikeLightning", false);
    }
}
