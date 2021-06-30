using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODStudioFootstepScript : MonoBehaviour
{
    [Header("FMOD Settings")]
    [SerializeField] [FMODUnity.EventRef] private string FootstepsEventPath;
    [SerializeField] private string MaterialParameterName;

    [Header("Playback Settings")]
    [SerializeField] private float StepDistance = 2.0f;
    [SerializeField] private float RayDistance = 1.2f;
    [SerializeField] private LayerMask materialCheckLayerMask;
    //[SerializeField] private string JumpInputName;

    //public string[] MaterialTypes;
    [HideInInspector] public float DefulatMaterialValue = 0.0f;

    private Player player;

    //Variables for footstep execution
    private float StepRandom;
   
    private Vector3 PrevPos;
    private float DistanceTravelled;
    //Variables for checking Material types
    private RaycastHit2D hit;
    private float FMOD_MaterialValue;

    private void Start()
    {
        StepRandom = Random.Range(0f, 0.5f);
        PrevPos = transform.position;
        player = GetComponent<Player>();
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * RayDistance, Color.blue);

        DistanceTravelled += (transform.position - PrevPos).magnitude;
        if (DistanceTravelled >= StepDistance + StepRandom)
        {
            MaterialCheck();
            PlayerFootsteep();
            StepRandom = Random.Range(0f, 0.5f);
            DistanceTravelled = 0f;
        }
        PrevPos = transform.position;
    }

    private void PlayerFootsteep()
    {
        if (player.PlayersState == Player.PlayerState.WALKING)
        {
            FMOD.Studio.EventInstance Footstep = FMODUnity.RuntimeManager.CreateInstance(FootstepsEventPath);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(Footstep, transform, GetComponent<Rigidbody>());
            Footstep.setParameterByName(MaterialParameterName, FMOD_MaterialValue);
            Footstep.start();
            Footstep.release();
        }
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
