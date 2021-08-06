using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODStudioFootstepScript : MonoBehaviour
{
    [Header("FMOD Settings")]
    [SerializeField] [FMODUnity.EventRef] private string FootstepsEventPath;
    [SerializeField] private string MaterialParameterName;
    [SerializeField] private float footstepVolume = 0.8f;

    [Header("Playback Settings")]
    [SerializeField] private float RayDistance = 1.2f;
    [SerializeField] private LayerMask materialCheckLayerMask;

    //public string[] MaterialTypes;
    [HideInInspector] public float DefulatMaterialValue = 0.0f;

    //Variables for checking Material types
    private RaycastHit2D hit;
    private float FMOD_MaterialValue;

    public void PlayerFootsteep()
    {
        MaterialCheck();

        FMOD.Studio.EventInstance Footstep = FMODUnity.RuntimeManager.CreateInstance(FootstepsEventPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Footstep, transform, GetComponent<Rigidbody>());
        Footstep.setParameterByName(MaterialParameterName, FMOD_MaterialValue);
        Footstep.setVolume(footstepVolume);
        Footstep.start();
        Footstep.release();
        
    }

    private void MaterialCheck()
    {
        hit = Physics2D.Raycast(transform.position, Vector3.down, RayDistance, materialCheckLayerMask);

        if (hit.collider != null && hit.collider.gameObject.GetComponent<FMODStudioMaterialSetter>())
        {
            FMOD_MaterialValue = hit.collider.gameObject.GetComponent<FMODStudioMaterialSetter>().MaterialValue;
           
        }
        else
        {
            FMOD_MaterialValue = DefulatMaterialValue;
        }
     
    }
}
