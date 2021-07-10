using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalImpactScript : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidbody;
    [Header("FMOD Settings")]
    [FMODUnity.EventRef]
    [SerializeField]
    private string metalCrashEventRef;
    [SerializeField]
    private float rigidbodyImpactRequirement = 1.0f; // How fast the metal is going to trigger the sound effect

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8 && collision.collider.CompareTag("Ground") && rigidbody.velocity.magnitude >= rigidbodyImpactRequirement)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(metalCrashEventRef, gameObject);
        }
    }
}
