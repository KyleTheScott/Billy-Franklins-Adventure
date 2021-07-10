using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspendedPlatformGroundCollision : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SuspendedPlatform suspendedPlatform;
    [Header("FMOD Settings")]
    [FMODUnity.EventRef]
    [SerializeField]
    private string woodCrashEventRef;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8 && collision.collider.CompareTag("Ground"))
        {
            Debug.Log("HitGround");
            Debug.Log(suspendedPlatform.platformRigidbody.velocity.magnitude);
            FMODUnity.RuntimeManager.PlayOneShot(woodCrashEventRef);
            suspendedPlatform.SetKinematic();
        }
    }
}
