using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KiteLightning : MonoBehaviour
{
    private Animator kiteAnimator;

    void Start()
    {
        kiteAnimator = gameObject.GetComponent<Animator>();
    }

    public void KiteElectrifyStart()
    {
        kiteAnimator.SetBool("StrikeLightning", true);
    }

    public void KiteElectrifyEnd()
    {
        kiteAnimator.SetBool("StrikeLightning", false);
    }
}
